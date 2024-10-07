using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGen : LevelGenerator
{   
    public override IEnumerator BuildLevel(GameManager gm)
    {
        List<Vector2Int> floors = new List<Vector2Int>();
        int right = Random.Range(1, Mathf.CeilToInt(gm.LevelSize.x / 2f));
        int left = -Random.Range(1, Mathf.CeilToInt(gm.LevelSize.x / 2f));
        int up = Random.Range(1, Mathf.CeilToInt(gm.LevelSize.y / 2f));
        int down = -Random.Range(1, Mathf.CeilToInt(gm.LevelSize.y / 2f));
        for (int x = left; x <= right; x++)
        for (int y = down; y <= up; y++)
        {
            floors.Add(new Vector2Int(x,y));
            AddTile(x,y,TileTypes.Floor);
        }

        God.GM.SetStatus("Building Floors");
        
        foreach (Vector2Int f in floors)
        {
            gm.SpawnTile(f,TileTypes.Floor);
            yield return gm.StartCoroutine(Pause());
        }

        yield return gm.StartCoroutine(WaitForPress("Press space to build walls. . ."));
        God.GM.SetStatus("Building Walls");
        
        List<Vector2Int> walls = FindWalls();
        
        foreach (Vector2Int f in walls)
        {
            AddTile(f.x,f.y,TileTypes.Wall);
            gm.SpawnTile(f,TileTypes.Wall);
            yield return gm.StartCoroutine(Pause());
        }
        yield return gm.StartCoroutine(WaitForPress("Build complete!"));
        God.GM.SetStatus("");

    }
}
