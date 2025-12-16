using UnityEngine;
using UnityEngine.UI;

public class BuildRocket : MonoBehaviour
{
    public Button button;
    public GameObject BuildZone;
    public GameObject cameraModeUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        cameraModeUI.SetActive(false);
        BuildZone.SetActive(true);
    }
}
