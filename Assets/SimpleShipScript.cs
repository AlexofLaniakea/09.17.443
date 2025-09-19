using UnityEngine;

public class SimpleShipScript : MonoBehaviour
{
    private Rigidbody rb;
    private ConstantForce cf;
    public float rotationSpeed = 100f;
    public float gravityMagnitude;
    public Vector3 gravityDirection;
    public float mass = 1f;
    public float timeScale = 1000f; //All velocities and accelerations should be set to timescale
    public float acceleration = 1f;

    public void SetGravityForce(float gravityAcceleration, Vector3 vector){
        gravityMagnitude = gravityAcceleration * mass;
        gravityDirection = vector;
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
        Vector3 thrustVector = transform.forward.normalized;
        Vector3 gravityVector = gravityMagnitude * gravityDirection * Mathf.Pow(10, -3) * timeScale;
        Vector3 accelerationVector = transform.forward.normalized * acceleration * Mathf.Pow(10, -3) * timeScale;
        Vector3 netForce = accelerationVector + gravityVector;
        cf.force = netForce;
    }

}
