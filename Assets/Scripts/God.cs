using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class God
{
    public static float GameSpeed = 1 / 20f;

    public static Vector2Int SlotSize = new Vector2Int(9, 9);
    
    public static PlayerMovement Player;
    public static Camera Cam;
    public static GameManager GM;

    public static Directions[] Cardinal = new[] {Directions.Up, Directions.Right, Directions.Down, Directions.Left}; 
    public static Directions[] EightDir = new[] {Directions.Up, Directions.Right, Directions.Down, Directions.Left,Directions.UR,Directions.UL,Directions.DL,Directions.DR};

    
    public static List<Vector2Int> CardinalDirs = new List<Vector2Int>()
        { new Vector2Int(-1,0),new Vector2Int(1,0),new Vector2Int(0,-1),new Vector2Int(0,1) };
    public static List<Vector2Int> EightDirs = new List<Vector2Int>()
    {
        new Vector2Int(-1,0),new Vector2Int(1,0),new Vector2Int(0,-1),new Vector2Int(0,1),
        new Vector2Int(-1,-1),new Vector2Int(1,-1),new Vector2Int(-1,1),new Vector2Int(1,1)
    };
    
    public static bool CoinFlip(float odds=0.5f)
    {
        return UnityEngine.Random.Range(0,1f) < odds;
    }
    
    public static Vector2Int Dir2Point(Directions d)
    {
        switch (d) {
            case Directions.Down:
                return new Vector2Int (0, -1);
            case Directions.Up:
                return new Vector2Int (0, 1);
            case Directions.Left:
                return new Vector2Int (-1, 0);
            case Directions.Right:
                return new Vector2Int (1, 0);
            case Directions.UL:
                return new Vector2Int (-1, 1);
            case Directions.UR:
                return new Vector2Int (1, 1);
            case Directions.DL:
                return new Vector2Int (-1, -1);
            case Directions.DR:
                return new Vector2Int (1, -1);
        }
        return new Vector2Int(0,0);
    }
    
    public static List<Vector2Int> Outline(params Vector2Int[] shape)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        foreach (Vector2Int p in shape)
        {
            foreach (Directions dir in God.EightDir)
            {
                Vector2Int w = p + God.Dir2Point(dir);
                if (!shape.Contains(w) && !r.Contains(w))
                    r.Add(w);
            }
        }
        return r;
    }
    
    public static Directions OppositeDirection(Directions d)
    {
        Vector3 v = new Vector2();
        v.Test();
        switch (d)
        {
            case Directions.Up: return Directions.Down;
            case Directions.Down: return Directions.Up;
            case Directions.Left: return Directions.Right;
            case Directions.Right: return Directions.Left;
        }

        return Directions.None;
    }

    
    //Extends lists to return a random entry
    public static T Random<T>(this List<T> l) where T:class
    {
        if (l.Count == 0)
            return null;
        return l[UnityEngine.Random.Range(0, l.Count)];
    }

    //Extends lists to shuffle randomly
    public static List<T> Shuffle<T>(this List<T> l) where T:class
    {
        if (l.Count == 0)
            return l;
        List<T> temp = new List<T>();
        temp.AddRange(l);
        List<T> r = new List<T>();
        while (temp.Count > 0)
        {
            T chosen = temp.Random();
            temp.Remove(chosen);
            r.Add(chosen);
        }
        return r;
    }
}

public enum Directions{
    None,
    Up,
    Down,
    Left,
    Right,
    UL,
    UR,
    DL,
    DR
}