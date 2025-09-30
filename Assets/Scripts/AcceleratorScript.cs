using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AcceleratorScript : MonoBehaviour
{
    public TMP_Text textMeshPro;  

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
        script.SetThrust(acceleration);
    }

    // Called whenever the slider moves
    private void OnSliderChanged(float value)
    {
        acceleration = value * Mathf.Pow(10, -2);
        textMeshPro.text = "Thrust:\n"+acceleration * 1000f+"m/s^2";
    }
}
