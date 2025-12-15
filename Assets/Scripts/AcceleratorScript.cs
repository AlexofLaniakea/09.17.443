using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AcceleratorScript : MonoBehaviour
{
    public TMP_Text textMeshPro;  

    public GameObject ship;
    //private SimpleShipScript script;
    private CompositeShip script;
    public Slider slider;
    private float acceleration = 0.0f;
    private float value;

    void Start()
    {
        //ship = GameObject.Find("Simple Ship");
        //script = ship.GetComponent<SimpleShipScript>();
        script = ship.GetComponent<CompositeShip>();

        // Subscribe to slider value change event
        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    void Update()
    {
        //script.SetThrust(acceleration);
    }

    // Called whenever the slider moves
    private void OnSliderChanged(float value)
    {
        this.value = value;
        acceleration = value * 10f/Parameters.GetModelScale();
        textMeshPro.text = "Thrust:\n"+acceleration*Parameters.GetModelScale()+"m/s^2";
    }

    public float GetValue(){
        return value;
    }
}
