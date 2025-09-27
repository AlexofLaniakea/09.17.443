using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class MainScript : MonoBehaviour//Manage space objects
{
    public GameObject body;
    private float timeScale;
    private List<GameObject> bodies = new List<GameObject>();
    public GameObject focus;
    public GameObject core;
    public GameObject ship;

    public GameObject map;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Constructor: Name, diameter, mass, distance from primary, initial angle, angular velocity

        GameObject sun = Instantiate(body);
        Body sunScript = sun.GetComponent<Body>();
        sunScript.Initialize("Sun", 1400000, (float)1.989 * Mathf.Pow(10, 30), 0f, 0f, 2.66f * Mathf.Pow(10, -6));

        GameObject earth = Instantiate(body);
        Body earthScript = earth.GetComponent<Body>();
        earthScript.Initialize("Earth", 12786, (float)5.972 * Mathf.Pow(10, 24), 150000000f, 0.39f, 2.66f * Mathf.Pow(10, -6));
        sunScript.AddSatellite(earth);

        GameObject jupiter = Instantiate(body);
        Body jupiterScript = jupiter.GetComponent<Body>();
        jupiterScript.Initialize("Jupiter", 139820, (float)1.898 * Mathf.Pow(10, 27), 773000000f, 0.106f, 1.67f * Mathf.Pow(10, -8));
        bodies.Add(jupiter);
        sunScript.AddSatellite(jupiter);

        GameObject io = Instantiate(body);
        Body ioScript = io.GetComponent<Body>();
        ioScript.Initialize("Io", 3643, 8.93f * Mathf.Pow(10, 22), 421700, 4.11f, 4.11f * Mathf.Pow(10, -5));
        bodies.Add(io);
        jupiterScript.AddSatellite(io);

        GameObject europa = Instantiate(body);
        Body europaScript = europa.GetComponent<Body>();
        europaScript.Initialize("Europa", 3122, (float)4.80 * Mathf.Pow(10, 22), 670900, 2.05f, 4.94f * Mathf.Pow(10, -5));
        bodies.Add(europa);
        jupiterScript.AddSatellite(europa);

        GameObject ganymede = Instantiate(body);
        Body ganymedeScript = ganymede.GetComponent<Body>();
        ganymedeScript.Initialize("Ganymede", 5262, (float)1.48 * Mathf.Pow(10, 23), 1070000, 0f, 1.02f * Mathf.Pow(10, -5));
        bodies.Add(ganymede);
        jupiterScript.AddSatellite(ganymede);

        GameObject callisto = Instantiate(body);
        Body callistoScript = callisto.GetComponent<Body>();
        callistoScript.Initialize("Callisto", 4821, 1.08f * Mathf.Pow(10, 23), 1883000, 5.10f, 4.36f * Mathf.Pow(10, -6));
        bodies.Add(callisto);
        jupiterScript.AddSatellite(callisto);


        GameObject moon = Instantiate(body);
        Body moonScript = moon.GetComponent<Body>();
        moonScript.Initialize("Moon", 3474, (float)7.348 * Mathf.Pow(10, 22), 348400f, 0f, 2.66f * Mathf.Pow(10, -6));

        earthScript.AddSatellite(moon);
        bodies.Add(earth);
        bodies.Add(moon);
        bodies.Add(sun);
        //earth.GetComponent<Rigidbody>().linearVelocity = new Vector3(1f, 1f, 1f);
        focus = earth;

        SimpleShipScript shipScript = ship.GetComponent<SimpleShipScript>();
        shipScript.SetPosition(new Vector3(13000,0,0));

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
            /*focus.transform.position = new Vector3(0f,0f,0f);
            focus.GetComponent<Body>().RenderSatellites();
            focus.SetActive(true);*/

            SimpleShipScript shipScript = ship.GetComponent<SimpleShipScript>();


            //Next render all planets relative to ship
            Vector3 shipPosition = shipScript.GetPosition();
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
