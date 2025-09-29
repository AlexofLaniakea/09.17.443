using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    public Button button;
    public GameObject startMenu;
    public GameObject flightGUi;


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
        State.SetState(1);
        startMenu.SetActive(false);
        flightGUi.SetActive(true);
    }
}
