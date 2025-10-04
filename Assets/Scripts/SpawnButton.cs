using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpawnButton : MonoBehaviour
{
    public TMP_Text textMeshPro;  
 
    public Button button;
    public GameObject ship;
    public GameObject gb;
    public GameObject startMenu;
    public GameObject flightGUi;


    private GameObject body;


    public void Initialize(GameObject body, Vector2 position){
        this.body = body;

        gb.SetActive(true);
        gb.transform.SetParent(GameObject.Find("StartMenu").transform);
        gb.GetComponent<RectTransform>().localPosition = position;
        Body bodyScript = body.GetComponent<Body>();

        textMeshPro.text = bodyScript.GetName();
        //button.onClick.AddListener(OnButtonClick);
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
        shipScript.SetPosition(new Vector3(bodyScript.GetSystemRadius()/4f,0f,0f));

        State.SetState(1);
        startMenu.SetActive(false);
        flightGUi.SetActive(true);
    }
}
