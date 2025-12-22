using UnityEngine;
using UnityEngine.UI;

public class BuildRocket : MonoBehaviour
{
    public Button button;
    public GameObject BuildZone;
    public GameObject cameraModeUI;
    public GameObject startMenu;
    public GameObject spawnButton;
    public GameObject ship;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        cameraModeUI.SetActive(false);
        startMenu.SetActive(false);
        BuildZone.SetActive(true);
    }
}
