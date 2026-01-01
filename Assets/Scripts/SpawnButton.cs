using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpawnButton : MonoBehaviour
{
    public TMP_Text textMeshPro;  
 
    private static GameObject ship;
    public Button button;
    public GameObject gb;
    public GameObject startMenu;
    public GameObject flightGUi;
    public GameObject planetSelection;

    private GameObject body;


    public void Initialize(GameObject body, Vector2 position){
        this.body = body;

        gb.SetActive(true);
        gb.transform.SetParent(planetSelection.transform);
        gb.GetComponent<RectTransform>().localPosition = position;
        Body bodyScript = body.GetComponent<Body>();

        textMeshPro.text = bodyScript.GetName();
        //button.onClick.AddListener(OnButtonClick);
    }

    public static void SetShip(GameObject sh){
        ship=sh;
    }


    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    void OnButtonClick()
    {
        SimpleShipScript shipScript = ship.GetComponent<SimpleShipScript>();
        Body bodyScript = body.GetComponent<Body>();

        shipScript.SetFocus(body);
        shipScript.SetPosition(new Vector3(bodyScript.GetSystemRadius()/8f,0f,0f));
        shipScript.SetVelocity(new Vector3(0f,0f,0f));

        State.SetState(1);
        startMenu.SetActive(false);
        planetSelection.SetActive(false);
        flightGUi.SetActive(true);
        ship.SetActive(true);
        ship.GetComponent<SimpleShipScript>().enabled=true;

    }
}
