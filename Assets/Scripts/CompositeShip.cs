using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CompositeShip : SimpleShipScript
{
    public GameObject otherObject;
    public GameObject enginesObject;
    public GameObject fuelTanksObject;

    private List<GameObject> others;
    private List<GameObject> engines;
    private List<GameObject> fuelTanks;


    private Vector3 angularVelocity;
    private Vector3 com;
    private Vector3 thrustVector;
    private Vector3 angularAcceleration;

    private float mass;

    public void UpdateCOM(){
        Vector3 sum = new Vector3(0,0,0);
        int adding = 0;
        mass = 0;
        foreach(GameObject engine in engines){
            Engine script = engine.GetComponent<Engine>();
            Vector3 position = engine.transform.position;
            float m = script.GetMass();
            sum += position * m;
            mass += m;
            adding += 1;
        }
        foreach(GameObject fuelTank in fuelTanks){
            FuelTank script = fuelTank.GetComponent<FuelTank>();
            Vector3 position = fuelTank.transform.position;
            float m = script.GetMass();
            sum += position * m;
            mass += m;
            adding += 1;
        }
        if(otherObject != null){
            Part script = otherObject.GetComponent<Part>();
            Vector3 position = otherObject.transform.position;
            float m = script.GetMass();
            sum += position * m;
            mass += m;
            adding += 1;
        }
        com = sum / adding;
    }

    public void UpdateThrustVector(){
        Vector3 sum = new Vector3(0,0,0);
        Vector3 angularVelocity = new Vector3(0,0,0);
        int activeEngines = 0;
        foreach(GameObject engine in engines){
            Engine script = engine.GetComponent<Engine>();
            Vector3 position = engine.transform.position;
            Vector3 orientation = engine.transform.forward.normalized;
            float force = script.GetThrust();
            sum += orientation*force;
            activeEngines++;
        }
        thrustVector = sum/mass;
        thrust = thrustVector.magnitude;
        thrust /= Parameters.GetModelScale();
        int activeTanks = 0;
        float thrustForce = thrust*mass*Parameters.GetModelScale();
        foreach(GameObject fuelTank in fuelTanks){
            FuelTank script = fuelTank.GetComponent<FuelTank>();
            if(script.GetFuelMass() > 0){
                activeTanks++;
            }
        }
        if(activeTanks == 0){
            Debug.Log("Out of fuel");
            thrust = 0;
        }
        float forcePerTank = thrustForce/activeTanks*Parameters.GetUpdateTime();
        forcePerTank*= Parameters.getTimeScale();
        foreach(GameObject fuelTank in fuelTanks){
            FuelTank script = fuelTank.GetComponent<FuelTank>();
            if(script.GetFuelMass() > 0){
                float specificImpulse = script.GetSpecificImpulse();
                float massUsed = forcePerTank/specificImpulse;
                float fuelMass = script.GetFuelMass();
                script.SetFuelMass(fuelMass-massUsed);
                Debug.Log(fuelMass);
                if(fuelMass-massUsed<0){
                    script.SetFuelMass(0);
                }
            }
        }
    }

    public override void SetThrust(float thrust){
        foreach(GameObject engine in engines){
            Engine script = engine.GetComponent<Engine>();
            script.SetThrustPercent(thrust);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        engines = new List<GameObject>();
        fuelTanks = new List<GameObject>();
        foreach (Transform childTransform in enginesObject.transform)
        {
            engines.Add(childTransform.gameObject);
        }
        foreach (Transform childTransform in fuelTanksObject.transform)
        {
            fuelTanks.Add(childTransform.gameObject);
        }
        com = new Vector3(0,0,0);
        UpdateCOM();
        UpdateThrustVector();

        mainScript = main.GetComponent<MainScript>();
        StartCoroutine(ClockOneSecond());
        StartCoroutine(ClockOneSecond2());
    }

    IEnumerator ClockOneSecond2(){
        while(true){
            UpdateThrustVector();
            yield return new WaitForSeconds(Parameters.GetUpdateTime());
        }
    }
}
