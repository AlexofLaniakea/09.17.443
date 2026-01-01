using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class MainScript : MonoBehaviour//Manage space objects
{
    //Mathematical constants
    private float parsec = (float)3.086 * Mathf.Pow(10, 13);

    public GameObject body;
    public GameObject ship;
    public GameObject map;
    public GameObject spawnButton;
    public TextAsset orbitFile;
    public TextAsset starFile;
    public int starCount = 0;

    private float timeScale;
    private float modelScale;
    private List<GameObject> bodies = new List<GameObject>();
    private GameObject focus;
    private GameObject core;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Constructor: Name, diameter, mass, distance from primary, initial angle, angular velocity
        modelScale = Parameters.GetModelScale();
        //Load Sol
        //Load other stars
        //Load planets

        //Read csv file
        string csvContent = orbitFile.text;

        string[] lines = csvContent.Split('\n');
        string[] solValues = lines[1].Split(',');
        GameObject sol = Instantiate(body);
        Body solScript = sol.GetComponent<Body>(); 
        float solDiameter = float.Parse(solValues[2])*1000f/modelScale;
        float solMass = float.Parse(solValues[3]);
        solScript.Initialize("Sol",solDiameter,solMass,0f,0f,0f,"Sol");
        bodies.Add(sol);

        string starContent = starFile.text;
        string[] starLines = starContent.Split('\n');
        if(starCount == 0){starCount = 30;}
        int realStarCount = 1;
        for(int i = 2; i < starCount; i++){
            string line = starLines[i];
            string[] values = line.Split(',');
            string name = values[1];
            float distance = float.Parse(values[2])*parsec;
            float x = float.Parse(values[3])*parsec;
            float y = float.Parse(values[4])*parsec;
            float z = float.Parse(values[5])*parsec;
            if(z>(Mathf.Abs(x) + Mathf.Abs(y))/2f && Mathf.Abs(z)>1.2f){continue;}
            Vector2 pos = new Vector2(x,y);
            pos = pos.normalized * distance;
            float angle = Mathf.Atan2(pos.y, pos.x);
            GameObject newBody = Instantiate(body);
            Body script = newBody.GetComponent<Body>();
            script.Initialize(name,solDiameter,solMass,distance,0,0f,"Sol");
            NeighborEdge neighbor = new NeighborEdge(newBody, distance, angle);
            solScript.AddNeighbor(neighbor);
            bodies.Add(newBody);
            realStarCount += 1;
        }

        for(int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] values = line.Split(',');
            string name = values[0];
            string primaryName = values[1];
            float diameter = float.Parse(values[2])*1000f/modelScale;
            float mass = float.Parse(values[3]);
            float distance = float.Parse(values[4])*1000f/modelScale;
            float startAngle = float.Parse(values[5]);
            float time = float.Parse(values[6]);
            float isSatellite = float.Parse(values[7]);
            string texture = values[8];
            if(texture == null || texture.Length == 1){
                texture = name;
            }

            float angularVelocity = 0f;

            if(time != 0f){
                angularVelocity = 2*Mathf.PI/time;
            }

            GameObject newBody = Instantiate(body);
            Body script = newBody.GetComponent<Body>();
            script.Initialize(name,diameter,mass,distance,startAngle,angularVelocity,texture);
            foreach(GameObject b in bodies){
                Body bScript = b.GetComponent<Body>();
                if(bScript.GetName().Equals(primaryName)){
                    if(isSatellite == 1){bScript.AddSatellite(newBody);}
                    else{
                        NeighborEdge neighbor = new NeighborEdge(newBody, distance, startAngle);
                        bScript.AddNeighbor(neighbor);
                    }
                    break;
                }
            }
            bodies.Add(newBody);
        }

        //Make every neighbor of Sol also a neighbor of each other
        List<NeighborEdge> solNeighbors = sol.GetComponent<Body>().GetNeighbors();
        int neighborCount = solNeighbors.Count;
        for(int i = 0; i < neighborCount; i++){
            NeighborEdge neighbori = solNeighbors[i];
            GameObject bodyi = neighbori.GetBody();
            for(int j = 0; j < neighborCount; j++){
                if(i == j){continue;}
                NeighborEdge neighborj = solNeighbors[j];
                GameObject bodyj = neighborj.GetBody();
                string namei = bodyi.name;
                string namej = bodyj.name;
                float anglei = neighbori.GetAngle();
                float anglej = neighborj.GetAngle();
                float distancei = neighbori.GetDistance();
                float distancej = neighborj.GetDistance();
                float xi = distancei * Mathf.Cos(anglei);
                float xj = distancej * Mathf.Cos(anglej);
                float yi = distancei * Mathf.Sin(anglei);
                float yj = distancej * Mathf.Sin(anglej);
                Vector2 positioni = new Vector2(xi,yi);
                Vector2 positionj = new Vector2(xj,yj);
                Vector2 delta = positionj-positioni;
                float distance = Vector2.Distance(positioni,positionj);
                float angle = Mathf.Atan2(delta.y, delta.x);
                NeighborEdge neighbor = new NeighborEdge(neighborj.GetBody(),distance,angle); 
                neighbori.GetBody().GetComponent<Body>().AddNeighbor(neighbor);
            }
            NeighborEdge solEdge = new NeighborEdge(sol,neighbori.GetDistance(),neighbori.GetAngle()-Mathf.PI);
            bodyi.GetComponent<Body>().AddNeighbor(solEdge);
        }
        
        //Make two rows of four buttons each. GUI width is 300 from the center
        for (int i = 0; i < 2; i++){
            for (int j = 0; j < 4; j++){
                GameObject newSpawnButton = Instantiate(spawnButton);
                SpawnButton buttonScript = newSpawnButton.GetComponent<SpawnButton>();
                buttonScript.Initialize(bodies[realStarCount + i*4+j+1], new Vector2(j*160-240,i*-100f));
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public void Render(GameObject focus, Vector3 position)
    {
        foreach(GameObject body in bodies){
            body.SetActive(false);
        }
        //Next render all planets relative to ship
        Vector3 shipPosition = position;
        focus.transform.position = shipPosition * -1;
        focus.SetActive(true);

        //focus.GetComponent<Body>().RenderSatellites();
        if(focus.GetComponent<Body>().GetPrimary()){
            focus.GetComponent<Body>().RenderPrimary();
        }
        else{
            focus.GetComponent<Body>().RenderSatellites();
        }

        foreach(GameObject body in bodies){
           body.GetComponent<Body>().SetSkyPoint((body.transform.position).normalized * 1000000f);
        }

        foreach(GameObject body in bodies){
            body.GetComponent<Body>().PhysicsClock();
        }
    }

}
