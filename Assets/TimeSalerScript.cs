using UnityEngine;
using UnityEngine.UI; // needed for Slider

public class TimeScalerScript : MonoBehaviour
{
    private GameObject ship;
    private SimpleShipScript script;
    public Slider slider;
    private float timeScale = 1.0f;

    void Start()
    {
        ship = GameObject.Find("Simple Ship");
        script = ship.GetComponent<SimpleShipScript>();

        // Subscribe to slider value change event
        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    void Update()
    {
        script.setTimeScale(timeScale);
        Parameters.setTimeScale(timeScale);
    }

    // Called whenever the slider moves
    private void OnSliderChanged(float value)
    {
        timeScale = value * 999f + 1f;
    }
}
