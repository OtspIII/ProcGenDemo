using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator
{
    public Dictionary<int, Dictionary<int, TileTypes>> Map = new Dictionary<int, Dictionary<int, TileTypes>>();
    
    public virtual IEnumerator BuildLevel(GameManager gm)
    {
        yield return null;
    }

    public void AddTile(int x, int y, TileTypes type)
    {
        if(!Map.ContainsKey(x)) Map.Add(x,new Dictionary<int, TileTypes>());
        if (Map[x].ContainsKey(y)) Map[x][y] = type;
        else Map[x].Add(y,type);
    }

    public TileTypes GetTile(int x, int y)
    {
        if (!Map.ContainsKey(x)) return TileTypes.None;
        if (!Map[x].ContainsKey(y)) return TileTypes.None;
        return Map[x][y];
    }

    public List<Vector2Int> GetEmptyAdj(int x, int y)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        foreach (Vector2Int d in God.EightDirs)
        {
            TileTypes c = GetTile(x + d.x, y + d.y);
            if(c == TileTypes.None)
                r.Add(new Vector2Int(x+d.x,y+d.y));
        }

        return r;
    }

    public List<Vector2Int> FindWalls()
    {
        List<Vector2Int> r = new List<Vector2Int>();
        foreach (int x in Map.Keys)
        {
            foreach (int y in Map[x].Keys)
            {
                foreach (Vector2Int adj in GetEmptyAdj(x, y))
                {
                    if (!r.Contains(adj))
                    {
                        r.Add(adj);
                    }
                }
            }
        }
        return r;
    }

    public IEnumerator WaitForPress(string txt="Press space to continue. . .")
    {
        if(txt != "")
            God.GM.SetStatus(txt);
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
        yield return null;
    }
}

public enum TileTypes
{
    None=0,
    Floor=1,
    Wall=2,
}