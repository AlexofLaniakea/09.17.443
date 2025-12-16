using UnityEngine;

public class FuelTank : MonoBehaviour
{
    public GameObject gb;
    public float specificImpulse;
    public float dryMass;//kg
    public float fuelMass;//kg
    private float mass;

    public void SetFuelMass(float mass){
        fuelMass = mass;
    }

    public float GetMass(){return dryMass+fuelMass; }

    public void UpdateMass(){mass = dryMass+fuelMass;}

    public float GetSpecificImpulse(){ return specificImpulse; }

    public float GetFuelMass(){ return fuelMass; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateMass();
    }

    // Update is called once per frame
    void Update()
    {
        
    }   
}
