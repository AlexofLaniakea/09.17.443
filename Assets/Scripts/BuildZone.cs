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
        if (Input.GetMouseButtonDown(1))
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

                GameObject placing = Instantiate(selected);
                float x = Mathf.Floor(pos.x);
                float y = Mathf.Floor(pos.y);
                float z = Mathf.Floor(pos.z);

                pos = new Vector3(x,y,z);
                Vector3 size = placing.transform.localScale;
                Vector3 rotatedSize = size;
                int mag = 1;
                /*if(hit.normal.x < -0.1){ 
                    placing.transform.eulerAngles = new Vector3(0,-90f,0);
                    rotatedSize = new Vector3(size.z, size.y, size.x);
                }
                else if(hit.normal.x > 0.1){ 
                    placing.transform.eulerAngles = new Vector3(0,90f,0);
                    rotatedSize = new Vector3(size.z, size.y, size.x);
                }
                else if (hit.normal.z < -0.1)
                {
                    placing.transform.eulerAngles = new Vector3(0, 180f, 0);
                }
                else if (hit.normal.y > 0.1)
                {
                    placing.transform.eulerAngles = new Vector3(-90f, 0, 0);
                    rotatedSize = new Vector3(size.x, size.z, size.y);
                }
                else if (hit.normal.y < -0.1)
                {
                    placing.transform.eulerAngles = new Vector3(90f, 0, 0);
                    rotatedSize = new Vector3(size.x, size.z, size.y);
                }*/
                if(hit.normal.x<0||hit.normal.y<0||hit.normal.z<0){mag= -1;}
                if(mag == 1){
                    placing.transform.position=pos + (rotatedSize/2f);
                }
                else{
                    placing.transform.position=pos-rotatedSize/2f+new Vector3(1,1,1)+hit.normal;
                }
                placing.transform.SetParent(category.transform);
                placing.SetActive(true);
            }
        }
        if(Input.GetMouseButtonDown(0)){
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
                GameObject clickedObject = hit.collider.gameObject;
                string name = clickedObject.name;
                if(!name.Equals("Build Platform")){
                    Destroy(clickedObject);
                }
            }  
        }
    }
}
