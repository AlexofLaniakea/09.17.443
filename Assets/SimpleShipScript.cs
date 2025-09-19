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
    public float mass = 1f;
    private float timeScale = 1f; //All velocities and accelerations should be multiplied by timescale
    public float acceleration = 0f;

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

    public void setAcceleration(float acceleration){
        this.acceleration = acceleration;
    }

    public float getTimeScale(){
        return timeScale;
    }

    public float getAcceleration(){
        return acceleration;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cf = GetComponent<ConstantForce>();
        rb.sleepThreshold = 0.0f;
        //rb.linearVelocity = Vector3.forward * 10f; 
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
        Vector3 gravityVector = gravityMagnitude * gravityDirection;
        Vector3 accelerationVector = transform.forward.normalized * acceleration;
        //Vector3 netForce = (accelerationVector + gravityVector) * Mathf.Pow(10, -3) * timeScale;
        Vector3 netForce = accelerationVector;
        foreach(GravityVector v in gravityVectors){
            netForce += v.getVector();
        }
        //Debug.Log(gravityVectors.Count);
        cf.force = netForce * Mathf.Pow(10, -3) * timeScale;
    }

}
