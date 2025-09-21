using UnityEngine;

public class Parameters
{
    private static float timeScale;
    private static float modelScale;

    public static void setTimeScale(float ts)
    {
        timeScale = ts;
    }

    public static float getTimeScale(){
        return timeScale;
    }

    public static void SetModelScale(float ms)
    {
        modelScale = ms;
    }

    public static float GetModelScale(){
        return modelScale;
    }
}
