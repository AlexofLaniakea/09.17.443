using UnityEngine;
using UnityEngine.UI;

public class FreeMode : MonoBehaviour
{
    
    public Button button;
    public GameObject ship;
    public GameObject cameraModeUI;
    public GameObject SpawnSelectionUI;

    private SimpleShipScript shipScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        ship = Instantiate(ship);
        shipScript = ship.GetComponent<SimpleShipScript>();
        shipScript.SetFreeCam(true);

        SpawnSelectionUI.SetActive(true);
        cameraModeUI.SetActive(false);
        SpawnButton.SetShip(ship);
    }
    
}