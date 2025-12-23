using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Engine : MonoBehaviour
{
    public GameObject gb;
    public GameObject fire;
    public Gradient flameGradient;
    public Renderer flameRenderer;

    public float mass;//kg
    public float maxForce;//N
    public float fuelEfficiency;//%

    private List<string> fuelTypes;
    private float thrust;//N
    private float power;
    private Vector3 thrustVector;
    private bool status = true;

    public void SetThrust(float thrust){
        this.thrust = thrust;
        power = thrust/maxForce;
    }

    public void SetThrustPercent(float percent){            
        this.thrust = maxForce * percent;
        power = percent;
    }

    public float GetThrust(){
        return thrust;
    }

    public float GetMass(){return mass; }

    public void SetStatus(bool status){
        this.status = status;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(fire == null){
            return;
        }
        if(!status){power = 0;}
        Color flameColor = flameGradient.Evaluate(power);

        // If using a standard material
        flameRenderer.material.color = flameColor;

        // If using emission (recommended)
        flameRenderer.material.SetColor("_EmissionColor", flameColor);
    }
}
