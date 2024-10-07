using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class God
{
    public static float GameSpeed = 1 / 20f;
    
    public static PlayerMovement Player;
    public static Camera Cam;
    public static GameManager GM;

    public static List<Vector2Int> CardinalDirs = new List<Vector2Int>()
        { new Vector2Int(-1,0),new Vector2Int(1,0),new Vector2Int(0,-1),new Vector2Int(0,1) };
    public static List<Vector2Int> EightDirs = new List<Vector2Int>()
    {
        new Vector2Int(-1,0),new Vector2Int(1,0),new Vector2Int(0,-1),new Vector2Int(0,1),
        new Vector2Int(-1,-1),new Vector2Int(1,-1),new Vector2Int(-1,1),new Vector2Int(1,1)
    };
}
