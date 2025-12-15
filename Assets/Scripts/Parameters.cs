using UnityEngine;

public class Parameters
{
    private static float timeScale;
    private static float modelScale = 1000f;//Meters
    private static float updateTime = 0.02f;

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

    public static void SetUpdateTime(float ut)
    {
        updateTime = ut;
    }

    public static float GetUpdateTime(){
        return updateTime;
    }
}
