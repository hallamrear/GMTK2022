using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGen : MonoBehaviour
{
    public Tile BlankTile;
    public Tile ColouredTile;
    public Tile LineTile;

    public Tilemap WorldTilemap;

    [Range(8, 64)]
    public int MaxDistanceFromZeroZero;

    [Range(2, 32)]
    public int MaxRoomSize;
    [Range(2, 32)]
    public int MinRoomSize;

    [Range(2, 16)]
    public int MaxRoomCount;
    [Range(2, 16)]
    public int MinRoomCount;
    private int mRoomCount;

    private List<Vector2> mRoomPositions;
    List<Rect> rects = new List<Rect>();

    private void OnValidate()
    {
        if(MaxRoomCount < MinRoomCount + 1)
        {
            MaxRoomCount = MinRoomCount + 1;
        }

        if (MaxRoomSize < MinRoomSize + 1)
        {
            MaxRoomSize = MinRoomSize + 1;
        }
    }

    private void Start()
    {
        mRoomPositions = new List<Vector2>();
        GenerateRooms();

    }

    [ContextMenu("redo")]
    private void GenerateRooms()
    {
        GeneratePoints();
        GenerateLinesBetweenRooms();
        EnlargeRooms();
    }

    // a1 is line1 start, a2 is line1 end, b1 is line2 start, b2 is line2 end
    static bool Intersects(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersection)
    {
        intersection = Vector2.zero;

        Vector2 b = a2 - a1;
        Vector2 d = b2 - b1;
        float bDotDPerp = b.x * d.y - b.y * d.x;

        // if b dot d == 0, it means the lines are parallel so have infinite intersection points
        if (bDotDPerp == 0)
            return false;

        Vector2 c = b1 - a1;
        float t = (c.x * d.y - c.y * d.x) / bDotDPerp;
        if (t < 0 || t > 1)
            return false;

        float u = (c.x * b.y - c.y * b.x) / bDotDPerp;
        if (u < 0 || u > 1)
            return false;

        intersection = a1 + t * b;

        return true;
    }

    private void GeneratePoints()
    {
        mRoomCount = 0;
        mRoomPositions.Clear();
        rects.Clear();

        mRoomCount = RNGCore.Instance.RandomRoll(MinRoomCount, MaxRoomCount);

        for(int i = 0; i < mRoomCount; i++)
        {
            Vector2 position = new Vector2();
            position.x = RNGCore.Instance.RandomRoll(-MaxDistanceFromZeroZero, MaxDistanceFromZeroZero);
            position.y = RNGCore.Instance.RandomRoll(-MaxDistanceFromZeroZero, MaxDistanceFromZeroZero);

            mRoomPositions.Add(position);
            WorldTilemap.SetTile(new Vector3Int((int)position.x, (int)position.y, 0), ColouredTile);
        }
    }

    private void walk_grid(Vector2 p0, Vector2 p1)
    {
        float dx = p1.x - p0.x, dy = p1.y - p0.y;
        float nx = Math.Abs(dx), ny = Math.Abs(dy);
        int sign_x = dx > 0 ? 1 : -1, sign_y = dy > 0 ? 1 : -1;

        Vector2 p = new Vector2(p0.x, p0.y);
        List<Vector2> points = new List<Vector2>();
        for (int ix = 0, iy = 0; ix < nx || iy < ny;)
        {
            float decision = (1 + 2 * ix) * ny - (1 + 2 * iy) * nx;
            if (decision == 0)
            {
                // next step is diagonal
                p.x += sign_x;
                p.y += sign_y;
                ix++;
                iy++;
            }
            else if (decision < 0)
            {
                // next step is horizontal
                p.x += sign_x;
                ix++;
            }
            else
            {
                // next step is vertical
                p.y += sign_y;
                iy++;
            }
            points.Add(new Vector2(p.x, p.y));
        }

        foreach (Vector2 point in points)
        {
            WorldTilemap.SetTile(new Vector3Int((int)point.x, (int)point.y, 0), LineTile);
        }
    }

    private void GenerateLinesBetweenRooms()
    {

    }

    private void GenerateLinesBetweenRoomsOld()
    {
        List<Tuple<Vector2, Vector2>> Lines = new List<Tuple<Vector2, Vector2>>();
        
        for(int x = 0; x < mRoomPositions.Count; x++)
        {
            for (int y = 0; y < mRoomPositions.Count; y++)
            {
                Lines.Add(new Tuple<Vector2, Vector2>(mRoomPositions[x], mRoomPositions[y]));
            }
        }
              
        foreach(var line in Lines)
        {
            walk_grid(line.Item1, line.Item2);
        }
        
        foreach (Vector2 position in mRoomPositions)
        {
            WorldTilemap.SetTile(new Vector3Int((int)position.x, (int)position.y, 0), ColouredTile);
        }
    }

    private void EnlargeRooms()
    {
        Vector2 position;
        for (int i = 0; i < mRoomPositions.Count; i++)
        {
            position = mRoomPositions[i];
            int targetRoomSize = RNGCore.Instance.RandomRoll(MinRoomSize, MaxRoomSize);
            int currentRoomSize = 0;


            while(currentRoomSize != targetRoomSize || 
                
                (WorldTilemap.GetTile(new Vector3Int((int)position.x, (int)position.y + 1)) != ColouredTile &&
                 WorldTilemap.GetTile(new Vector3Int((int)position.x + 1, (int)position.y)) != ColouredTile &&
                 WorldTilemap.GetTile(new Vector3Int((int)position.x - 1, (int)position.y)) != ColouredTile &&
                 WorldTilemap.GetTile(new Vector3Int((int)position.x, (int)position.y - 1)) != ColouredTile)
                )
            {
                Vector2 cardinal;
                int dir = RNGCore.Instance.RandomRoll(1, 4);

                switch (dir)
                {
                    default:
                        cardinal = Vector2.zero;
                        break;

                    case 1:
                        cardinal = Vector2.up;
                        break;
                    case 2:
                        cardinal = Vector2.left;
                        break;
                    case 3:
                        cardinal = Vector2.down;
                        break;
                    case 4:
                        cardinal = Vector2.right;
                        break;
                }

                position += cardinal;
                currentRoomSize++;
                WorldTilemap.SetTile(new Vector3Int((int)position.x, (int)position.y), ColouredTile);
            }

           

        }











        //foreach(Vector2 position in mRoomPositions)
        //{
        //    Vector2 roomSize = new Vector2(
        //        RNGCore.Instance.RandomRoll(MinRoomSize, MaxRoomSize),
        //        RNGCore.Instance.RandomRoll(MinRoomSize, MaxRoomSize)
        //        );
        //
        //    if (roomSize.x / 2.0f != 0.0f)
        //    {
        //        roomSize.x--;
        //    }
        //    
        //    if (roomSize.y / 2.0f != 0.0f)
        //    {
        //        roomSize.y--;
        //    }
        //
        //    Vector2 halfSize = new Vector2(roomSize.x / 2.0f, roomSize.y / 2.0f);
        //    Vector2 BL = position - halfSize;
        //    Vector2 TR = position + halfSize;
        //
        //    WorldTilemap.BoxFill(new Vector3Int((int)BL.x, (int)BL.y, 1), ColouredTile, (int)BL.x, (int)BL.y, (int)TR.x, (int)TR.y);
        //    WorldTilemap.SetTile(new Vector3Int((int)position.x, (int)position.y, 1), LineTile);
        //}
    }

    public void Update()
    {

    }


    private void OnDrawGizmos()
    {
       
    }
}