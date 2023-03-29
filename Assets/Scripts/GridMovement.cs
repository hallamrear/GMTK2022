using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridMovement : MonoBehaviour
{
    [HideInInspector] public WorldGeneration mWorld;

    [HideInInspector] public List<int> mDamageRollList = new List<int>();
    public GameObject BulletPrefab;

    public Tile mCharacterTile;
    public Tilemap WorldTilemap;
    public Tilemap CharacterMap;
    GridLayout mGrid;

    protected Vector3Int FacingDirection;
    protected Vector3Int mTilePosition;
    protected Vector3Int mPreviousPosition;

    protected bool mMovementEnabled = false;
    protected float mTimeSinceLastMovement;
    protected float mMovementCooldown = 0.1f;
    private bool HasReset = false;

    // Start is called before the first frame update
    void Start()
    {
        mDamageRollList.Add(Random.Range(1, 6));
        mDamageRollList.Add(Random.Range(1, 6));
        mDamageRollList.Add(Random.Range(1, 6));

        mWorld = FindObjectOfType<WorldGeneration>();
        mGrid = WorldTilemap.GetComponentInParent<GridLayout>();
        mPreviousPosition = mTilePosition; 
    }

    public void ResetToStart()
    {
        SetTilePosition(mWorld.GetStartRoomPosition(), mCharacterTile);
    }

    public void SetTilePosition(Vector3Int pos, Tile tile)
    {
        mPreviousPosition = mTilePosition;
        mTilePosition = pos;

        CharacterMap.SetTile(mPreviousPosition, null);
        CharacterMap.SetTile(mTilePosition, tile);
    }

    public Vector3Int GetTilePosition()
    {
        return mTilePosition;
    }

    public void Kill()
    {

    }

    void Update()
    {
        if(HasReset == false)
        {
            ResetToStart();
            HasReset = true;
        }

        FacingDirection = mTilePosition - mPreviousPosition;

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
        }

        if (Input.GetKey(KeyCode.X))
        {
            ResetToStart();
        }
    }

    private void Fire()
    {
        if(mDamageRollList.Count != 0)
        {
            GameObject obj = Instantiate(BulletPrefab);
            obj.GetComponent<Bullet>().SetPlayerWhoFired(this);
            obj.GetComponent<Bullet>().SetTilePosition(mTilePosition + FacingDirection);
            obj.GetComponent<Bullet>().SetDamageValue(mDamageRollList[mDamageRollList.Count - 1]);
            mDamageRollList.RemoveAt(mDamageRollList.Count - 1);
            obj.GetComponent<Bullet>().SetDirection(FacingDirection);
            obj.GetComponent<Bullet>().mWorld = mWorld;
        }
    }

    protected void MovePlayer(Vector3Int direction)
    {
        mTimeSinceLastMovement = 0.0f;

        if (WorldTilemap.GetTile(mTilePosition) == null)
            ResetToStart();

        Vector3Int target = mTilePosition + direction;
        if (CanMove(target))
        {
            SetTilePosition(target, mCharacterTile);
        }
    }

    protected virtual bool CanMove(Vector3Int position)
    {
        TileBase tile = WorldTilemap.GetTile(position);
       
        if(tile == mWorld.ExitTile)
        {
            GetComponent<Statistics>().LevelsComplete++;
            mWorld.GenerateMap();
            HasReset = false;
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
                WorldTilemap.SetTile(chest.Value.Position, mWorld.mOpenChestTile);

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
        if (tile == mWorld.mOpenChestTile)
            return false;
        if (tile == mWorld.DoorTile)
            return true;
        if (tile == mWorld.FloorTile)
            return true;
        if (tile == mWorld.TunnelTile)
            return true;

        return false;
    }
}
