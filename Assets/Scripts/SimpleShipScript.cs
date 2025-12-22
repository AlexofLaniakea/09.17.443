using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleShipScript : MonoBehaviour
{
    public GameObject gb;
    public GameObject main;
    public GameObject map;
    public GameObject accelerator;
    public GameObject camera;

    protected MainScript mainScript;

    private GameObject focus;

    private Rigidbody rb;
    private ConstantForce cf;
    public StateIndicatorScript kinematicsDisplay;
    private float rotationSpeed = 100f;
    private float gravityMagnitude;
    private Vector3 gravityDirection;
    private List<GravityVector> gravityVectors = new List<GravityVector>();
    private float timer = 0;
    private bool canSwitchFocus = true;

    
    private float timeScale = 1f; //All velocities and accelerations should be multiplied by timescale
    
    //Physics scalars
    protected float thrust= 0f;
    private float right = 0;
    private float forward = 0;

    //3D kinematics variables. Stored in 1 unit = 1000 m
    private Vector3 position;
    private Vector3 velocity;
    private Vector3 acceleration;

    //Control parameters
    private bool isFreeCam = false;

    public void AddGravity(string name, Vector3 vector){
        foreach(GravityVector v in gravityVectors){
            if (v.getName().Equals(name)){
                v.setVector(vector);
                return;
            }
        }
        gravityVectors.Add(new GravityVector(name, vector));
    }

    public void SetGravityVector(Vector3 vector){

    }

    public virtual void SetThrust(float thrust){
        this.thrust = thrust/Parameters.GetModelScale()*10f;
    }

    public void SetPosition(Vector3 position){
        this.position = position;
    }

    public void SetVelocity(Vector3 velocity){
        this.velocity = velocity;
    }

    public void SetFocus(GameObject focus){
        this.focus = focus;
    }

    public void SetFreeCam(bool boolean)
    {
        isFreeCam = boolean;
    }

    public float getTimeScale(){
        return timeScale;
    }

    public float GetThrust(){
        return thrust;
    }

    public Vector3 GetPosition(){
        return position;
    }

    public Vector3 GetVelocity(){
        return velocity;
    }

    public GameObject GetFocus(){
        return focus;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainScript = main.GetComponent<MainScript>();
        StartCoroutine(ClockOneSecond());
        //position = new Vector3(150000000f, 0f, 30000f);
    }

    // Update is called once per frame
    void Update()
    {
        camera.SetActive(true);
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }

        // Rotate right when pressing D
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);            
        }

        SetThrust(accelerator.GetComponent<AcceleratorScript>().GetValue());
        /*if (Input.GetKey(KeyCode.W))
        {
            transform.Rotate(Vector3.right, -rotationSpeed * Time.deltaTime);
        }

        // Rotate right when pressing D
        if (Input.GetKey(KeyCode.S))
        {
            transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);            
        }*/
        if (Input.GetKey(KeyCode.I)){//Move camera in
            camera.transform.position -= camera.transform.position.normalized;

            //set angle to offset
            Vector3 offset = camera.transform.position-gb.transform.position;
            offset=offset.normalized*-1;
            camera.transform.rotation = Quaternion.LookRotation(offset);
        }
        if (Input.GetKey(KeyCode.O)){//Move camera out
            camera.transform.position += camera.transform.position.normalized;

            //set angle to offset
            Vector3 offset = camera.transform.position-gb.transform.position;
            offset=offset.normalized*-1;
            camera.transform.rotation = Quaternion.LookRotation(offset);
        }

        if(Input.GetKey(KeyCode.LeftArrow)){
            float x = camera.transform.position.x;
            float y = camera.transform.position.z;
            float z = camera.transform.position.y;
            float theta = 0.01f;

            float xNew = x*Mathf.Cos(theta)-y*Mathf.Sin(theta);
            float yNew = x*Mathf.Sin(theta)+y*Mathf.Cos(theta);
            camera.transform.position=new Vector3(xNew,z,yNew);

            //set angle to offset
            Vector3 offset = camera.transform.position-gb.transform.position;
            offset=offset.normalized*-1;
            camera.transform.rotation = Quaternion.LookRotation(offset);
        }
        if(Input.GetKey(KeyCode.RightArrow)){
            float x = camera.transform.position.x;
            float y = camera.transform.position.z;
            float z = camera.transform.position.y;
            float theta = -0.01f;
            
            float xNew = x*Mathf.Cos(theta)-y*Mathf.Sin(theta);
            float yNew = x*Mathf.Sin(theta)+y*Mathf.Cos(theta);
            camera.transform.position=new Vector3(xNew,z,yNew);

            //set angle to offset
            Vector3 offset = camera.transform.position-gb.transform.position;
            offset=offset.normalized*-1;
            camera.transform.rotation = Quaternion.LookRotation(offset);
        }
        /*if(Input.GetKey(KeyCode.UpArrow)){
            float x = camera.transform.position.y;
            float y = camera.transform.position.z;
            float z = camera.transform.position.x;
            float theta = 0.01f;
            
            float xNew = x*Mathf.Cos(theta)-y*Mathf.Sin(theta);
            float yNew = x*Mathf.Sin(theta)+y*Mathf.Cos(theta);
            camera.transform.position=new Vector3(z,xNew,yNew);

            //set angle to offset
            Vector3 offset = camera.transform.position-gb.transform.position;
            offset=offset.normalized*-1;
            camera.transform.rotation = Quaternion.LookRotation(offset);
        }
        if(Input.GetKey(KeyCode.DownArrow)){
            float x = camera.transform.position.y;
            float y = camera.transform.position.z;
            float z = camera.transform.position.x;
            float theta = -0.01f;
            
            float xNew = x*Mathf.Cos(theta)-y*Mathf.Sin(theta);
            float yNew = x*Mathf.Sin(theta)+y*Mathf.Cos(theta);
            camera.transform.position=new Vector3(z,xNew,yNew);

            //set angle to offset
            Vector3 offset = camera.transform.position-gb.transform.position;
            offset=offset.normalized*-1;
            camera.transform.rotation = Quaternion.LookRotation(offset);
        }*/


        if(Input.GetKey(KeyCode.UpArrow)){ forward = 1f; }
        else if(Input.GetKey(KeyCode.DownArrow)){ forward = -1f; }
        else{ forward = 0f;}

        if(Input.GetKey(KeyCode.RightArrow)){ right = 1f; }
        else if(Input.GetKey(KeyCode.LeftArrow)){ right = -1f; }
        else{ right = 0f;}
        if(Input.GetKey(KeyCode.LeftShift))
        {
            right *= 10f;
            forward *= 10f;
        }

    }

    protected IEnumerator ClockOneSecond()
    {
        while(true)
        {
            while(State.GetState() != 1){
                gb.SetActive(false);
                yield return new WaitForSeconds(0.1f);
            }
            mainScript.Render(focus,position);

            Body focusScript = focus.GetComponent<Body>();

            float systemRadius = focusScript.GetSystemRadius();

            float modelScale = Parameters.GetModelScale();

            if(!isFreeCam)
            {
                float updateTime = Parameters.GetUpdateTime();
                float timeScale = Parameters.getTimeScale();

                acceleration = thrust * transform.forward.normalized;

                //Add gravities of satellites
                foreach(GameObject s in focusScript.GetSatellites()){
                    Body script = s.GetComponent<Body>();
                    GravityVector v = script.GetGravity(gb);
                    acceleration += v.getVector();
                }

                Vector2 r0_vec = new Vector2(position.x, position.z)*modelScale;
                Vector2 v0_vec = new Vector2(velocity.x, velocity.z)*modelScale;
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

                float theta_world = Mathf.Atan2(r0_vec.y, r0_vec.x);
                float theta_orbit = theta_world - omega;

                theta_orbit = Mathf.Repeat(theta_orbit, 2f * Mathf.PI);

                float n = Mathf.Sqrt(mu / Mathf.Pow(a, 3f));

                if(thrust > 0 || e > 0.99f){
                    acceleration += focusScript.GetGravity(gb).getVector();

                    acceleration = acceleration * Mathf.Pow(timeScale, 1f);
                    velocity += acceleration * updateTime;
                    position += velocity * updateTime * timeScale;
                }
                else{
                    float E0 = MathPlus.TrueAnomalyToEccentricAnomaly(theta_orbit, e);
                    float M0 = E0 - e * Mathf.Sin(E0);
                    float Mnew = M0 + n * timeScale * updateTime;
                    float Enew = MathPlus.SolveKeplerForE(Mnew, e);
                    float vnew = MathPlus.EccentricAnomalyToTrueAnomaly(Enew, e);
                    
                    float r = a * (1 - e*e) / (1 + e * Mathf.Cos(vnew));
                    
                    // *** ADD ROTATION BY omega HERE ***
                    
                    // Position in perifocal frame (periapsis on +X)
                    float x_perifocal = r * Mathf.Cos(vnew);
                    float z_perifocal = r * Mathf.Sin(vnew);
                    
                    // Velocity in perifocal frame
                    float vr = (h / p) * e * Mathf.Sin(vnew);
                    float vt = h / r;
                    float vx_perifocal = vr * Mathf.Cos(vnew) - vt * Mathf.Sin(vnew);
                    float vz_perifocal = vr * Mathf.Sin(vnew) + vt * Mathf.Cos(vnew);
                    
                    // Rotate by omega to inertial frame
                    float cos_omega = Mathf.Cos(omega);
                    float sin_omega = Mathf.Sin(omega);
                    
                    // Rotate position
                    float x_inertial = x_perifocal * cos_omega - z_perifocal * sin_omega;
                    float z_inertial = x_perifocal * sin_omega + z_perifocal * cos_omega;
                    
                    // Rotate velocity (same rotation)
                    float vx_inertial = vx_perifocal * cos_omega - vz_perifocal * sin_omega;
                    float vz_inertial = vx_perifocal * sin_omega + vz_perifocal * cos_omega;
                    
                    position = new Vector3(x_inertial, 0f, z_inertial) / modelScale;
                    velocity = new Vector3(vx_inertial, 0f, vz_inertial) / modelScale;
                }
                
                //Change focus if another body has stronger gravity than the current focus
                kinematicsDisplay.UpdateDisplay(acceleration.magnitude*modelScale/timeScale, velocity.magnitude*modelScale);
                    
            }
            else
            {
                acceleration = focusScript.GetGravity(gb).getVector();
                float closestDistance = Vector3.Distance(focus.transform.position,gb.transform.position);
                foreach(GameObject s in focusScript.GetSatellites()){
                    Body script = s.GetComponent<Body>();
                    GravityVector v = script.GetGravity(gb);
                    acceleration += v.getVector();
                    float satelliteDistance = Vector3.Distance(s.transform.position,gb.transform.position);
                    if(satelliteDistance < closestDistance){
                        closestDistance = satelliteDistance;
                    }
                }
                position += (transform.forward.normalized * forward) * closestDistance / 20f;
            }

            foreach(GameObject s in focusScript.GetSatellites()){
                Body script = s.GetComponent<Body>();
                float distanceFromSatellite = Vector3.Distance(transform.position, s.transform.position);
                if(distanceFromSatellite < script.GetSystemRadius()*(3f/4f) && canSwitchFocus){
                    Vector3 offset = (transform.position - s.transform.position);
                    position = offset;
                    focus = s;
                    StartCoroutine(FocusSwitchTimer());
                    //Velocity = velocty - planet velocity
                    focusScript=focus.GetComponent<Body>();
                    float angle = focusScript.GetAngle();
                    float angularVelocity = focusScript.GetAngularVelocity();
                    float radius = focusScript.GetDistance()*modelScale;
                    float bodySpeed = radius*angularVelocity;
                    Vector3 vDir = new Vector3(
                    -Mathf.Sin(angle),0,
                    Mathf.Cos(angle)
                    );
                    Vector3 bodyVelocity = bodySpeed*vDir;
                    bodyVelocity /= modelScale;
                    velocity -= bodyVelocity;
                    break;
                }
            }

            GameObject primary = focusScript.GetPrimary();
            if(primary != null){
                float distanceFromFocus = Vector3.Distance(transform.position, focus.transform.position);
                if(distanceFromFocus > focusScript.GetSystemRadius() && canSwitchFocus)
                {
                    Vector3 offset = (transform.position - primary.transform.position);
                    position = offset;
                    //Velocity = velocty + planet velocity
                    float angle = focusScript.GetAngle();
                    float angularVelocity = focusScript.GetAngularVelocity();
                    float radius = focusScript.GetDistance()*modelScale;
                    float bodySpeed = radius*angularVelocity;
                    Vector3 vDir = new Vector3(
                    -Mathf.Sin(angle),0,
                    Mathf.Cos(angle)
                    );
                    Vector3 bodyVelocity = bodySpeed*vDir;
                    bodyVelocity /= modelScale;
                    velocity += bodyVelocity;
                    focus = primary;
                    StartCoroutine(FocusSwitchTimer());
                }
            }

            map.GetComponent<MapDisplay>().SetFocus(focus, gb);
            //timer += Parameters.getTimeScale() * Parameters.GetUpdateTime();
            yield return new WaitForSeconds(Parameters.GetUpdateTime());
        }
        
    }

    public IEnumerator FocusSwitchTimer(){
       canSwitchFocus = true;
       yield return new WaitForSeconds(1);
       canSwitchFocus = true;
    }
}
