using UnityEngine;
using UnityEngine.UI;

public class SpawnConstructedRocket : MonoBehaviour
{
    public Button button;

    public GameObject ship;
    public GameObject buildUI;
    public GameObject buildZone;
    public GameObject ships;
    public GameObject startUI;
    public GameObject spawnSelectionUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        ship.transform.SetParent(ships.transform);
        buildUI.SetActive(false);
        buildZone.SetActive(false);
        ship.GetComponent<CompositeShip>().enabled=true;
        startUI.SetActive(true);
        spawnSelectionUI.SetActive(true);
        ship.transform.position *= 0;
    }
}
