using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public LevelGenMethod Method;
    public Vector2Int LevelSize;

    public TextMeshProUGUI Status;
    
    public GameObject FloorPrefab;
    public GameObject WallPrefab;
    
    public Dictionary<int, Dictionary<int, bool>> Map = new Dictionary<int, Dictionary<int, bool>>();

    private void Awake()
    {
        God.GM = this;
        God.Cam = Camera.main;
    }

    void Start()
    {
        God.Cam.orthographicSize = Mathf.Max(LevelSize.x / 2f,LevelSize.y * 0.8f);
        LevelGenerator gen = null;
        switch (Method)
        {
            case LevelGenMethod.Simple:{ gen = new SimpleGen(); break; }
            case LevelGenMethod.Rogue:{ gen = new RogueGen(); break; }
            default:
            {
                Debug.Log("INVALID LEVEL GEN METHOD: " + Method);
                break;
            }
        }
        if(gen != null) StartCoroutine(gen.BuildLevel(this));
    }

    public void SpawnTile(Vector2Int where,TileTypes type)
    {
        GameObject pref = null;
        switch (type)
        {
            case TileTypes.Floor: { pref = FloorPrefab; break; }
            case TileTypes.Wall: { pref = WallPrefab; break; }
            default:
            {
                Debug.Log("INVALID TILE TYPE: " + type);
                break;
            }
        }
        Instantiate(pref, new Vector3(where.x,where.y,50), Quaternion.identity);
    }

    public void SetStatus(string txt)
    {
        Status.text = txt;
    }
}

public enum LevelGenMethod
{
    None=0,
    Simple=1,
    Rogue=2,
}