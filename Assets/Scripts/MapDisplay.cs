using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapDisplay : MonoBehaviour
{
    public GameObject gb;
    public Image spriteRenderer;
    public Texture2D texture;


    public GameObject shipDot;
    public GameObject satellitePrefab;
    public GameObject center;

    float mapWidth = 300f;

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
        spriteRenderer = GetComponent<Image>();

        // ✅ Create a new texture for drawing
        texture = new Texture2D(300, 300, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;

        // Fill background black
        Color[] fill = new Color[texture.width * texture.height];
        for (int i = 0; i < fill.Length; i++) fill[i] = Color.black;
        texture.SetPixels(fill);

        // Example: draw a vertical white line
        for (int i = 0; i < texture.height; i++)
        {
            texture.SetPixel(100, i, Color.white);
        }

        texture.Apply();

        // ✅ Wrap in a Sprite and assign to UI Image
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f
        );

        spriteRenderer.sprite = sprite;

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
        position = new Vector2(shipScript.GetPosition().x, shipScript.GetPosition().z);
        Vector2 velocity = new Vector2(shipScript.GetVelocity().x, shipScript.GetVelocity().z);
        
        float pMag= position.magnitude;
        float vMag = velocity.magnitude;
        float mu = focusScript.GetMass() * 6.67f*Mathf.Pow(10f,-11f);

        float h= position.x * velocity.y - position.y * velocity.x;
        Vector2 eVector = (1f/mu)*((Mathf.Pow(vMag,2)-mu/pMag)*position-Vector2.Dot(position,velocity)*velocity);
        float e = eVector.magnitude;

        float semiMaj = (vMag*vMag)/2f - mu/pMag;

        float a = -mu/(2f*semiMaj);

        for (int i = 0; i < texture.height; i++)
        {
            for(int j = 0; j < texture.width; j++){
                texture.SetPixel(j, i, Color.black);
            }
        }

        float omega = Mathf.Atan2(eVector.y, eVector.x);
        for(float i = 0; i < 2*Mathf.PI; i+=Mathf.PI/100){
            //Plot graph for each angle
            float r = (h*h/mu) / (1f + e * Mathf.Cos(i - omega));
            float scale = mapWidth / (2f * (float)(a * (1 + e))); // roughly size orbit to screen
            int x = (int)(r * Mathf.Cos(i) + mapWidth/2);
            int y = (int)(r * Mathf.Sin(i) + mapWidth/2);
            //Debug.Log(x+","+y);
            texture.SetPixel(x,y, Color.white);
        }
        for(int i = 0; i < 150; i++){
            texture.SetPixel(100,i,Color.white);
        }
        texture.Apply();

    }   

    public void UpdateDisplay(){
        //Set ship dot to position relative to center
        //Set satellites relative to center
    }
}
