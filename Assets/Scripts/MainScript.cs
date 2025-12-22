using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class MainScript : MonoBehaviour//Manage space objects
{
    public GameObject body;
    public GameObject ship;
    public GameObject map;
    public GameObject spawnButton;
    public TextAsset orbitFile;

    private float timeScale;
    private float modelScale;
    private List<GameObject> bodies = new List<GameObject>();
    private GameObject focus;
    private GameObject core;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Constructor: Name, diameter, mass, distance from primary, initial angle, angular velocity

        //Read csv file
        string csvContent = orbitFile.text;

        string[] lines = csvContent.Split('\n');
        modelScale = Parameters.GetModelScale();

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

            float angularVelocity = 0f;

            if(time > 0f){
                angularVelocity = 2*Mathf.PI/time;
            }

            GameObject newBody = Instantiate(body);
            Body script = newBody.GetComponent<Body>();
            script.Initialize(name, diameter, mass, distance, startAngle, angularVelocity);
            foreach(GameObject b in bodies){
                Body bScript = b.GetComponent<Body>();
                if(bScript.GetName().Equals(primaryName)){
                    bScript.AddSatellite(newBody);
                    break;
                }
            }
            bodies.Add(newBody);
        }


        //SimpleShipScript shipScript = ship.GetComponent<SimpleShipScript>();

        
        //Make two rows of four buttons each. GUI width is 300 from the center
        for (int i = 0; i < 2; i++){
            for (int j = 0; j < 4; j++){
                GameObject newSpawnButton = Instantiate(spawnButton);
                SpawnButton buttonScript = newSpawnButton.GetComponent<SpawnButton>();
                buttonScript.Initialize(bodies[i*4+j+1], new Vector2(j*160-240,i*-100f));
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public void Render(GameObject focus, Vector3 position)
    {
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
           // body.GetComponent<Body>().SetSkyPoint((body.transform.position).normalized * 100f);
        }

        foreach(GameObject body in bodies){
            body.GetComponent<Body>().PhysicsClock();
        }
    }

}
