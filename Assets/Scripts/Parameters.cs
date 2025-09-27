using UnityEngine;

public class Parameters
{
    private static float timeScale;
    private static float modelScale;
    private static float updateTime = 0.05f;

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
