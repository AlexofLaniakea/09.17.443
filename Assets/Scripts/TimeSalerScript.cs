using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeScalerScript : MonoBehaviour
{

    public TMP_Text textMeshPro;  

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
        Parameters.setTimeScale(timeScale);
    }

    // Called whenever the slider moves
    private void OnSliderChanged(float value)
    {
        timeScale = value * 999f + 1f;

        textMeshPro.text = "Time:\n"+timeScale+"x";

    }
}
