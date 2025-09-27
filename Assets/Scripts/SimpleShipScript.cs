using UnityEngine;
using System.Collections.Generic;

public class SimpleShipScript : MonoBehaviour
{
    private Rigidbody rb;
    private ConstantForce cf;
    public StateIndicatorScript kinematicsDisplay;
    public float rotationSpeed = 100f;
    public float gravityMagnitude;
    public Vector3 gravityDirection;
    public List<GravityVector> gravityVectors = new List<GravityVector>();
    
    
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

    public float getTimeScale(){
        return timeScale;
    }

    public float GetThrust(){
        return thrust;
    }

    public Vector3 GetPosition(){
        return position;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cf = GetComponent<ConstantForce>();
        rb.sleepThreshold = 0.0f;
        position = new Vector3(15000f, 0f, 0f);
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
        foreach(GravityVector v in gravityVectors){
            acceleration += v.getVector();
            if(v.getVector().magnitude > strongestGravity.getVector().magnitude){
                strongestGravity = v;
            }
            //Debug.Log(v.getName() + " " + v.getVector().x);
        }

        //Change focus if another body has stronger gravity than the current focus
        kinematicsDisplay.UpdateDisplay(acceleration.magnitude, velocity.magnitude, strongestGravity.getName());


        acceleration = acceleration * Mathf.Pow(timeScale, 2);

        velocity += acceleration * updateTime;
        position += velocity * updateTime * timeScale;
    }
}
