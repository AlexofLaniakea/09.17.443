using UnityEngine;
using TMPro;

public class StateIndicatorScript : MonoBehaviour
{
    public TMP_Text myTextMeshProText; 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myTextMeshProText.text = "Acceleration: 0m/s\n Velocity: 0m/s";

    }

    public void UpdateDisplay(float acceleration, float velocity){
         myTextMeshProText.text = "Acceleration: " + acceleration + "m/s^2\n Velocity: " + velocity + "m/s\n";
    }

    public void UpdateDisplay(float acceleration, float velocity, string name){
         myTextMeshProText.text = "Acceleration: " + acceleration + "m/s^2\n Velocity: " + velocity + "m/s\n"
         + "Strongest Gravity: " + name;
    }
}
