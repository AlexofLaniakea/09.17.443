using UnityEngine;
using UnityEngine.UI;

public class ZoomIn : MonoBehaviour
{
    public Button button;
    public MapDisplay display;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Add a listener to the button's onClick event
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    void OnButtonClick()
    {
        display.ZoomIn();
    }
}
