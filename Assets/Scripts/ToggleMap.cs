using UnityEngine;
using UnityEngine.UI;

public class ToggleMap : MonoBehaviour
{
    public Button button;
    public GameObject display;

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
        if(display.activeSelf){
            display.SetActive(false);
        }
        else{
            display.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
