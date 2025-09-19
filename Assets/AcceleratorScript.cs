using UnityEngine;
using UnityEngine.UI; // needed for Slider

public class AcceleratorScript : MonoBehaviour
{
    private GameObject ship;
    private SimpleShipScript script;
    public Slider slider;
    private float acceleration = 0.0f;

    void Start()
    {
        ship = GameObject.Find("Simple Ship");
        script = ship.GetComponent<SimpleShipScript>();

        // Subscribe to slider value change event
        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    void Update()
    {
        script.setAcceleration(acceleration);
    }

    // Called whenever the slider moves
    private void OnSliderChanged(float value)
    {
        acceleration = value * 10f;
    }
}
