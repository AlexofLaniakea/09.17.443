using UnityEngine;
using UnityEngine.UI;

public class PartSelection : MonoBehaviour
{
    public Button button;
    public GameObject part;
    public GameObject category;
    public GameObject buildZone;

    private BuildZone buildZoneScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button.onClick.AddListener(OnButtonClick);
        buildZoneScript = buildZone.GetComponent<BuildZone>();
    }

    void OnButtonClick()
    {
        buildZoneScript.SetSelected(part,category);
    }
}
