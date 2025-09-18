using UnityEngine;
using System.Collections.Generic;


public class Body : MonoBehaviour
{
    string name = "Earth";
    float mass = (float)5.972 * Mathf.Pow(10, 24);
    GameObject shipsListObject;
    public List<GameObject> ships;
    float G = (float)6.673 * Mathf.Pow(10, -11);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shipsListObject = GameObject.Find("Ships");
        foreach (Transform child in shipsListObject.transform)
        {
            ships.Add(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Find the distance between body and all nearby ships and add force?
    }

    void FixedUpdate()
    {
        foreach(GameObject ship in ships){
            float distance = Vector3.Distance(transform.position, ship.transform.position) * 10000;
            float acceleration =  G * mass / Mathf.Pow(distance, 2);
            Vector3 offset = ship.transform.position - transform.position;
            Debug.Log(acceleration);
        }
    }
}
