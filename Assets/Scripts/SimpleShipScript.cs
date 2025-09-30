using UnityEngine;
using System.Collections.Generic;

public class SimpleShipScript : MonoBehaviour
{
    public GameObject gb;
    public GameObject focus;

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

    //3D kinematics variables. Stored in 1 unit = 1000 m
    private Vector3 position;
    private Vector3 velocity;
    private Vector3 acceleration;



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

    public void SetFocus(GameObject focus){
        this.focus = focus;
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
        if (Input.GetKey(KeyCode.W))
        {
            transform.Rotate(Vector3.right, -rotationSpeed * Time.deltaTime);
        }

        // Rotate right when pressing D
        if (Input.GetKey(KeyCode.S))
        {
            transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);            
        }
    }

    public void PhysicsClock(){
        float updateTime = Parameters.GetUpdateTime();
        float timeScale = Parameters.getTimeScale();
        acceleration = thrust * transform.forward.normalized;
        GravityVector strongestGravity = new GravityVector("None", new Vector3(0,0,0));

        Body focusScript = focus.GetComponent<Body>();
        acceleration += focusScript.GetGravity(gb).getVector();
        strongestGravity = focusScript.GetGravity(gb);
        
        GameObject strongestGravityBody = null;
        float closestSatelliteDistance = 0f;

        foreach(GameObject s in focusScript.GetSatellites()){
            Body script = s.GetComponent<Body>();
            GravityVector v = script.GetGravity(gb);
            acceleration += v.getVector();
            if(v.getVector().magnitude > strongestGravity.getVector().magnitude){
                strongestGravity = v;
                strongestGravityBody = s;
            }
            float distanceFromSatellite = Vector3.Distance(transform.position, s.transform.position);
            if(distanceFromSatellite < script.GetSystemRadius()){
                Vector3 offset = (transform.position - s.transform.position);
                position = offset;
                focus = s;
                break;
            }
        }

        /*if(strongestGravityBody != null && strongestGravityBody != focus){
            Vector3 offset = (transform.position - strongestGravityBody.transform.position);
            position = offset;
            focus = strongestGravityBody;
        }*/

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
        

        //Change focus if another body has stronger gravity than the current focus
        kinematicsDisplay.UpdateDisplay(acceleration.magnitude * 1000, velocity.magnitude * 1000);


        acceleration = acceleration * Mathf.Pow(timeScale, 1f);

        velocity += acceleration * updateTime;
        position += velocity * updateTime * timeScale;
    }
}
