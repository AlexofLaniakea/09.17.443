using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class MainScript : MonoBehaviour//Manage space objects
{
    public GameObject body;
    public GameObject ship;
    public GameObject map;
    public TextAsset orbitFile;

    private float timeScale;
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

        foreach(string line in lines)
        {
            string[] values = line.Split(',');
            string name = values[0];
            string primaryName = values[1];
            float diameter = float.Parse(values[2]);
            float mass = float.Parse(values[3]);
            float distance = float.Parse(values[4]);
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

        focus = bodies[5];

        SimpleShipScript shipScript = ship.GetComponent<SimpleShipScript>();
        shipScript.SetPosition(new Vector3(300000,0,0));
        shipScript.SetFocus(focus);

        StartCoroutine(ClockOneSecond());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    IEnumerator ClockOneSecond()
    {
        while(true)
        {
            while(State.GetState() != 1){
                yield return new WaitForSeconds(0.1f);
            }
            /*focus.transform.position = new Vector3(0f,0f,0f);
            focus.GetComponent<Body>().RenderSatellites();
            focus.SetActive(true);*/

            SimpleShipScript shipScript = ship.GetComponent<SimpleShipScript>();


            //Next render all planets relative to ship
            Vector3 shipPosition = shipScript.GetPosition();
            focus = shipScript.GetFocus();
            focus.transform.position = shipPosition * -1;
            focus.SetActive(true);
            focus.GetComponent<Body>().RenderSatellites();
            if(focus.GetComponent<Body>().GetPrimary()){
                focus.GetComponent<Body>().RenderPrimary();
            }

            //When gravity from satellite becomes greater than that of the focus, switch to satellite
            //When gravity from primary becomes greater than that of the focus, switch to primary

            //Manage 2D render thing???
        
            shipScript.PhysicsClock();
            foreach(GameObject body in bodies){
                body.GetComponent<Body>().PhysicsClock();
            }

            map.GetComponent<MapDisplay>().SetFocus(focus, ship);

            yield return new WaitForSeconds(Parameters.GetUpdateTime());
        }
    }
}
