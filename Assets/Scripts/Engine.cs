using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Engine : MonoBehaviour
{
    public GameObject gb;
    public GameObject ship;

    public float mass;//kg
    public float maxForce;//N
    public float fuelEfficiency;//%
    private List<string> fuelTypes;

    private float thrust;//N
    private Vector3 thrustVector;
    private bool on = true;

    public void SetThrust(float thrust){
        this.thrust = thrust;
    }

    public void SetThrustPercent(float percent){            
        this.thrust = maxForce * percent;
    }

    public float GetThrust(){
        return thrust;
    }

    public float GetMass(){return mass; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
