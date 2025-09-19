using UnityEngine;

public class Parameters
{
    private static float timeScale;

    public static void setTimeScale(float ts)
    {
        timeScale = ts;
    }

    public static float getTimeScale(){
        return timeScale;
    }
}
