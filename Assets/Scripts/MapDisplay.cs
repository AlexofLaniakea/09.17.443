using UnityEngine;
using System.Collections.Generic;

public class MapDisplay : MonoBehaviour
{
    public GameObject gb;

    public GameObject shipDot;
    public GameObject satellitePrefab;
    public GameObject center;

    float mapWidth = 300f;

    private GameObject ship;
    List<GameObject> satellites;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        satellites = new List<GameObject>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFocus(GameObject focus, GameObject ship){
        Body focusScript = focus.GetComponent<Body>();
        SimpleShipScript shipScript = ship.GetComponent<SimpleShipScript>();


        this.ship = ship;

        if(satellites != null){
            foreach(GameObject satellite in satellites){
                Destroy(satellite);
            }
        }
        
        int satelliteCount = focusScript.GetSatellites().Count;

        float radius = focusScript.GetSatellites()[satelliteCount - 1].GetComponent<Body>().GetDistance();

        foreach(GameObject satellite in focusScript.GetSatellites()){
            Body satelliteScript = satellite.GetComponent<Body>();
            GameObject image = Instantiate(satellitePrefab);
            float realDistance = satelliteScript.GetDistance();
            float ratio = satelliteScript.GetDistance() / radius;
            float angle = satelliteScript.GetAngle();
            float guiDistance = ratio * mapWidth / 2;

            float x = guiDistance * Mathf.Cos(angle);
            float y = guiDistance * Mathf.Sin(angle);
            
            image.transform.SetParent(gb.transform);
            image.GetComponent<RectTransform>().localPosition = new Vector2(x, y);
            image.SetActive(true);

            satellites.Add(image);
        }

        Vector2 position = new Vector2(shipScript.GetPosition().x, shipScript.GetPosition().y);
        position = position / radius;
        position = position * mapWidth / 2;
        
        shipDot.GetComponent<RectTransform>().localPosition = position;
    }

    public void UpdateDisplay(){
        //Set ship dot to position relative to center
        //Set satellites relative to center
    }
}
