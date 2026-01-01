using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapDisplay : MonoBehaviour
{
    public GameObject gb;
    public RawImage spriteRenderer;
    public Texture2D texture;


    public GameObject shipDot;
    public GameObject satellitePrefab;
    public GameObject center;

    float mapWidth = 225f;

    private GameObject ship;
    private float zoom = 1f;
    List<GameObject> satellites;

    public static float Atanh(float x)
    {
        // atanh(x) = 0.5 * ln((1 + x) / (1 - x))
        // Valid for |x| < 1
        
        return 0.5f * Mathf.Log((1f + x) / (1f - x));
    }

    float MeanToTime(float M0, float Mf, bool prograde, float n)
    {
        float dM = prograde ? (Mf - M0) : (M0 - Mf);
        //dM = NormalizeAngle(dM);
        return dM / n; // seconds
    }

    public float GetAreaFraction(float theta1, float theta2, float a, float b, float e)
    {
        // Ensure angles are in [0, 2π)
        theta1 = Mathf.Repeat(theta1, 2f * Mathf.PI);
        theta2 = Mathf.Repeat(theta2, 2f * Mathf.PI);

        
        // Ensure theta2 > theta1 for positive area
        if (theta2 < theta1)
        {
            //theta2 += 2f * Mathf.PI;
            theta1 -= 2f*Mathf.PI;
        }
        
        // Convert true anomalies to eccentric anomalies
        float E1 = TrueAnomalyToEccentricAnomaly(theta1, e);
        float E2 = TrueAnomalyToEccentricAnomaly(theta2, e);
        
        // Calculate area swept using Kepler's equation
        float sweptArea = a * b * 0.5f * ((E2 - e * Mathf.Sin(E2)) - (E1 - e * Mathf.Sin(E1)));
        
        // Total area of ellipse
        float totalArea = Mathf.PI * a * b;
        
        return sweptArea / totalArea;
    }

    private float TrueAnomalyToEccentricAnomaly(float theta, float e)
    {
        // For elliptical orbits (e < 1)
        if (e < 1f)
        {
            // Use atan2 for correct quadrant
            float E = 2f * Mathf.Atan2(
                Mathf.Sqrt(1f - e) * Mathf.Sin(theta / 2f),
                Mathf.Sqrt(1f + e) * Mathf.Cos(theta / 2f)
            );
            return E;
        }
        else
        {
            // Hyperbolic case (e > 1) - different formula
            float H = 2f * Atanh(Mathf.Sqrt((e - 1f) / (e + 1f)) * Mathf.Tan(theta / 2f));
            return H;
        }
    }

    public void ZoomIn(){
        zoom *= 1.1f;
    }

    public void ZoomOut(){
        if(zoom > 1f){
            zoom /= 1.1f;

        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        satellites = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFocus(GameObject focus, GameObject ship){
        if(satellites == null){
            return;
        }
        Body focusScript = focus.GetComponent<Body>();
        SimpleShipScript shipScript = ship.GetComponent<SimpleShipScript>();

        this.ship = ship;

        float dist = Vector3.Distance(focus.transform.position, ship.transform.position);

        foreach(GameObject satellite in satellites){
            Destroy(satellite);
        }

        satellites = new List<GameObject>();
        
        int satelliteCount = focusScript.GetSatellites().Count;

        float radius = focusScript.GetSystemRadius();
        float neighborhoodRadius = focusScript.GetNeighborhoodRadius();
        if(dist > radius && neighborhoodRadius > radius){
            radius = neighborhoodRadius;
        }

        foreach(GameObject satellite in focusScript.GetSatellites()){
            Body satelliteScript = satellite.GetComponent<Body>();
            GameObject image = Instantiate(satellitePrefab);
            float realDistance = satelliteScript.GetDistance();
            float ratio = satelliteScript.GetDistance() / radius;
            float angle = satelliteScript.GetAngle();
            float guiDistance = ratio * mapWidth / 2;

            float x = guiDistance * Mathf.Cos(angle);
            float y = guiDistance * Mathf.Sin(angle);
            
            image.transform.SetParent(gb.transform);
            image.GetComponent<RectTransform>().localPosition = new Vector2(x, y);
            image.SetActive(true);

            satellites.Add(image);
        }

        foreach(NeighborEdge neighbor in focusScript.GetNeighbors()){
            GameObject image = Instantiate(satellitePrefab);
            float realDistance = neighbor.GetDistance();
            float ratio = neighbor.GetDistance() / radius;
            float angle = neighbor.GetAngle();
            float guiDistance = ratio * mapWidth / 2;

            float x = guiDistance * Mathf.Cos(angle);
            float y = guiDistance * Mathf.Sin(angle);
            
            image.transform.SetParent(gb.transform);
            image.GetComponent<RectTransform>().localPosition = new Vector2(x, y);
            image.SetActive(true);

            satellites.Add(image);
        }

        Vector2 position = new Vector2(shipScript.GetPosition().x, shipScript.GetPosition().z);
        position = position / radius;
        position = position * mapWidth / 2;  
      
        shipDot.GetComponent<RectTransform>().localPosition = position;
        shipDot.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, -ship.transform.eulerAngles.y);

        //Move the satellite objects to a further position based on the zoom level
        foreach(GameObject satellite in satellites){
            if(satellite == null){ continue; }
            Vector3 offset = satellite.GetComponent<RectTransform>().localPosition - shipDot.GetComponent<RectTransform>().localPosition;
            satellite.transform.GetComponent<RectTransform>().localPosition = shipDot.GetComponent<RectTransform>().localPosition + offset * zoom;
            satellite.SetActive(true);
        }

        //Move center
        Vector2 centerPosition = -1f * position * (zoom - 1.0f);
        center.GetComponent<RectTransform>().localPosition = centerPosition;

        for(int i = 0; i < 300; i++){
            for(int j = 0; j < 300; j++){
                texture.SetPixel(i, j, Color.black);
            }
        }

        if(false && radius==neighborhoodRadius){
            texture.Apply();
            return;
        }


        //Create graph
        float modelScale = Parameters.GetModelScale();
        radius *= modelScale;

        Vector2 r0_vec = new Vector2(shipScript.GetPosition().x, shipScript.GetPosition().z)*modelScale;
        Vector2 v0_vec = new Vector2(shipScript.GetVelocity().x, shipScript.GetVelocity().z)*modelScale;
        float x0 = r0_vec.x;
        float y0 = r0_vec.y;
        float vx0 = v0_vec.x;
        float vy0 = v0_vec.y;
        float rv_dot = Vector2.Dot(r0_vec,v0_vec);

        float G = 6.67f * Mathf.Pow(10f,-11f);
        float mass = focusScript.GetMass();
        float mu = G*mass;

        float r0 = r0_vec.magnitude;
        float v0 = v0_vec.magnitude;

        float h = x0*vy0-y0*vx0;

        Vector2 v_vec = v0_vec.normalized * v0;

        Vector2 e_vec = (1f / mu) * (
            (v0 * v0 - mu / r0) * r0_vec
            - rv_dot * v_vec
        );
        float e = e_vec.magnitude;

        float epsilon = v0*v0/2-mu/r0;

        float omega = Mathf.Atan2(e_vec[1],e_vec[0]);

        float p = h*h/mu;

        float a = -mu / (2f * epsilon);

        float b = a * Mathf.Sqrt(1f - e * e);

        float theta_world = Mathf.Atan2(r0_vec.y, r0_vec.x);
        float theta_orbit = theta_world - omega;

        float lowerBound;
        float upperBound;

        if(e > 1f){
            float bound = Mathf.Acos(-1/e);
            //Only count from -pi/2 to pi/2
            lowerBound = omega -1f * bound;
            //upperBound = omega + Mathf.PI/2;
            upperBound = omega + bound;
        }else{
            lowerBound = 0f;
            upperBound = 2f * Mathf.PI;
        }

        float centerX = centerPosition.x;
        float centerY = centerPosition.y;

        for(float theta = lowerBound; theta < upperBound; theta+=Mathf.PI/500){
            float r_world = p/(1+e*Mathf.Cos(theta-omega)) * zoom;
            float rx = r_world * Mathf.Cos(theta);
            float ry = r_world * Mathf.Sin(theta);
            float cartx = rx / radius*mapWidth/2;
            float carty = ry / radius*mapWidth/2;

            float graphx = (rx / radius*mapWidth/2 + mapWidth / 2f) + centerX;
            float graphy = (ry / radius*mapWidth/2 + mapWidth / 2f) + centerY;
            if(graphx <= mapWidth && graphy <= mapWidth && graphx >= 0 && graphy >= 0){
                texture.SetPixel((int)graphx, (int)graphy, Color.white);
            }
        }
        foreach(GameObject satellite in focusScript.GetSatellites())
        {
            //Using centerX and centerY plot a circle
            if(satellite == null){ continue; }
            Vector3 satellitePosition = satellite.transform.position;
            Vector3 focusPosition = focus.transform.position;
            Body satelliteScript = satellite.GetComponent<Body>();
            float R = satelliteScript.GetDistance() * modelScale;
            float distance = R/radius * mapWidth/2f*zoom;
            for(float theta = lowerBound; theta < 2*Mathf.PI; theta+=Mathf.PI/500){
                float rx = distance * Mathf.Cos(theta);
                float ry = distance * Mathf.Sin(theta);
                float graphx = (rx + mapWidth / 2f) + centerX;
                float graphy = (ry + mapWidth / 2f) + centerY;
                if(graphx <= mapWidth && graphy <= mapWidth && graphx >= 0 && graphy >= 0){
                    texture.SetPixel((int)graphx, (int)graphy, Color.white);
                }
            }
            //Check for intercept
            //r_rocket = p/(1+e*Mathf.Cos(theta-omega))
            //r_satellite = distance
            float C = (p / R - 1f) / e;

            if (false&&Mathf.Abs(C) > 1f)
            {
                //Debug.Log("No real intersections — orbit solution misses circle.");
            }
            else
            {
                float acosC = Mathf.Acos(C);
                float theta1 = omega + acosC;
                float theta2 = omega - acosC;
                if(theta1 < 0){
                    theta1 += 2*Mathf.PI;
                }
                if(theta2 < 0){
                    theta2 += 2*Mathf.PI;
                }

                float T = 2*Mathf.PI*Mathf.Sqrt(Mathf.Pow(a,3f)/(G*mass));
                

                float n = Mathf.Sqrt(mu / Mathf.Pow(a, 3f));
                float areaFraction = GetAreaFraction(theta_world-omega, theta1-omega, a, b, e);

                //float time = MeanToTime(theta_world, theta1, true, n);
                float time = areaFraction * T;
                float satelliteAngle = satelliteScript.GetAngle();
                float angularVelocity = satelliteScript.GetAngularVelocity();

                float finalSatAngle = satelliteAngle+angularVelocity*time;
                finalSatAngle = Mathf.Repeat(finalSatAngle, 2f * Mathf.PI);

                float angularDistance = Mathf.Abs(finalSatAngle-(theta1));
                if(angularDistance > Mathf.PI){
                    angularDistance = 2*Mathf.PI - angularDistance;
                }

                //Debug.Log(satelliteScript.GetName()+" "+theta1+" " +angularDistance);
            }
        }
        texture.Apply();
    }   

    public void RenderNeighborhood(GameObject focus, GameObject ship){
        return;
    }

    public void UpdateDisplay(){
        //Set ship dot to position relative to center
        //Set satellites relative to center
    }
}