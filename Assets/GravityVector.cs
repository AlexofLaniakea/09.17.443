using UnityEngine;


public class GravityVector
{
    private string name;
    private Vector3 vector;


    public GravityVector(string name, Vector3 vector){
        this.name = name;
        this.vector = vector;
    }
    
    public void setName(string name){
        this.name = name;
    }
    
    public void setVector(Vector3 vector){
        this.vector = vector;
    } 

    public string getName(){
        return name;
    }

    public Vector3 getVector(){
        return vector;
    }

}
