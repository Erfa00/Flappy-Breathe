using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData 
{
    public int score;

    public PlayerData (Game game)
    {
        score = game.score;
    }
}
