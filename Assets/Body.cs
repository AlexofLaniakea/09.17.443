using UnityEngine;
using System.Collections.Generic;


public class Body : MonoBehaviour
{
    private GameObject gb;
    private string name;
    private float mass; //(float)5.972 * Mathf.Pow(10, 24);
    private GameObject shipsListObject;
    private List<GameObject> ships;
    private float G = (float)6.673 * Mathf.Pow(10, -11);
    private float angularVelocity;
    private float velocity = 0f;
    private List<GameObject> satellites;

    public void Initialize(string name, float size, float mass, Vector3 position){
        this.name = name;
        gameObject.name = name;
        this.mass = mass;
        transform.localScale = Vector3.one * size;
        transform.position = position;
        gameObject.SetActive(true);

        // load material from Resources/PlanetTextures/[name]
        Material mat = Resources.Load<Material>("PlanetTextures/" + name);
        if (mat != null)
        {
            // assign to MeshRenderer
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = mat;
            }
        }

        ships = new List<GameObject>();
        satellites = new List<GameObject>();


        shipsListObject = GameObject.Find("Ships");
        foreach (Transform child in shipsListObject.transform)
        {
            ships.Add(child.gameObject);
        }
    }

    public void SetAngularVelocity(float angularVelocity){
        this.angularVelocity = angularVelocity;
    }

    public void SetVelocity(float velocity){
        this.velocity = velocity;
    }

    public void AddSatellite(GameObject satellite){
        satellites.Add(satellite);
    }

    public float GetAngularVelocity(){
        return angularVelocity;
    }

    public float GetVelocity(){
        return velocity;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Upate is called once per frame
    void Update()
    {
        //Find the distance between body and all nearby ships and add force?
    }

    void FixedUpdate()
    {
        if(!gameObject.activeSelf){
            return;
        }

        float timeScale = 1f;
        

        foreach(GameObject ship in ships){
            float distance = Vector3.Distance(transform.position, ship.transform.position) * 1000;
            float acceleration =  G * mass / Mathf.Pow(distance, 2);
            Vector3 offset = transform.position - ship.transform.position;
            offset = offset.normalized;
            SimpleShipScript simpleShipScript = ship.GetComponent<SimpleShipScript>();
            simpleShipScript.SetGravityForce(acceleration, offset);
            simpleShipScript.AddGravity(name, acceleration * offset);
            timeScale = simpleShipScript.getTimeScale();
        }

        //Move satellites
        foreach(GameObject satellite in satellites){
            float distance = Vector3.Distance(transform.position, satellite.transform.position);
            //Debug.Log(distance);
            Vector3 offset = transform.position - satellite.transform.position;
            offset = offset.normalized;
            Vector3 tangent = Vector3.Cross(Vector3.up, offset).normalized;
            Body satelliteScript = satellite.GetComponent<Body>();
            float velocity = satelliteScript.GetVelocity();
            //satellite.GetComponent<Rigidbody>().linearVelocity = tangent * velocity * timeScale;
            satellite.GetComponent<Rigidbody>().transform.RotateAround(transform.position, Vector3.up, 2.7f * Mathf.Pow(10f, -6f) * timeScale);
        }
    }

    public void OnClockTick(){
        float timeScale = Parameters.getTimeScale();
        //
    }
}
