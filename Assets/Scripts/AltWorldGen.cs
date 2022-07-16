using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;








public class AltWorldGen : MonoBehaviour
{
    private struct Tunnel
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

    private struct Room
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

    public Tile BlankTile;
    public Tile ColouredTile;
    public Tile LineTile;

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
    List<Room> mRooms = new List<Room>();
    List<Tunnel> mTunnels = new List<Tunnel>();
    List<Tile> mTilesToRedraw = new List<Tile>();
    List<Vector3Int> mPositionsToRedraw = new List<Vector3Int>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    [ContextMenu("redo")]
    void GenerateMap()
    {
        mTunnels.Clear();
        mRooms.Clear();
        mTilesToRedraw.Clear();
        mPositionsToRedraw.Clear();
        WorldTilemap.ClearAllTiles();

        GenerateRooms();

        foreach (Room r in mRooms)
        {
            for (int x = 0; x < r.Size.x; x++)
            {
                for (int y = 0; y < r.Size.y; y++)
                {
                    mPositionsToRedraw.Add(new Vector3Int(r.Position.x + x, r.Position.y + y, 0));
                    mTilesToRedraw.Add(ColouredTile);
                }
            }
        }

        WorldTilemap.SetTiles(mPositionsToRedraw.ToArray(), mTilesToRedraw.ToArray());
    }

    private void GenerateRooms()
    {
        mRoomCount = RNGCore.Instance.RandomRoll(MinRoomCount, MaxRoomCount);
        for (int i = 0; i < mRoomCount; i++)
        {
            int roomWidth = RNGCore.Instance.RandomRoll(MinRoomSize, MaxRoomSize);
            int roomHeight = RNGCore.Instance.RandomRoll(MinRoomSize, MaxRoomSize);

            int roomPosX = RNGCore.Instance.RandomRoll(0, MapSize - roomWidth - 1);
            int roomPosY = RNGCore.Instance.RandomRoll(0, MapSize - roomHeight - 1);

            Room newRoom = new Room(new Vector2Int(roomPosX, roomPosY), new Vector2Int(roomWidth, roomHeight));

            //Does this room intersect with any existing rooms?
            bool hasOverlap = false;
            foreach (Room r in mRooms)
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


            mPositionsToRedraw.Add(new Vector3Int(newRoom.Center.x, newRoom.Center.y, 2));
            mTilesToRedraw.Add(LineTile);

            if (mRooms.Count == 0)
            {
                startRoom = newRoom;
            }
            else
            {
                GenerateTunnel(newRoom, mRooms[mRooms.Count - 1]);
            }

            mRooms.Add(newRoom);
        }
    }


    private void GenerateTunnel(Room A, Room B)
    {
        Vector2Int start = A.Center;
        Vector2Int end = B.Center;
        Vector2Int corner = new Vector2Int();

        if (RNGCore.Instance.CoinFlip())
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

        mTunnels.Add(new Tunnel(A, start, B, end));
    }

    void plotLine(int x0, int y0, int x1, int y1)
    {
        int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = -Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx + dy, e2; /* error value e_xy */

        for (; ; )
        {  /* loop */
            mPositionsToRedraw.Add(new Vector3Int(x0,y0, 2));
            mTilesToRedraw.Add(LineTile);

            if (x0 == x1 && y0 == y1) break;
            e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; } /* e_xy+e_x > 0 */
            if (e2 <= dx) { err += dx; y0 += sy; } /* e_xy+e_y < 0 */
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
