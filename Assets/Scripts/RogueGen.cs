using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueGen : LevelGenerator
{
    //We break the map into a 3x3 grid of room 'slots' to start
    //I keep them both in a dictionary (so I can find them by their X/Y coordinate)
    //  and in a master list (so I can foreach them easily)
    public Dictionary<int, Dictionary<int, RoomSlot>> Slots = new Dictionary<int, Dictionary<int, RoomSlot>>();
    public List<RoomSlot> AllSlots = new List<RoomSlot>();

    
    public List<Room> Rooms = new List<Room>();
    public List<Room> NonHalls = new List<Room>();
    public Dictionary<int, Dictionary<int, Tile>> Tiles = new Dictionary<int, Dictionary<int, Tile>>();
    public List<Tile> EmptyTiles = new List<Tile>();
    public RectInt Bounds= new RectInt(9999,9999,-9999,-9999);
    public RoomSlot PlayerRoom = null;
    public RoomSlot ExitRoom = null;
    
    public override IEnumerator BuildLevel(GameManager gm)
    {
        for (int x = 0;x <3;x++)
        for (int y = 0; y < 3; y++)
        {
            if (!Slots.ContainsKey(x))
                Slots.Add(x,new Dictionary<int, RoomSlot>());
            RoomSlot s = new RoomSlot(new Vector2Int(x * God.SlotSize.x, y * God.SlotSize.y));
            Slots[x].Add(y,s);
            AllSlots.Add(s);
        }
        for (int x = 0;x <3;x++)
        for (int y = 0; y < 3; y++)
        {
            if (x != 0)
            {
                Slots[x-1][y].Neighbors.Add(Directions.Right,Slots[x][y]);
                Slots[x][y].Neighbors.Add(Directions.Left,Slots[x-1][y]);
            }
            if (y != 0)
            {
                Slots[x][y-1].Neighbors.Add(Directions.Up,Slots[x][y]);
                Slots[x][y].Neighbors.Add(Directions.Down,Slots[x][y-1]);
            }
        }
        List<RoomSlot> unlinked = new List<RoomSlot>();
        unlinked.AddRange(AllSlots);
        PlayerRoom = unlinked[Random.Range(0,unlinked.Count)];
        unlinked.Remove(PlayerRoom);
        List<RoomSlot> active = new List<RoomSlot>(){PlayerRoom};
        int safety = 999;
        while (unlinked.Count > 0 && active.Count > 0 && safety > 0)    
        {
            safety--;
            RoomSlot a = active[active.Count - 1];
            List<Directions> maybe = new List<Directions>();
            foreach (Directions d in a.Neighbors.Keys)
            {
                RoomSlot rs = a.Neighbors[d];
                if (unlinked.Contains(rs))
                    maybe.Add(d);
            }

            if (maybe.Count > 0)
            {
                Directions chosen = maybe[Random.Range(0,maybe.Count)];
                a.Links.Add(chosen);
                a.Neighbors[chosen].Links.Add(God.OppositeDirection(chosen));
                active.Add(a.Neighbors[chosen]);
            }
            else
            {
                active.Remove(a);
                if (ExitRoom == null)
                    ExitRoom = a;
            }
        }

        if (ExitRoom == null)
            ExitRoom = active[active.Count - 1];

        foreach (RoomSlot rs in AllSlots)
        {
            int le = Random.Range(2, rs.Shape.width / 2+1);
            int ri = Random.Range(2, rs.Shape.width / 2 + 1);
            int up = Random.Range(2, rs.Shape.height / 2 + 1);
            int dwn = Random.Range(2, rs.Shape.height / 2 + 1);
//            le = rs.Shape.width / 2;
//            ri = le;
            
            Room r = BuildRoom(rs.Heart.x-le,rs.Heart.y-dwn,le+ri,up+dwn);
            rs.R = r;
            foreach (Directions dir in rs.Links)
            {
                if (dir == Directions.Left)
                {
                    BuildRoom(rs.Heart.x-God.SlotSize.x, rs.Heart.y - 1, God.SlotSize.x, 2,true);
                }
                if (dir == Directions.Down)
                {
                    BuildRoom(rs.Heart.x-1, rs.Heart.y - God.SlotSize.y, 2, God.SlotSize.y,true);
                }
            }
        }
        BuildWalls();
        // God.GM.Spawn(Things.Player, PlayerRoom.R.RandomTile());
        // for (int n = 0; n < 8; n++)
        // God.GM.Spawn(Things.Gear, PlayerRoom.R.RandomTile());
        // for (int n = 0; n < God.Roll(1, 3, 1); n++)
        //     God.GM.Spawn(Things.Gear, NonHalls.Random().RandomTile());
        // for (int n = 0; n < God.Roll(2, 3, 1); n++)
        //     God.GM.Spawn(Things.Gold, NonHalls.Random().RandomTile());
        //
        // Dictionary<MonsterEntry, float> monsters = Library.MonsterProfile(lvl);
        foreach (Room rm in NonHalls)
        {
            // if (rm != PlayerRoom.R)
            // {
            //     for (int n = 0; n < God.Roll(2, 4, -2); n++)
            //         God.GM.Spawn(God.WeightedRandom(monsters).Type, rm.RandomTile());
            //     
            // }
            //
            // if (rm == ExitRoom.R)
            // {
            //     Actor exit = God.GM.Spawn(lvl == God.Levels ? Things.Amulet : Things.Stairs, rm.RandomTile());
            //     Debug.Log("EXIT: " + exit.transform.position);
            // }
        }
        yield return null;
    }

    public Room BuildRoom(int x, int y, int w, int h,bool hall=false)
    {
        Room r = new Room(this,new RectInt(x,y,w,h),hall);
        Rooms.Add(r);
        if (!hall)
            NonHalls.Add(r);
        return r;
    }

    public void AddTile(int x, int y,Room r=null,bool wall=false)
    {
        Tile t = new Tile(r,this,x,y);
        t.Wall = wall;
        if (!Tiles.ContainsKey(t.Loc.x))
            Tiles.Add(x,new Dictionary<int, Tile>());
        if (Tiles[x].ContainsKey(t.Loc.y))
        {
            if (Tiles[x][y].Wall && !wall)
                Tiles[x][y].Wall = false;
//            else
//                Debug.Log("DOUBLE TILE ADD: " + x + " / " + y + " / " + r);
            return;
        }
        Tiles[x].Add(t.Loc.y,t);
        EmptyTiles.Add(t);
        if (r != null)
        {
            r.Tiles.Add(t);
            r.EmptyTiles.Add(t);
        }
        Bounds.x = Mathf.Min(Bounds.x, x);
        Bounds.y = Mathf.Min(Bounds.y, y);
        Bounds.width = Mathf.Max(Bounds.width, x);
        Bounds.height = Mathf.Max(Bounds.height, y);
    }

    public Tile GetTile(int x, int y)
    {
        if (!Tiles.ContainsKey(x) || !Tiles[x].ContainsKey(y))
            return null;
        return Tiles[x][y];
    }
    
    public Tile GetTile(Vector2 p)
    {
        return GetTile(Mathf.FloorToInt(p.x+0.5f),Mathf.FloorToInt(p.y+0.5f));
    }
    
    public Tile GetTile(Vector2Int p)
    {
        return GetTile(p.x, p.y);
    }

    public void BuildWalls()
    {
        List<Vector2Int> xWall = new List<Vector2Int>();
        Dictionary<int,List<Vector2Int>> yWalls = new Dictionary<int, List<Vector2Int>>();
        for (int y = Bounds.y; y < Bounds.height + 2; y++)
        for (int x = Bounds.x; x < Bounds.width + 2; x++)
        {
            Tile t = GetTile(x, y);
//            Debug.Log("T: " + x + "." + y + " / " + t + " / " + t?.Wall);
            if (t == null || !t.Wall)
            {
                if (xWall.Count > 0)
                {
                    Vector2Int s = xWall[0];
                    Vector2Int e = xWall[xWall.Count-1];
                    RectInt shape = new RectInt(s.x,s.y,e.x-s.x+1,e.y-s.y+1);
//                    Debug.Log("XW: " + s + " / " + e + " / " + shape);
                    God.GM.SpawnShape(shape,TileTypes.Wall);
                    xWall.Clear();
                }

                if (yWalls.ContainsKey(x) && yWalls[x].Count > 1)
                {
                    Vector2Int s = yWalls[x][0];
                    Vector2Int e = yWalls[x][yWalls[x].Count-1];
                    RectInt shape = new RectInt(s.x,s.y,e.x-s.x+1,e.y-s.y+1);
//                    Debug.Log("YW: " + s + " / " + e + " / " + shape);
                    God.GM.SpawnShape(shape,TileTypes.Wall);
                }
                yWalls.Remove(x);
                continue;
            }
            if (!yWalls.ContainsKey(x))
                xWall.Add(new Vector2Int(x,y));
            if (!yWalls.ContainsKey(x))
                yWalls.Add(x,new List<Vector2Int>());
            yWalls[x].Add(new Vector2Int(x,y));
        }
    }
}

public class RoomSlot
{
    public RogueGen D;
    public RectInt Shape;
    public Room R;
    public Dictionary<Directions, RoomSlot> Neighbors = new Dictionary<Directions, RoomSlot>();
    public List<Directions> Links = new List<Directions>();
    public Vector2Int Heart;

    public RoomSlot(Vector2Int heart)
    {
        Heart = heart;
        Shape = new RectInt(heart.x-(God.SlotSize.x/2),heart.y-(God.SlotSize.y/2),God.SlotSize.x,God.SlotSize.y);
    }
}

public class Room
{
    public RogueGen D;
    public RectInt Shape;
    public List<Tile> Tiles = new List<Tile>();
    public List<Tile> EmptyTiles = new List<Tile>();
    public bool Hall;

    public Room(RogueGen d,RectInt shape, bool hall)
    {
        Hall = hall;
        Shape = shape;
        D = d;
        List<Vector2Int> shp = new List<Vector2Int>();
        for (int y = Shape.y; y < Shape.y + Shape.height; y++)
        {
            for (int x = Shape.x; x < Shape.x + Shape.width; x++)
            {
                D.AddTile(x,y,this);
                shp.Add(new Vector2Int(x,y));
            }
        }
        foreach(Vector2Int p in God.Outline(shp.ToArray()))
            D.AddTile(p.x,p.y,null,true);
    }

    public Tile RandomTile(bool empty=true)
    {
        return (empty ? EmptyTiles : Tiles).Random();
    }
}

public class Tile
{
    public RogueGen D;
    public Room R;
    public Vector2Int Loc;
    public bool Wall;
    // public Actor Contents;

    public Tile(Room r,RogueGen d,int x, int y)
    {
        Loc = new Vector2Int(x,y);
        D = d;
        R = r;
    }
}