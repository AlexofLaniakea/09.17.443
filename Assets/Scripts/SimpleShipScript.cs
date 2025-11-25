using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleShipScript : MonoBehaviour
{
    public GameObject gb;
    public GameObject main;
    public GameObject focus;
    public GameObject map;

    private MainScript mainScript;

    private Rigidbody rb;
    private ConstantForce cf;
    public StateIndicatorScript kinematicsDisplay;
    private float rotationSpeed = 100f;
    private float gravityMagnitude;
    private Vector3 gravityDirection;
    private List<GravityVector> gravityVectors = new List<GravityVector>();
    
    
    private float timeScale = 1f; //All velocities and accelerations should be multiplied by timescale
    
    //Physics scalars
    public float thrust= 0f;
    public float right = 0;
    public float forward = 0;

    //3D kinematics variables. Stored in 1 unit = 1000 m
    private Vector3 position;
    private Vector3 velocity;
    private Vector3 acceleration;

    //Control parameters
    private bool isFreeCam = true;

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

    public void SetThrust(float thrust){
        this.thrust = thrust;
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
        rb = GetComponent<Rigidbody>();
        cf = GetComponent<ConstantForce>();
        rb.sleepThreshold = 0.0f;

        mainScript = main.GetComponent<MainScript>();
        StartCoroutine(ClockOneSecond());
        //position = new Vector3(150000000f, 0f, 30000f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }

        // Rotate right when pressing D
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);            
        }
        /*if (Input.GetKey(KeyCode.W))
        {
            transform.Rotate(Vector3.right, -rotationSpeed * Time.deltaTime);
        }

        // Rotate right when pressing D
        if (Input.GetKey(KeyCode.S))
        {
            transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);            
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

    IEnumerator ClockOneSecond()
    {
        while(true)
        {
            while(State.GetState() != 1){
                yield return new WaitForSeconds(0.1f);
            }
            mainScript.Render(focus,position);

            Body focusScript = focus.GetComponent<Body>();

            float systemRadius = focusScript.GetSystemRadius();

            if(!isFreeCam)
            {
                float updateTime = Parameters.GetUpdateTime();
                float timeScale = Parameters.getTimeScale();
                float modelScale = Parameters.GetModelScale();
                acceleration = thrust * transform.forward.normalized;
                acceleration += focusScript.GetGravity(gb).getVector();
                
                //Change focus if too far away
                foreach(GameObject s in focusScript.GetSatellites()){
                    Body script = s.GetComponent<Body>();
                    GravityVector v = script.GetGravity(gb);
                    acceleration += v.getVector();
                    float distanceFromSatellite = Vector3.Distance(transform.position, s.transform.position);
                    if(distanceFromSatellite < script.GetSystemRadius()){
                        Vector3 offset = (transform.position - s.transform.position);
                        position = offset;
                        focus = s;
                        break;
                    }
                }

                //Change focus if another body has stronger gravity than the current focus
                kinematicsDisplay.UpdateDisplay(acceleration.magnitude*modelScale, velocity.magnitude*modelScale);

                acceleration = acceleration * Mathf.Pow(timeScale, 1f);

                velocity += acceleration * updateTime;
                position += velocity * updateTime * timeScale;
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
                if(distanceFromSatellite < script.GetSystemRadius()){
                    Vector3 offset = (transform.position - s.transform.position);
                    position = offset;
                    focus = s;
                    break;
                }
            }

            GameObject primary = focusScript.GetPrimary();
            if(primary != null){
                float distanceFromFocus = Vector3.Distance(transform.position, focus.transform.position);
                if(distanceFromFocus > focusScript.GetSystemRadius())
                {
                    Vector3 offset = (transform.position - primary.transform.position);
                    position = offset;
                    focus = primary;
                }
            }

            map.GetComponent<MapDisplay>().SetFocus(focus, gb);

            yield return new WaitForSeconds(Parameters.GetUpdateTime());
        }
        
    }

    public void PhysicsClock(){
       
    }
}
