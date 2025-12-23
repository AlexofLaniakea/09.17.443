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

    private Vector3 com;

    private float mass;
    private float I;
    private float torque;
    private float angularVelocity;
    private float angularAcceleration;

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
        }
        foreach(GameObject fuelTank in fuelTanks){
            FuelTank script = fuelTank.GetComponent<FuelTank>();
            Vector3 position = fuelTank.transform.position;
            float m = script.GetMass();
            sum += position * m;
            mass += m;
        }
        if(otherObject != null){
            Part script = otherObject.GetComponent<Part>();
            Vector3 position = otherObject.transform.position;
            float m = script.GetMass();
            sum += position * m;
            mass += m;
        }
        com = sum / mass;
    }

    public void UpdateMomentOfInertia(){
        float modelScale = 1f;
        I = 0f;
        Vector2 twoDcom = new Vector2(com.x,com.z);
        foreach(GameObject engine in engines){
            Engine script = engine.GetComponent<Engine>();
            Vector3 enginePos = engine.transform.position*modelScale;
            Vector2 twopos = new Vector2(enginePos.x, enginePos.z);
            Vector2 posPrime = twopos - twoDcom;
            float m = script.GetMass();
            I += m*Mathf.Pow(posPrime.magnitude, 2f);
        }
        foreach(GameObject fuelTank in fuelTanks){
            FuelTank script = fuelTank.GetComponent<FuelTank>();
            Vector3 tankPos = fuelTank.transform.position*modelScale;
            Vector2 twopos = new Vector2(tankPos.x, tankPos.z);
            Vector2 posPrime = twopos - twoDcom;
            float m = script.GetMass();
            I += m*Mathf.Pow(posPrime.magnitude, 2f);
        }
    }

    public void UpdateTorque(){
        float modelScale = 1f;
        torque = 0f;
        Vector3 com2D = new Vector3(com.x, com.z, 0f);
        foreach(GameObject engine in engines){
            Engine script = engine.GetComponent<Engine>();
            Vector3 enginePos = engine.transform.position;
            Vector3 twopos = new Vector3(enginePos.x, enginePos.z, 0f);
            Vector3 posPrime = twopos - com2D;
            float m = script.GetMass();
            Vector3 orientation = engine.transform.forward.normalized;
            float force = script.GetThrust();
            Vector3 forceVector = force * orientation;
            Vector3 forceVector2D = new Vector3(forceVector.x, forceVector.z, 0f);
            torque += Vector3.Cross(posPrime, forceVector2D).z;
        }
        angularAcceleration = torque / I;
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
            thrust = 0;
            kinematicsDisplay.SetFuel(0);
            foreach(GameObject engine in engines){
                Engine script = engine.GetComponent<Engine>();
                script.SetStatus(false);
            }
            return;
        }
        float forcePerTank = thrustForce/activeTanks*Parameters.GetUpdateTime();
        forcePerTank*= Parameters.getTimeScale();
        float remainingFuel = 0;
        float totalCapacity = 0;
        foreach(GameObject fuelTank in fuelTanks){
            FuelTank script = fuelTank.GetComponent<FuelTank>();
            if(script.GetFuelMass() > 0){
                float specificImpulse = script.GetSpecificImpulse();
                float massUsed = forcePerTank/specificImpulse;
                float fuelMass = script.GetFuelMass();
                script.SetFuelMass(fuelMass-massUsed);
                fuelMass = script.GetFuelMass();
                remainingFuel += fuelMass;
                totalCapacity += script.GetCapacity();
                if(fuelMass < 0){
                    script.SetFuelMass(0);
                }
            }
        }
        kinematicsDisplay.SetFuel(remainingFuel/totalCapacity);
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
        UpdateMomentOfInertia();
        UpdateThrustVector();

        mainScript = main.GetComponent<MainScript>();
        StartCoroutine(ClockOneSecond());
        StartCoroutine(ClockOneSecond2());
    }

    IEnumerator ClockOneSecond2(){
        while(true){
            UpdateThrustVector();
            UpdateCOM();
            UpdateMomentOfInertia();
            UpdateTorque();
            float timeScale = Parameters.getTimeScale();
            float updateTime = Parameters.GetUpdateTime();

            //rotate shi
            angularVelocity += angularAcceleration * timeScale*updateTime;
            gb.transform.eulerAngles += new Vector3(0,angularVelocity*updateTime*timeScale,0);
            Debug.Log(angularVelocity);
            //gb.transform.Rotate(Vector3.forward, angularVelocity * updateTime, Space.Self);
            yield return new WaitForSeconds(Parameters.GetUpdateTime());
        }
    }
}
