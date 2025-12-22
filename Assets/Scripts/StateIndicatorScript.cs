using UnityEngine;
using TMPro;

public class StateIndicatorScript : MonoBehaviour
{
    public TMP_Text myTextMeshProText; 
    private float fuel;
    private bool trackFuel = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myTextMeshProText.text = "Net Acceleration: 0m/s\n Velocity: 0m/s";

    }

    public void UpdateDisplay(float acceleration, float velocity){
        string text = "";
        if(trackFuel){
            text += "Fuel: "+fuel*100 + "%\n";
        }
        myTextMeshProText.text = text + "Net Acceleration: " + acceleration + "m/s^2\nVelocity: " + velocity + "m/s";
    }

    public void UpdateDisplay(float acceleration, float velocity, string name){
         myTextMeshProText.text = "Acceleration: " + acceleration + "m/s^2\nVelocity: " + velocity + "m/s\n"
         + "Strongest Gravity: " + name;
    }

    public void SetFuel(float fuel){
        this.fuel = fuel;
        trackFuel=true;
    }
}
