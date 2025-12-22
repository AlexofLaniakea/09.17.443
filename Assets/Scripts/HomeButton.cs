using UnityEngine;
using UnityEngine.UI;

public class HomeButton : MonoBehaviour
{
    public Button button;
    public GameObject startMenu;
    public GameObject cameraModeMenu;
    public GameObject flightGUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    void OnButtonClick()
    {
        startMenu.SetActive(true);
        cameraModeMenu.SetActive(true);
        flightGUI.SetActive(true);
        State.SetState(0);
    }
}
