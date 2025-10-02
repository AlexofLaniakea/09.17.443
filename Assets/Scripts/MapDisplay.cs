using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapDisplay : MonoBehaviour
{
    public GameObject gb;
    public RawImage spriteRenderer;
    public Texture2D texture;


    public GameObject shipDot;
    public GameObject satellitePrefab;
    public GameObject center;

    float mapWidth = 225f;

    private GameObject ship;
    private float zoom = 1f;
    List<GameObject> satellites;

    public void ZoomIn(){
        zoom *= 1.1f;
    }

    public void ZoomOut(){
        if(zoom > 1f){
            zoom /= 1.1f;

        }
    }

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
        if(satellites == null){
            return;
        }
        Body focusScript = focus.GetComponent<Body>();
        SimpleShipScript shipScript = ship.GetComponent<SimpleShipScript>();


        this.ship = ship;

        foreach(GameObject satellite in satellites){
            Destroy(satellite);
        }
        
        int satelliteCount = focusScript.GetSatellites().Count;

        float radius;

        radius = focusScript.GetSystemRadius();

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

        Vector2 position = new Vector2(shipScript.GetPosition().x, shipScript.GetPosition().z);
        position = position / radius;
        position = position * mapWidth / 2;  
      
        shipDot.GetComponent<RectTransform>().localPosition = position;
        shipDot.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, -ship.transform.eulerAngles.y);

        //Move the satellite objects to a further position based on the zoom level
        foreach(GameObject satellite in satellites){
            if(satellite == null){ continue; }
            Vector3 offset = satellite.GetComponent<RectTransform>().localPosition - shipDot.GetComponent<RectTransform>().localPosition;
            satellite.transform.GetComponent<RectTransform>().localPosition = shipDot.GetComponent<RectTransform>().localPosition + offset * zoom;
            satellite.SetActive(true);
        }

        //Move center
        Vector2 centerPosition = -1f * position * (zoom - 1.0f);
        center.GetComponent<RectTransform>().localPosition = centerPosition;

        //Create graph
        radius *= 1000f;

        Vector2 r0_vec = new Vector2(shipScript.GetPosition().x, shipScript.GetPosition().z) * 1000f;
        Vector2 v0_vec = new Vector2(shipScript.GetVelocity().x, shipScript.GetVelocity().z) * 1000f;
        float x0 = r0_vec.x;
        float y0 = r0_vec.y;
        float vx0 = v0_vec.x;
        float vy0 = v0_vec.y;
        float rv_dot = Vector2.Dot(r0_vec,v0_vec);

        float G = 6.67f * Mathf.Pow(10f,-11f);
        float mass = focusScript.GetMass();
        float mu = G*mass;

        float r0 = r0_vec.magnitude;
        float v0 = v0_vec.magnitude;

        float h = x0*vy0-y0*vx0;

        Vector2 e_vec = (1/mu)*((v0*v0-mu/r0)*r0_vec-rv_dot*v0_vec);
        float e = e_vec.magnitude;

        float epsilon = v0*v0/2-mu/r0;

        float omega = Mathf.Atan2(e_vec[1],e_vec[0]);

        float p = h*h/mu;

        for(int i = 0; i < 300; i++){
            for(int j = 0; j < 300; j++){
                texture.SetPixel(i, j, Color.black);
            }
        }

        float lowerBound;
        float upperBound;

        //Debug.Log(omega);


        if(e > 1f){
            float bound = Mathf.Acos(-1/e);
            //Only count from -pi/2 to pi/2
            lowerBound = omega -1f * bound;
            //upperBound = omega + Mathf.PI/2;
            upperBound = omega + bound;
        }else{
            lowerBound = 0f;
            upperBound = 2f * Mathf.PI;
        }

        for(float theta = lowerBound; theta < upperBound; theta+=Mathf.PI/500){
            float r_theta = p/(1+e*Mathf.Cos(theta-omega));
            float rx = r_theta * Mathf.Cos(theta);
            float ry = r_theta * Mathf.Sin(theta);
            //rx/radius + scale / 2;
            float graphx = rx / radius*mapWidth/2 + mapWidth / 2f;
            float graphy = ry / radius*mapWidth/2 + mapWidth / 2f;
            if(graphx <= mapWidth && graphy <= mapWidth && graphx >= 0 && graphy >= 0){
                texture.SetPixel((int)graphx, (int)graphy, Color.white);
            }
        }
    
        texture.Apply();

    }   

    public void UpdateDisplay(){
        //Set ship dot to position relative to center
        //Set satellites relative to center
    }
}