using UnityEngine;

public class NeighborEdge
{
    private GameObject body;
    private float distance;
    private float angle;
    
    public NeighborEdge(GameObject body, float distance, float angle){
        this.body = body;
        this.distance = distance;
        this.angle = angle;
    }

    public GameObject GetBody(){ return body; }
    public float GetDistance(){ return distance; }
    public float GetAngle(){ return angle; }
}
