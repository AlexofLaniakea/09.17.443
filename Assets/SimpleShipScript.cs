using UnityEngine;
using System.Collections.Generic;

public class SimpleShipScript : MonoBehaviour
{
    private Rigidbody rb;
    private ConstantForce cf;
    public float rotationSpeed = 100f;
    public float gravityMagnitude;
    public Vector3 gravityDirection;
    public List<GravityVector> gravityVectors = new List<GravityVector>();
    
    
    private float timeScale = 1f; //All velocities and accelerations should be multiplied by timescale
    
    public float accelerationMagnitude = 0f;
    public Vector3 accelerationDirection;

    //Physics scalars
    public float mass = 1f;
    public float thrust= 0f;

    //3D kinematics variables
    private Vector3 position;
    private Vector3 velocity;
    private Vector3 acceleration;
    

    //Polar coordinate
    private float distance;
    private float azimuth;
    private float elevation;


    public void SetGravityForce(float gravityAcceleration, Vector3 vector){
        gravityMagnitude = gravityAcceleration * mass;
        gravityDirection = vector;
    }

    public void setTimeScale(float timeScale){
        rb.linearVelocity = rb.linearVelocity * (timeScale/this.timeScale);
        this.timeScale = timeScale;
    }

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
        position = new Vector3(13000f, 0f, 0f);
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

    void FixedUpdate(){
        /*Vector3 gravityVector = gravityMagnitude * gravityDirection;
        Vector3 accelerationVector = transform.forward.normalized * acceleration;
        //Vector3 netForce = (accelerationVector + gravityVector) * Mathf.Pow(10, -3) * timeScale;
        Vector3 netForce = accelerationVector;
        foreach(GravityVector v in gravityVectors){
            netForce += v.getVector();
        }
        //Debug.Log(gravityVectors.Count);
        cf.force = netForce * Mathf.Pow(10, -3) * Mathf.Pow(timeScale, 2);*/
    }


    public void PhysicsClock(){
        acceleration = thrust * transform.forward.normalized;
       /* foreach(GravityVector v in gravityVectors){
            acceleration += v.getVector();
            Debug.Log(v.getName() + " " + acceleration);
        }*/

        velocity += acceleration;
        position += velocity;
    }
}
