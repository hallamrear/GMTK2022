using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class WorldGeneration : MonoBehaviour
{
    public struct Tunnel
    {
        public Room Start;
        public Room End;
        public Vector2Int StartPosition;
        public Vector2Int EndPosition;

        public Tunnel(Room start, Vector2Int startPos, Room end, Vector2Int endPos)
        {
            Start = start;
            End = end;
            StartPosition = startPos;
            EndPosition = endPos;
        }
    }

    public struct Room
    {
        public Vector2Int Position;
        public Vector2Int Size;
        public Vector2Int Center;

        public Vector2Int InnerTL { get; }
        public Vector2Int InnerBR { get; }

        public Room(Vector2Int position, Vector2Int size)
        {
            Position = position;
            Size = size;
            Center = Position + (Size / 2);
            InnerTL = new Vector2Int(Position.x + size.x + 1, Position.y + size.y + 1);
            InnerBR = new Vector2Int(Position.x + 1, Position.y + 1);
        }


        public bool Overlaps(Room other)
        {
            Rect rectThis = new Rect(Position.x, Position.y, Size.x, Size.y);
            Rect rectOther = new Rect(other.Position.x, other.Position.y, other.Size.x, other.Size.y);

            return rectThis.Overlaps(rectOther);
        }

    }

    [Serializable]
    public struct Chest
    {
        public Vector3Int Position;
        Room associatedRoom;
        [SerializeField] private List<GameObject> mItems;

        public Chest(Room room, Vector3Int position, ChestDropTable_SO table)
        {
            mItems = new List<GameObject>();
            Position = position;
            associatedRoom = room;

            GenerateItems(table);
        }

        public void Clear()
        {
            for (int i = 0; i < mItems.Count; i++)
            {
                Destroy(mItems[i].gameObject);
            }

            mItems.Clear();
        }

        void GenerateItems(ChestDropTable_SO table)
        {
            int count = RNGCore.RandomRoll(1, 3);

            int index = -1;

            for (int i = 0; i < count; i++)
            {
                int percent = RNGCore.RandomRoll(1, 100);
                
                if(percent <= 20)
                {
                    //add bomb
                    index = 0;
                }
                else if (percent > 20 && percent <= 40)
                {
                    //add reroll
                    index = 1;
                }
                else if (percent > 40 && percent <= 60)
                {
                    //add fan
                    index = 2;
                }
                else if (percent > 60 && percent <= 80)
                {
                    //add potion
                    index = 3;
                }
                else
                {
                    //add ladder
                    index = 4;
                }

                mItems.Add(table.Items[index]);
            }
        }

        public List<GameObject> GetItems()
        {
            return mItems;
        }
    }

    public Tile ExitTile;
    public Tile DoorTile;
    public Tile TunnelTile;
    public Tile WallTile;
    public Tile FloorTile;
    public Tile mClosedChestTile;
    public Tile mOpenChestTile;
    public Tilemap WorldTilemap;

    [Range(64, 512)]
    public int MapSize;

    [Range(2, 32)]
    public int MaxRoomSize;
    [Range(2, 32)]
    public int MinRoomSize;

    [Range(2, 16)]
    public int MaxRoomCount;
    [Range(2, 16)]
    public int MinRoomCount;
    private int mRoomCount;

    private Room startRoom;
    private Room endRoom;
    public List<Room> Rooms = new List<Room>();
    public List<Chest> Chests = new List<Chest>();
    public List<Tunnel> Tunnels = new List<Tunnel>();
    public List<Vector3Int> Doors = new List<Vector3Int>();

    [SerializeField] ChestDropTable_SO mChestDropTable;

    // Start is called before the first frame update
    void Awake()
    {
        GenerateMap();
    }
    private void GenerateRoomObjects()
    {
        foreach (Room room in Rooms)
        {
            int chestX = 0, chestY = 0;

            int chestCount = RNGCore.RandomRoll(0, 3);

            for (int i = 0; i < chestCount; i++)
            {
                chestX = RNGCore.RandomRoll(room.InnerTL.x + 1, room.InnerBR.x - 1);
                chestY = RNGCore.RandomRoll(room.InnerBR.y + 1, room.InnerTL.y - 1);

                Chests.Add(new Chest(room, new Vector3Int(chestX, chestY, 0), mChestDropTable));
                WorldTilemap.SetTile(new Vector3Int(chestX, chestY, 0), mClosedChestTile);
            }
        }
    }

    public Vector3Int GetStartRoomPosition()
    {
        return new Vector3Int(startRoom.Center.x, startRoom.Center.y, 0);
    }

    [ContextMenu("gen")]
    public void GenerateMap()
    {
        Doors.Clear();
        Tunnels.Clear();
        Rooms.Clear();
        foreach (var c in Chests)
        {
            c.Clear();
        }

        WorldTilemap.ClearAllTiles();

        GenerateRooms();

        foreach (Tunnel tunnel in Tunnels)
        {
            DrawTunnel(tunnel);
        }

        foreach (Room room in Rooms)
        {
            DrawRoom(room);
        }

        foreach (Vector3Int door in Doors)
        {
            WorldTilemap.SetTile(door, DoorTile);
        }

        GenerateRoomObjects();

        CleanupBadGeneration();

        FindObjectOfType<GridMovement>().SetTilePosition(GetStartRoomPosition());
    }

    void CleanupBadGeneration()
    {
        Vector3Int position;
        List<Chest> toRemove = new List<Chest>();

        foreach (Chest chest in Chests)
        {
            position = chest.Position;

            if (WorldTilemap.GetTile(position + Vector3Int.up) == null ||
                WorldTilemap.GetTile(position + Vector3Int.down) == null ||
                WorldTilemap.GetTile(position + Vector3Int.left) == null ||
                WorldTilemap.GetTile(position + Vector3Int.right) == null
                )
            {
                if (
                WorldTilemap.GetTile(position + Vector3Int.up) == DoorTile ||
                WorldTilemap.GetTile(position + Vector3Int.down) == DoorTile ||
                WorldTilemap.GetTile(position + Vector3Int.left) == DoorTile ||
                WorldTilemap.GetTile(position + Vector3Int.right) == DoorTile)
                {
                    WorldTilemap.SetTile(position, FloorTile);
                }


                if ((WorldTilemap.GetTile(position + Vector3Int.up) == WallTile &&
                    WorldTilemap.GetTile(position + Vector3Int.down) == WallTile) ||
                    (WorldTilemap.GetTile(position + Vector3Int.left) == WallTile &&
                    WorldTilemap.GetTile(position + Vector3Int.right) == WallTile))
                {
                    WorldTilemap.SetTile(position, WallTile);
                }
                else
                {
                    WorldTilemap.SetTile(position, null);
                }


               toRemove.Add(chest);
            }
        }

        for (int i = 0; i < toRemove.Count; i++)
        {
            Chests.Remove(toRemove[i]);
            toRemove[i].Clear();
        }

        toRemove.Clear();
    }


    private void DrawRoom(Room room)
    {
        for (int rOuterX = 0; rOuterX <= room.Size.x; rOuterX++)
        {
            if (WorldTilemap.GetTile(new Vector3Int(room.Position.x + rOuterX, room.Position.y, 0)) != TunnelTile)
                WorldTilemap.SetTile(new Vector3Int(room.Position.x + rOuterX, room.Position.y, 0), WallTile);
            else
                Doors.Add(new Vector3Int(room.Position.x + rOuterX, room.Position.y, 0));

            if (WorldTilemap.GetTile(new Vector3Int(room.Position.x + rOuterX, room.Position.y + room.Size.y, 0)) != TunnelTile)
                WorldTilemap.SetTile(new Vector3Int(room.Position.x + rOuterX, room.Position.y + room.Size.y, 0), WallTile);
            else
                Doors.Add(new Vector3Int(room.Position.x + rOuterX, room.Position.y + room.Size.y, 0));
        }

        for (int rOuterY = 0; rOuterY < room.Size.y; rOuterY++)
        {
            if (WorldTilemap.GetTile(new Vector3Int(room.Position.x, room.Position.y + rOuterY, 0)) != TunnelTile)
                WorldTilemap.SetTile(new Vector3Int(room.Position.x, room.Position.y + rOuterY, 0), WallTile);
            else
                Doors.Add(new Vector3Int(room.Position.x, room.Position.y + rOuterY, 0));

            if (WorldTilemap.GetTile(new Vector3Int(room.Position.x + room.Size.x, room.Position.y + rOuterY, 0)) != TunnelTile)
                WorldTilemap.SetTile(new Vector3Int(room.Position.x + room.Size.x, room.Position.y + rOuterY, 0), WallTile);
            else
                Doors.Add(new Vector3Int(room.Position.x + room.Size.x, room.Position.y + rOuterY, 0));
        }

        for (int innerX = 1; innerX < room.Size.x; innerX++)
        {
            for (int innerY = 1; innerY < room.Size.y; innerY++)
            {
                WorldTilemap.SetTile(new Vector3Int(room.Position.x + innerX, room.Position.y + innerY, 0), FloorTile);
            }
        }

        if(room.Center == endRoom.Center)
            WorldTilemap.SetTile(new Vector3Int(room.Center.x, room.Center.y, 0), ExitTile);
    }

    private void GenerateRooms()
    {
        mRoomCount = RNGCore.RandomRoll(MinRoomCount, MaxRoomCount);
        int endRoomIndex = RNGCore.RandomRoll(1, mRoomCount);

        for (int i = 0; i < mRoomCount; i++)
        {
            int roomWidth = RNGCore.RandomRoll(MinRoomSize, MaxRoomSize);
            int roomHeight = RNGCore.RandomRoll(MinRoomSize, MaxRoomSize);

            int roomPosX = RNGCore.RandomRoll(0, MapSize - roomWidth - 1);
            int roomPosY = RNGCore.RandomRoll(0, MapSize - roomHeight - 1);

            Room newRoom = new Room(new Vector2Int(roomPosX, roomPosY), new Vector2Int(roomWidth, roomHeight));

            //Does this room intersect with any existing rooms?
            bool hasOverlap = false;
            foreach (Room r in Rooms)
            {
                if (newRoom.Overlaps(r))
                {
                    hasOverlap = true;
                }
            }

            if (hasOverlap)
            {
                continue;
            }

            if(i == endRoomIndex)
            {
                endRoom = newRoom;
            }

            if (Rooms.Count == 0)
            {
                startRoom = newRoom;
            }
            else
            {
                Vector2Int start = newRoom.Center;
                Vector2Int end = Rooms[Rooms.Count - 1].Center;
                Tunnels.Add(new Tunnel(newRoom, start, Rooms[Rooms.Count - 1], end));
            }

            Rooms.Add(newRoom);
        }
    }

    private void DrawTunnel(Tunnel tunnel)
    {
        Vector2Int start = tunnel.Start.Center;
        Vector2Int end = tunnel.End.Center;
        Vector2Int corner = new Vector2Int();

        if (RNGCore.CoinFlip())
        {
            //move horizontally first
            corner.x = end.x;
            corner.y = start.y;
        }
        else
        {
            //move vertically first
            corner.x = start.x;
            corner.y = end.y;           
        }

        plotLine(start.x, start.y, corner.x, corner.y);
        plotLine(corner.x, corner.y, end.x, end.y);
    }

    void plotLine(int x0, int y0, int x1, int y1)
    {
        int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = -Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx + dy, e2; /* error value e_xy */

        for (; ; )
        {  /* loop */
            WorldTilemap.SetTile(new Vector3Int(x0, y0, 0), TunnelTile);

            if (x0 == x1 && y0 == y1) break;
            e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; } /* e_xy+e_x > 0 */
            if (e2 <= dx) { err += dx; y0 += sy; } /* e_xy+e_y < 0 */
        }
    }
}
