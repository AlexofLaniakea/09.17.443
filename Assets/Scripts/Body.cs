using UnityEngine;
using System.Collections.Generic;


public class Body : MonoBehaviour
{
    private GameObject gb;
    private GameObject skyPoint;

    private string name;
    private float mass;
    private GameObject shipsListObject;
    private List<GameObject> ships;
    private float G = (float)6.673 * Mathf.Pow(10, -11);
    private float velocity = 0f;
    private float diameter;
    private float systemRadius;
    private float neighborhoodRadius = 0f;

    private List<GameObject> satellites;
    private GameObject primary;
    private List<NeighborEdge> neighbors;

    private float distance;
    private float angle;
    private float angularVelocity;

    

    public void Initialize(string name, float size, float mass, float distance, float angle, float angularVelocity,
    string texture){
        this.name = name;
        gameObject.name = name;
        this.mass = mass;
        transform.localScale = Vector3.one * size;
        this.diameter = size;
        systemRadius = diameter * 20f;

        this.distance = distance;
        this.angle = angle;
        this.angularVelocity = angularVelocity;

        skyPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        skyPoint.name = name + " Sky Point";
        //skyPoint.transform.SetParent(transform);
        texture = texture.Replace("\r", "").Replace("\n", "");
        // load material from Resources/PlanetTextures/[name]
        Material mat = Resources.Load<Material>("PlanetTextures/" + texture);
        if (mat != null)
        {
            // assign to MeshRenderer
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            MeshRenderer skyPointRenderer = skyPoint.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = mat;
                skyPointRenderer.material = mat;
            }
        }

        satellites = new List<GameObject>();
        neighbors = new List<NeighborEdge>();

        Destroy(skyPoint.GetComponent<Collider>());        
        //skyPoint.transform.localScale *= 10f;

        gb = this.gameObject;
    }

    public void SetAngularVelocity(float angularVelocity){
        this.angularVelocity = angularVelocity;
    }

    public void SetVelocity(float velocity){
        this.velocity = velocity;
    }

    public void SetAngle(float angle){
        this.angle = angle;
    }

    public void AddSatellite(GameObject satellite){
        satellites.Add(satellite);
        Body satelliteScript = satellite.GetComponent<Body>();
        
        satellite.GetComponent<Body>().SetPrimary(gb);
        if(satelliteScript.GetDistance() * 1.25f > systemRadius){
            systemRadius = satelliteScript.GetDistance() * 1.25f;
        }
    }

    public void AddNeighbor(NeighborEdge neighbor){
        neighbors.Add(neighbor);
        if(neighbor.GetDistance() * 1.25f > neighborhoodRadius){
            neighborhoodRadius = neighbor.GetDistance() * 1.25f;
        }
    }

    public void SetPrimary(GameObject primary){
        this.primary = primary;
        Body primaryScript = primary.GetComponent<Body>();
        float primaryMass = primaryScript.GetMass();
        systemRadius = distance*Mathf.Pow(mass/(3*primaryMass),1/3f);
    }

    public string GetName(){
        return name;
    }

    public float GetAngularVelocity(){
        return angularVelocity;
    }

    public float GetVelocity(){
        return velocity;
    }

    public float GetAngle(){
        return angle;
    }

    public float GetDistance(){
        return distance;
    }

    public List<GameObject> GetSatellites(){
        return satellites;
    }

    public List<NeighborEdge> GetNeighbors(){ return neighbors; }

    public GameObject GetPrimary(){
        return primary;
    }

    public float GetMass(){
        return mass;
    }

    public float GetDiameter(){ return diameter;}

    public float GetSystemRadius(){ return systemRadius; }
    public float GetNeighborhoodRadius(){ return neighborhoodRadius;}

    public GravityVector GetGravity(GameObject ship)
    {
        float modelScale = Parameters.GetModelScale();
        float distance = Vector3.Distance(transform.position, ship.transform.position) * modelScale;
        float acceleration =  G * mass / Mathf.Pow(distance, 2) * 1/modelScale;
        Vector3 offset = transform.position - ship.transform.position;
        offset = offset.normalized;

        return new GravityVector(name, acceleration * offset);
        /*SimpleShipScript simpleShipScript = ship.GetComponent<SimpleShipScript>();
        simpleShipScript.AddGravity(name, acceleration * offset);   */     
    }

    public void SetSkyPoint(Vector3 position)
    {
        skyPoint.transform.position = position;
        float realDistance = gb.transform.position.magnitude;
        float skyDistance = position.magnitude;
        float skyDiameter = diameter * skyDistance/realDistance;
        if(skyDiameter < 5000f){
            skyDiameter = 5000f;
        }
        skyPoint.transform.localScale = new Vector3(skyDiameter,skyDiameter,1);
        skyPoint.transform.rotation = Quaternion.LookRotation(position);
        skyPoint.SetActive(gameObject.activeSelf);
    }

    public void PhysicsClock(){
        if(!gameObject.activeSelf){
            return;
        }

        float timeScale = Parameters.getTimeScale();
        float updateTime = Parameters.GetUpdateTime();
    }

    public void RenderSatellites(){
        float timeScale = Parameters.getTimeScale();
        foreach(GameObject satellite in satellites){
            Body satelliteScript = satellite.GetComponent<Body>();
            float satelliteAngle = satelliteScript.GetAngle();
            float satelliteDistance = satelliteScript.GetDistance();
            float satelliteAngularVelocity = satelliteScript.GetAngularVelocity();
            float x = transform.position.x + satelliteDistance * Mathf.Cos(satelliteAngle);
            float y = transform.position.y;
            float z = transform.position.z + satelliteDistance * Mathf.Sin(satelliteAngle);
            satellite.transform.position = new Vector3(x,y,z);
            satelliteScript.SetAngle(satelliteAngle + satelliteAngularVelocity * timeScale * Parameters.GetUpdateTime());
            satelliteScript.RenderSatellites();
            satellite.SetActive(true);
        }
        foreach(NeighborEdge neighbor in neighbors){
            float neighborAngle = neighbor.GetAngle();
            float neighborDistance = neighbor.GetDistance();
            float x = transform.position.x + neighborDistance * Mathf.Cos(neighborAngle);
            float y = transform.position.y;
            float z = transform.position.z + neighborDistance * Mathf.Sin(neighborAngle);
            neighbor.GetBody().transform.position = new Vector3(x,y,z);
            neighbor.GetBody().SetActive(true);
        }
    }

    public void RenderPrimary(){
        Body primaryScript = primary.GetComponent<Body>();
        float x = transform.position.x - distance * Mathf.Cos(angle);
        float y = transform.position.y;
        float z = transform.position.z - distance * Mathf.Sin(angle);
        primary.transform.position = new Vector3(x,y,z);
        angle += angularVelocity;
        primary.SetActive(true);
        primaryScript.RenderSatellites();
    }

    public void RenderNeighbors(){
       
    }
}
