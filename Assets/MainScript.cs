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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject earth = Instantiate(body);
        Body earthScript = earth.GetComponent<Body>();
        earthScript.Initialize("Earth", 12786f, (float)5.972 * Mathf.Pow(10, 24), 10000000f, 0f, 2.66f * Mathf.Pow(10, -6));
        GameObject moon = Instantiate(body);
        Body moonScript = moon.GetComponent<Body>();
        moonScript.Initialize("Moon", 3474, (float)7.348 * Mathf.Pow(10, 22), 348400f, 0f, 2.66f * Mathf.Pow(10, -6));
        GameObject sun = Instantiate(body);
        Body sunScript = sun.GetComponent<Body>();
        sunScript.Initialize("Sun", 1400000, (float)1.989 * Mathf.Pow(10, 33), 0f, 0f, 2.66f * Mathf.Pow(10, -6));
        earthScript.AddSatellite(moon);
        sunScript.AddSatellite(earth);
        bodies.Add(earth);
        bodies.Add(moon);
        //earth.GetComponent<Rigidbody>().linearVelocity = new Vector3(1f, 1f, 1f);
        focus = earth;

        StartCoroutine(Clock());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    IEnumerator Clock()
    {
        while(true)
        {
            focus.transform.position = new Vector3(0f,0f,0f);
            focus.GetComponent<Body>().RenderSatellites();
            focus.SetActive(true);

            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(0.1f);
    }
}
