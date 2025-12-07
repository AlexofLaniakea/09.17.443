using UnityEngine;

public static class MathPlus
{
    private const float G = 6.67430e-11f;
    private const float EPSILON = 1e-10f;
    private const float SAFE_MIN_DISTANCE = 0.01f;
    private const float MAX_DISTANCE_STRAIGHT_LINE = 1000f;
    private const float MAX_ITERATIONS = 5;

    // Main O(1) analytical solution
    public static void ComputePlanetGravityMotion(
        Vector2 initialPosition,
        Vector2 initialVelocity,
        float totalTime,
        float planetMass,
        out Vector2 finalPosition,
        out Vector2 finalVelocity)
    {
        // Early exit cases
        if (planetMass <= 0 || totalTime <= 0)
        {
            finalPosition = initialPosition;
            finalVelocity = initialVelocity;
            return;
        }

        float mu = G * planetMass;
        Vector2 r0 = initialPosition;
        Vector2 v0 = initialVelocity;
        float r0Mag = r0.magnitude;

        // Handle zero or near-zero position
        if (r0Mag < EPSILON)
        {
            // At or very near center, just use linear motion
            finalPosition = v0 * totalTime;
            finalVelocity = v0;
            return;
        }

        // Calculate specific angular momentum (cross product in 2D gives scalar)
        float h = r0.x * v0.y - r0.y * v0.x;

        // SPECIAL CASE: Zero or very small angular momentum (radial motion)
        if (Mathf.Abs(h) < EPSILON)
        {
            SolveRadialMotion(r0, v0, mu, totalTime, out finalPosition, out finalVelocity);
            return;
        }

        // For very far distances, use straight line approximation
        if (r0Mag > MAX_DISTANCE_STRAIGHT_LINE)
        {
            finalPosition = r0 + v0 * totalTime;
            finalVelocity = v0;
            return;
        }

        // Calculate specific orbital energy
        float energy = v0.sqrMagnitude * 0.5f - mu / r0Mag;

        // Determine orbit type and solve
        if (Mathf.Abs(energy) < EPSILON)
        {
            // Parabolic orbit (rare) - use refined approximation
            SolveParabolicOrbit(r0, v0, mu, h, totalTime, out finalPosition, out finalVelocity);
        }
        else if (energy < 0)
        {
            // Elliptical orbit (bound)
            SolveEllipticalOrbit(r0, v0, mu, h, energy, totalTime, out finalPosition, out finalVelocity);
        }
        else
        {
            // Hyperbolic orbit (unbound) - use approximation without sinh/cosh
            SolveHyperbolicOrbitApprox(r0, v0, mu, h, energy, totalTime, out finalPosition, out finalVelocity);
        }
    }

    // Special case: Radial motion (h = 0)
    private static void SolveRadialMotion(
        Vector2 r0, Vector2 v0, float mu, float time,
        out Vector2 finalPos, out Vector2 finalVel)
    {
        Vector2 direction = r0.normalized;
        float r0Mag = r0.magnitude;
        float vr0 = Vector2.Dot(v0, direction); // Radial velocity component

        // Calculate total energy for the 1D radial motion
        float energy = vr0 * vr0 * 0.5f - mu / r0Mag;

        if (energy >= 0)
        {
            // Unbound radial motion (escape or approach from infinity)
            SolveRadialUnbound(r0Mag, vr0, mu, time, direction, out finalPos, out finalVel);
        }
        else
        {
            // Bound radial motion (oscillates through center)
            SolveRadialBound(r0Mag, vr0, mu, time, direction, out finalPos, out finalVel);
        }
    }

    // Unbound radial motion (hyperbolic radial)
    private static void SolveRadialUnbound(
        float r0, float vr0, float mu, float time,
        Vector2 direction,
        out Vector2 finalPos, out Vector2 finalVel)
    {
        float energy = vr0 * vr0 * 0.5f - mu / r0;
        
        // Use a simple analytic approximation
        if (time < 1f)
        {
            // Short time: use constant acceleration approximation
            float a0 = -mu / (r0 * r0);
            float r = r0 + vr0 * time + 0.5f * a0 * time * time;
            float vr = vr0 + a0 * time;
            
            // Ensure minimum distance
            if (r < SAFE_MIN_DISTANCE) r = SAFE_MIN_DISTANCE;
            
            finalPos = direction * r;
            finalVel = direction * vr;
        }
        else
        {
            // Long time: velocity approaches asymptotic value
            float vInfinity = Mathf.Sqrt(2f * energy);
            float characteristicTime = r0 / Mathf.Max(Mathf.Abs(vInfinity), EPSILON);
            
            // Exponential approach to terminal velocity
            float v = vr0;
            float r = r0;
            
            // Fixed-point iteration (still O(1))
            for (int i = 0; i < 3; i++)
            {
                float avgV = (vr0 + v) * 0.5f;
                float avgR = r0 + avgV * time * 0.5f;
                if (avgR < SAFE_MIN_DISTANCE) avgR = SAFE_MIN_DISTANCE;
                
                float accel = -mu / (avgR * avgR);
                v = vr0 + accel * time;
                r = r0 + avgV * time;
            }
            
            // Ensure minimum distance
            if (r < SAFE_MIN_DISTANCE) r = SAFE_MIN_DISTANCE;
            
            finalPos = direction * r;
            finalVel = direction * v;
        }
    }

    // Bound radial motion (falls through center and back)
    private static void SolveRadialBound(
        float r0, float vr0, float mu, float time,
        Vector2 direction,
        out Vector2 finalPos, out Vector2 finalVel)
    {
        // For bound radial motion, use energy conservation with approximation
        
        // Calculate period of oscillation
        float energy = vr0 * vr0 * 0.5f - mu / r0;
        float a = -mu / (2f * energy); // Semi-major axis (positive)
        float period = 2f * Mathf.PI * Mathf.Sqrt(a * a * a / mu);
        
        // Wrap time to within one period
        float tWrapped = time % period;
        
        // Use polynomial approximation for small angles
        if (tWrapped < period * 0.25f)
        {
            // Small angle approximation
            float omega = Mathf.Sqrt(mu / (a * a * a));
            float angle = omega * tWrapped;
            
            // Use series expansion for small angles
            float cosTerm = 1f - angle * angle * 0.5f + angle * angle * angle * angle / 24f;
            float sinTerm = angle - angle * angle * angle / 6f;
            
            float r = a * (1f - cosTerm);
            float vr = Mathf.Sqrt(mu / a) * sinTerm;
            
            // Ensure minimum distance
            if (r < SAFE_MIN_DISTANCE) r = SAFE_MIN_DISTANCE;
            
            finalPos = direction * r;
            finalVel = direction * vr;
        }
        else
        {
            // Use numerical approximation with fixed steps (still O(1))
            const int steps = 8;
            float dt = tWrapped / steps;
            float r = r0;
            float vr = vr0;
            
            for (int i = 0; i < steps; i++)
            {
                float accel = -mu / (r * r);
                vr += accel * dt;
                r += vr * dt;
                
                // Simple bounce at center with energy loss
                if (r < SAFE_MIN_DISTANCE)
                {
                    r = SAFE_MIN_DISTANCE;
                    vr = -Mathf.Abs(vr) * 0.5f; // Damped bounce
                }
            }
            
            finalPos = direction * r;
            finalVel = direction * vr;
        }
    }

    // Elliptical orbit solver
    private static void SolveEllipticalOrbit(
        Vector2 r0, Vector2 v0, float mu, float h, float energy,
        float time, out Vector2 finalPos, out Vector2 finalVel)
    {
        // Ensure h is not too small
        float hSafe = Mathf.Max(Mathf.Abs(h), EPSILON);
        
        // Orbital parameters
        float a = -mu / (2f * energy); // Semi-major axis (positive)
        float eSquared = 1f - (hSafe * hSafe) / (mu * a);
        float e = Mathf.Sqrt(Mathf.Max(0f, eSquared)); // Eccentricity
        
        // Clamp eccentricity to avoid edge cases
        e = Mathf.Clamp(e, 0f, 0.9999f);
        
        float r0Mag = r0.magnitude;
        
        // Calculate initial true anomaly
        float cosNu0 = (hSafe * hSafe / (mu * r0Mag) - 1f) / e;
        cosNu0 = Mathf.Clamp(cosNu0, -1f, 1f);
        float nu0 = Mathf.Acos(cosNu0);
        
        // Determine correct quadrant based on radial velocity
        if (Vector2.Dot(r0, v0) < 0)
        {
            nu0 = 2f * Mathf.PI - nu0;
        }
        
        // Calculate initial eccentric anomaly
        float E0;
        if (e < 0.8f)
        {
            // Use standard formula for moderate eccentricity
            float tanHalfNu0 = Mathf.Tan(nu0 * 0.5f);
            float sqrtFactor = Mathf.Sqrt((1f + e) / (1f - e));
            E0 = 2f * Mathf.Atan2(tanHalfNu0, sqrtFactor);
        }
        else
        {
            // For high eccentricity, use alternative calculation
            E0 = Mathf.Acos((Mathf.Cos(nu0) + e) / (1f + e * Mathf.Cos(nu0)));
            if (nu0 > Mathf.PI) E0 = 2f * Mathf.PI - E0;
        }
        
        // Wrap E0 to [0, 2π)
        E0 = ModTwoPi(E0);
        
        // Initial mean anomaly
        float M0 = E0 - e * Mathf.Sin(E0);
        
        // Mean motion and propagated mean anomaly
        float n = Mathf.Sqrt(mu / (a * a * a));
        float M = M0 + n * time;
        
        // Wrap mean anomaly to [0, 2π)
        M = ModTwoPi(M);
        
        // Solve Kepler's equation: M = E - e*sin(E)
        float E = SolveKeplerEquationElliptical(M, e);
        
        // Final true anomaly
        float nu;
        if (e < 0.8f)
        {
            float tanHalfE = Mathf.Tan(E * 0.5f);
            float sqrtFactor = Mathf.Sqrt((1f + e) / (1f - e));
            nu = 2f * Mathf.Atan(tanHalfE * sqrtFactor);
        }
        else
        {
            nu = Mathf.Acos((Mathf.Cos(E) - e) / (1f - e * Mathf.Cos(E)));
            if (E > Mathf.PI) nu = 2f * Mathf.PI - nu;
        }
        
        // Wrap true anomaly to [0, 2π)
        nu = ModTwoPi(nu);
        
        // Distance
        float r = a * (1f - e * Mathf.Cos(E));
        
        // Position in orbital plane (x toward periapsis)
        Vector2 posOrbital = new Vector2(r * Mathf.Cos(nu), r * Mathf.Sin(nu));
        
        // Velocity in orbital plane
        float sqrtMuA = Mathf.Sqrt(mu * a);
        float sinNu = Mathf.Sin(nu);
        float cosNu = Mathf.Cos(nu);
        Vector2 velOrbital = new Vector2(
            -sqrtMuA * sinNu / r,
            sqrtMuA * (e + cosNu) / r
        );
        
        // Transform to original frame
        // Calculate the rotation from orbital plane to original frame
        float rotationAngle = CalculateOrbitRotation(r0, v0, nu0, nu, posOrbital);
        
        finalPos = RotateVector(posOrbital, rotationAngle);
        finalVel = RotateVector(velOrbital, rotationAngle);
    }

    // Parabolic orbit approximation (avoiding transcendental equations)
    private static void SolveParabolicOrbit(
        Vector2 r0, Vector2 v0, float mu, float h, float time,
        out Vector2 finalPos, out Vector2 finalVel)
    {
        // For parabolic orbits, use energy approximation
        // Parabolic orbits are rare, so a good approximation is sufficient
        
        float r0Mag = r0.magnitude;
        
        // Use a refined constant acceleration approximation
        Vector2 a0 = CalculateGravityAcceleration(r0, mu / G);
        
        // First prediction
        Vector2 pos1 = r0 + v0 * time + 0.5f * a0 * time * time;
        Vector2 a1 = CalculateGravityAcceleration(pos1, mu / G);
        
        // Second prediction with better average
        Vector2 aAvg = (a0 + a1) * 0.5f;
        Vector2 pos2 = r0 + v0 * time + 0.5f * aAvg * time * time;
        Vector2 a2 = CalculateGravityAcceleration(pos2, mu / G);
        
        // Weighted average for final calculation
        Vector2 aFinal = (a0 + 2f * a1 + a2) * 0.25f;
        finalPos = r0 + v0 * time + 0.5f * aFinal * time * time;
        finalVel = v0 + aFinal * time;
        
        // Ensure we don't get too close to center
        if (finalPos.magnitude < SAFE_MIN_DISTANCE)
        {
            finalPos = finalPos.normalized * SAFE_MIN_DISTANCE;
            // Adjust velocity to conserve angular momentum approximately
            float rNew = finalPos.magnitude;
            float speedScale = Mathf.Sqrt(r0Mag / rNew);
            finalVel = finalVel.normalized * finalVel.magnitude * speedScale;
        }
    }

    // Hyperbolic orbit approximation (avoiding sinh/cosh)
    private static void SolveHyperbolicOrbitApprox(
        Vector2 r0, Vector2 v0, float mu, float h, float energy,
        float time, out Vector2 finalPos, out Vector2 finalVel)
    {
        // For hyperbolic orbits, we can use an energy-based approximation
        // without needing hyperbolic trig functions
        
        float r0Mag = r0.magnitude;
        float v0Mag = v0.magnitude;
        
        // Calculate asymptotic velocity
        float vInfinity = Mathf.Sqrt(2f * energy);
        
        // Estimate how much the object will move
        if (time < 1f || r0Mag > 100f)
        {
            // For short times or large distances, use polynomial approximation
            
            // Calculate initial acceleration
            Vector2 a0 = CalculateGravityAcceleration(r0, mu / G);
            
            // First-order prediction
            Vector2 pos1 = r0 + v0 * time + 0.5f * a0 * time * time;
            float r1 = pos1.magnitude;
            
            // Estimate acceleration at new position
            float a1Mag = mu / (r1 * r1);
            Vector2 a1 = -pos1.normalized * a1Mag;
            
            // Average acceleration
            Vector2 aAvg = (a0 + a1) * 0.5f;
            
            finalPos = r0 + v0 * time + 0.5f * aAvg * time * time;
            finalVel = v0 + aAvg * time;
        }
        else
        {
            // For longer times, use an asymptotic approximation
            
            // The object will approach vInfinity
            // Use a simple model: v(t) = vInfinity * tanh(k*t) adjusted
            
            // Estimate characteristic time to reach near-terminal velocity
            float k = vInfinity / Mathf.Max(r0Mag, 1f);
            
            // Approximate tanh with rational function (avoids sinh/cosh)
            float kt = k * time;
            float tanhApprox = kt / (1f + Mathf.Abs(kt));
            
            float vMag = v0Mag + (vInfinity - v0Mag) * tanhApprox;
            
            // Direction will change gradually
            Vector2 initialDir = v0.normalized;
            Vector2 radialDir = -r0.normalized;
            
            // The velocity direction will rotate toward radial as it escapes
            // Simple linear interpolation for direction change
            float blend = Mathf.Clamp01(time * 0.1f);
            Vector2 vDir = Vector2.Lerp(initialDir, radialDir, blend).normalized;
            
            finalVel = vDir * vMag;
            
            // Estimate position (integral of velocity)
            // Simple trapezoidal rule
            Vector2 vAvg = (v0 + finalVel) * 0.5f;
            finalPos = r0 + vAvg * time;
        }
        
        // Ensure minimum distance
        if (finalPos.magnitude < SAFE_MIN_DISTANCE)
        {
            finalPos = finalPos.normalized * SAFE_MIN_DISTANCE;
        }
    }

    // Overload with planet position
    public static void ComputePlanetGravityMotion(
        Vector2 initialPosition,
        Vector2 initialVelocity,
        float totalTime,
        float planetMass,
        Vector2 planetPosition,
        out Vector2 finalPosition,
        out Vector2 finalVelocity)
    {
        // Transform to planet-centered coordinates
        Vector2 relativePos = initialPosition - planetPosition;
        
        ComputePlanetGravityMotion(relativePos, initialVelocity, totalTime, planetMass,
            out Vector2 finalRelativePos, out Vector2 finalRelativeVel);
        
        // Transform back to world coordinates
        finalPosition = finalRelativePos + planetPosition;
        finalVelocity = finalRelativeVel;
    }

    // ==================== HELPER METHODS ====================

    // Gravity acceleration calculation
    private static Vector2 CalculateGravityAcceleration(Vector2 position, float mass)
    {
        float distance = position.magnitude;
        if (distance < EPSILON) return Vector2.zero;
        
        float acceleration = G * mass / (distance * distance);
        Vector2 direction = -position.normalized;
        return acceleration * direction;
    }

    // Solve Kepler's equation for elliptical orbits: M = E - e*sin(E)
    private static float SolveKeplerEquationElliptical(float M, float e)
    {
        // Initial guess - different strategies based on eccentricity
        float E;
        if (e < 0.8f)
        {
            E = M + e * Mathf.Sin(M) + 0.5f * e * e * Mathf.Sin(2f * M);
        }
        else
        {
            E = Mathf.PI; // Start at π for high eccentricity
        }
        
        // Fixed-point iteration (O(1) with fixed iterations)
        for (int i = 0; i < MAX_ITERATIONS; i++)
        {
            float sinE = Mathf.Sin(E);
            float f = E - e * sinE - M;
            
            // Check convergence
            if (Mathf.Abs(f) < EPSILON)
                break;
            
            // Update using derivative
            float fPrime = 1f - e * Mathf.Cos(E);
            
            // Avoid division by very small numbers
            if (Mathf.Abs(fPrime) < EPSILON)
                break;
                
            E = E - f / fPrime;
            
            // Keep E in reasonable range
            if (E < 0) E += 2f * Mathf.PI;
            if (E > 2f * Mathf.PI) E -= 2f * Mathf.PI;
        }
        
        return E;
    }

    // Calculate rotation from orbital plane to original frame
    private static float CalculateOrbitRotation(Vector2 r0, Vector2 v0, float nu0, float nu, Vector2 posOrbital)
    {
        // Simple approach: find the angle that rotates posOrbital to match r0's direction
        
        // In the orbital plane, when nu = nu0, posOrbital should align with r0
        // So the rotation angle = angle(r0) - angle(posOrbital_when_nu=nu0)
        
        // Calculate what posOrbital would be at nu0
        float rAtNu0 = posOrbital.magnitude; // Approximately same distance
        Vector2 posAtNu0 = new Vector2(rAtNu0 * Mathf.Cos(nu0), rAtNu0 * Mathf.Sin(nu0));
        
        // Avoid zero vector
        if (posAtNu0.magnitude < EPSILON || r0.magnitude < EPSILON)
            return 0f;
            
        float angleR0 = Mathf.Atan2(r0.y, r0.x);
        float anglePosAtNu0 = Mathf.Atan2(posAtNu0.y, posAtNu0.x);
        
        return angleR0 - anglePosAtNu0;
    }

    // Rotate a vector by an angle
    private static Vector2 RotateVector(Vector2 v, float angle)
    {
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);
        return new Vector2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos
        );
    }

    // Modulo 2π
    private static float ModTwoPi(float angle)
    {
        angle = angle % (2f * Mathf.PI);
        if (angle < 0) angle += 2f * Mathf.PI;
        return angle;
    }

    public static float SolveKeplerForE(float M, float e)
    {
        float tolerance = 1e-6f;
        int maxIterations = 20;

        // Wrap M to [0, 2π)
        M = M % (2f * Mathf.PI);
        if (M < 0) M += 2f * Mathf.PI;
        
        // Initial guess (depends on eccentricity)
        float E;
        if (e < 0.8f)
            E = M;  // Good guess for low/moderate e
        else
            E = M + 0.85f * e * Mathf.Sign(Mathf.Sin(M));  // Better for high e
        
        // Newton-Raphson iteration
        for (int i = 0; i < maxIterations; i++)
        {
            float sinE = Mathf.Sin(E);
            float cosE = Mathf.Cos(E);
            
            // f(E) = E - e*sinE - M
            float f = E - e * sinE - M;
            
            // Check convergence
            if (Mathf.Abs(f) < tolerance)
                return E;
            
            // f'(E) = 1 - e*cosE
            float fPrime = 1f - e * cosE;
            
            // Avoid division by zero (rare but possible)
            if (Mathf.Abs(fPrime) < 1e-12f)
                break;
            
            // Newton update
            float delta = f / fPrime;
            E -= delta;
        }
        
        // If Newton didn't converge, fallback to simple method
        // This works because Kepler's equation is monotonic
        float lower = 0f;
        float upper = 2f * Mathf.PI;
        
        for (int i = 0; i < 50; i++)
        {
            E = (lower + upper) / 2f;
            float f = E - e * Mathf.Sin(E) - M;
            
            if (Mathf.Abs(f) < tolerance)
                return E;
            
            if (f > 0)
                upper = E;
            else
                lower = E;
        }
        
        return E;
    }

    public static float EccentricAnomalyToTrueAnomaly(float E, float e)
    {
        // Direct formula (no quadrant issues)
        float cosE = Mathf.Cos(E);
        float sinE = Mathf.Sin(E);
        
        float cosNu = (cosE - e) / (1f - e * cosE);
        float sinNu = (Mathf.Sqrt(1f - e * e) * sinE) / (1f - e * cosE);
        
        float nu = Mathf.Atan2(sinNu, cosNu);
        
        // Convert to [0, 2π)
        if (nu < 0) nu += 2f * Mathf.PI;
        
        return nu;
    }

    public static float TrueAnomalyToEccentricAnomaly(float nu, float e)
    {
        // Direct formula: cos E = (e + cos ν)/(1 + e cos ν)
        float cosNu = Mathf.Cos(nu);
        float sinNu = Mathf.Sin(nu);
        
        float cosE = (e + cosNu) / (1f + e * cosNu);
        float sinE = (Mathf.Sqrt(1f - e * e) * sinNu) / (1f + e * cosNu);
        
        float E = Mathf.Atan2(sinE, cosE);
        
        // Convert to [0, 2π)
        if (E < 0) E += 2f * Mathf.PI;
        
        return E;
    }

    public static float Atanh(float x)
    {
        // atanh(x) = 0.5 * ln((1 + x) / (1 - x))
        // Valid for |x| < 1
        
        return 0.5f * Mathf.Log((1f + x) / (1f - x));
    }
}