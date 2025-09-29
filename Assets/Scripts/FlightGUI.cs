using UnityEngine;

public class FlightGUI : MonoBehaviour
{
    public GameObject gb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gb.SetActive(State.GetState() == 1);
    }
}
