using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridMovement : MonoBehaviour
{
    [HideInInspector] public WorldGeneration mWorld;

    public Tile mCharacterTile;
    public Tilemap WorldTilemap;
    public Tilemap CharacterMap;
    GridLayout mGrid;

    private Vector3Int mTilePosition;
    private Vector3Int mPreviousPosition;

    bool mMovementEnabled = false;
    float mTimeSinceLastMovement;
    float mMovementCooldown = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        mWorld = FindObjectOfType<WorldGeneration>();
        mGrid = WorldTilemap.GetComponentInParent<GridLayout>();
        mPreviousPosition = mTilePosition; 
    }

    public void ResetToStart()
    {
        SetTilePosition(mWorld.GetStartRoomPosition());
    }

    public void SetTilePosition(Vector3Int pos)
    {
        mPreviousPosition = mTilePosition;
        mTilePosition = pos;

        CharacterMap.SetTile(mPreviousPosition, null);
        CharacterMap.SetTile(mTilePosition, mCharacterTile);
    }

    public Vector3Int GetTilePosition()
    {
        return mTilePosition;
    }

    void Update()
    {
        Vector3 p = mGrid.CellToWorld(mTilePosition);
        p.z = 0.0f;
        transform.position = Vector3.Lerp(transform.position, p, Time.deltaTime);

        if (mTimeSinceLastMovement < mMovementCooldown)
        {
            mTimeSinceLastMovement += Time.deltaTime;
            mMovementEnabled = false;
        }
        else
        {
            mMovementEnabled = true;
        }

        if (mMovementEnabled)
        {
            if (Input.GetKey(KeyCode.W))
            {
                MovePlayer(Vector3Int.up);
            }
            if (Input.GetKey(KeyCode.A))
            {
                MovePlayer(Vector3Int.right);
            }
            if (Input.GetKey(KeyCode.S))
            {
                MovePlayer(Vector3Int.down);
            }
            if (Input.GetKey(KeyCode.D))
            {
                MovePlayer(Vector3Int.left);
            }
        }

        if(Input.GetKey(KeyCode.X))
        {
            ResetToStart();
        }

        //if(Input.GetMouseButton(0))
        //{
        //    Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    worldPos.z = 0.0f;
        //    SetTilePosition(mGrid.WorldToCell(worldPos));
        //}
    }

    private void MovePlayer(Vector3Int direction)
    {
        mTimeSinceLastMovement = 0.0f;

        Vector3Int target = mTilePosition + direction;
        if (CanMove(target))
        {
            SetTilePosition(target);
        }
    }

    private bool CanMove(Vector3Int position)
    {
        TileBase tile = WorldTilemap.GetTile(position);
       
        if(tile == mWorld.ExitTile)
        {
            GetComponent<Statistics>().LevelsComplete++;
            mWorld.GenerateMap();
            return false;
        }

        if(tile == mWorld.mClosedChestTile)
        {
            WorldGeneration.Chest? chest = null;

            foreach (WorldGeneration.Chest c in mWorld.Chests)
            {
                if(c.Position == position)
                {
                    chest = c;
                    break;
                }
            }

            if (chest != null)
            {
                CharacterMap.SetTile(chest.Value.Position, mWorld.mOpenChestTile);


                List<GameObject> items = chest.Value.GetItems();

                if (chest.Value.GetItems().Count != 0)
                {
                    foreach (GameObject item in items)
                    {
                        var obj = Instantiate(item);
                        GetComponent<Inventory>().AddItem(obj);
                    }
                    chest.Value.GetItems().Clear();
                    GetComponent<Statistics>().ChestsOpened++;
                }
            }

            return false;
        }

        if (tile == mWorld.DoorTile)
            return true;
        if (tile == mWorld.FloorTile)
            return true;
        if (tile == mWorld.TunnelTile)
            return true;

        return false;
    }
}
