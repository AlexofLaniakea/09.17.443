using UnityEngine;

public class SimpleShip : MonoBehaviour
{
    private Rigidbody rb;
    private ConstantForce cf;
    public float rotationSpeed = 100f;
    public float gravityMagnitude;
    public Vector3 gravityVector;
    public float mass = 1;

    void SetGravityForce(float acceleration, Vector3 vector){
        gravityMagnitude = acceleration * mass;
        gravityVector = vector;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cf = GetComponent<ConstantForce>();
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
    }

    void FixedUpdate(){
        cf.force = transform.forward.normalized * 1f;

    }

}
