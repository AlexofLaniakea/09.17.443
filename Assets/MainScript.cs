using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class MainScript : MonoBehaviour
{
    public GameObject body;
    private float timeScale;
    private List<GameObject> bodies = new List<GameObject>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject earth = Instantiate(body);
        Body earthScript = earth.GetComponent<Body>();
        earthScript.Initialize("Earth", 12786f, (float)5.972 * Mathf.Pow(10, 24), new Vector3(12500f, 0f, 0f));
        GameObject moon = Instantiate(body);
        Body moonScript = moon.GetComponent<Body>();
        moonScript.Initialize("Moon", 3474, (float)7.348 * Mathf.Pow(10, 22), new Vector3(0f, 0f, 348400f));
        moonScript.SetVelocity(1.022f);
        moonScript.SetAngularVelocity(2.38f * Mathf.Pow(10, -6));
        earthScript.AddSatellite(moon);
        bodies.Add(earth);
        bodies.Add(moon);
        //earth.GetComponent<Rigidbody>().linearVelocity = new Vector3(1f, 1f, 1f);

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

            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(0.1f);
    }
}
