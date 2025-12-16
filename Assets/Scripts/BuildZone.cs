using UnityEngine;
using UnityEngine.EventSystems;

public class BuildZone : MonoBehaviour
{
    public GameObject UI;
    public Camera cam;

    public GameObject ship;
    public GameObject engines;
    public GameObject fuelTanks;

    private GameObject category;
    private GameObject selected;

    public void SetSelected(GameObject selected, GameObject category){ 
        this.selected=selected; 
        this.category=category;
    }

    void OnEnable()
    {
        UI.SetActive(true);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Block world click if pointer is over UI
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Vector3 pos = hit.point;
                Debug.Log($"Clicked world position: {pos}");

                GameObject placing = Instantiate(selected);
                float x = Mathf.Floor(pos.x)+0.5f;
                float y = Mathf.Floor(pos.y)+0.5f;
                float z = Mathf.Floor(pos.z)+0.5f;
                float height = placing.transform.localScale.z;
                pos = new Vector3(x,y,z);
                placing.transform.position=pos + new Vector3(0,0,height/2f);
                placing.transform.SetParent(category.transform);
                placing.SetActive(true);
            }
        }
    }

}
