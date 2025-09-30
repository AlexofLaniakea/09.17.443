using UnityEngine;
using TMPro;

public class StateIndicatorScript : MonoBehaviour
{
    public TMP_Text myTextMeshProText; 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myTextMeshProText.text = "Net Acceleration: 0m/s\n Velocity: 0m/s";

    }

    public void UpdateDisplay(float acceleration, float velocity){
         myTextMeshProText.text = "Net Acceleration: " + acceleration + "m/s^2\nVelocity: " + velocity + "m/s";
    }

    public void UpdateDisplay(float acceleration, float velocity, string name){
         myTextMeshProText.text = "Acceleration: " + acceleration + "m/s^2\nVelocity: " + velocity + "m/s\n"
         + "Strongest Gravity: " + name;
    }
}
