using UnityEngine;

public class State
{
    private static int gameState = 0;//0 menu, 1 playing, 2 paused

    public static void SetState(int gs){
        gameState = gs;
    }

    public static int GetState(){
        return gameState;
    }
}
