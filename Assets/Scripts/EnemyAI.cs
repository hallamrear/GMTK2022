using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyAI : MonoBehaviour
{
    private Vector3Int lastDir = Vector3Int.zero;
    [HideInInspector] public GridMovement PlayerMovement;
    [SerializeField] public Tile mEnemyTile;
    [SerializeField] private Tile mPathTile;
    public float SightRange;

    private bool IsAlive { get; set; }

    protected bool mMovementEnabled = false;
    protected float mTimeSinceLastMovement;
    protected float mMovementCooldown = 0.2f;

    protected Vector3Int mTilePosition;
    protected Vector3Int mPreviousPosition;

    [HideInInspector] public Tilemap CharacterMap;
    [HideInInspector] public Tilemap WorldTilemap;
    [HideInInspector] public WorldGeneration mWorld;
    GridLayout mGrid;

    // Start is called before the first frame update
    void Start()
    {
        IsAlive = true;
        PlayerMovement = FindObjectOfType<GridMovement>();
        CharacterMap = PlayerMovement.CharacterMap;
        mWorld = FindObjectOfType<WorldGeneration>();
        WorldTilemap = mWorld.WorldTilemap;
        mGrid = WorldTilemap.GetComponentInParent<GridLayout>();
    }

    protected void MovePlayer(Vector3Int direction)
    {
        mTimeSinceLastMovement = 0.0f;

        Vector3Int target = mTilePosition + direction;
        if (CanMove(target))
        {
            SetTilePosition(target);
        }
    }


    public void SetTilePosition(Vector3Int pos)
    {
        mPreviousPosition = mTilePosition;
        mTilePosition = pos;

        if (CharacterMap == null)
        {
            CharacterMap = FindObjectOfType<GridMovement>().CharacterMap;
        }

        CharacterMap.SetTile(mPreviousPosition, null);
        CharacterMap.SetTile(mTilePosition, mEnemyTile);

        transform.position = CharacterMap.CellToWorld(mTilePosition);
    }

    public Vector3Int GetTilePosition()
    {
        return mTilePosition;
    }


    //Check which component of the vector(x, y, or z) has the biggest absolute value.
    //Make that component 1, and all others 0.
    //Change the sign to the original sign of that component.
    public Vector3Int DirectionToCardinalDirection(Vector3Int direction)
    {
        Vector3Int cardinal = new Vector3Int();
        if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if(direction.x > 0)
                cardinal.x = 1;
            else
                cardinal.x = -1;

            cardinal.y = 0;
        }
        else
        {
            if (direction.y > 0)
                cardinal.y = 1;
            else
                cardinal.y = -1;

            cardinal.x = 0;
        }

        return cardinal;
    }


    // Update is called once per frame
    void Update()
    {
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

            Vector3Int direction = Vector3Int.zero;

            float distance = Vector3Int.Distance(PlayerMovement.GetTilePosition(), mTilePosition);
            if (distance <= SightRange)
            {
                int randomDirChance = RNGCore.RandomRoll(0, 100);
                if (randomDirChance > 80)
                {
                    int dir = RNGCore.RandomRoll(1, 4);

                    switch (dir)
                    {
                        case 1:
                            direction = Vector3Int.up;
                            break;
                        case 2:
                            direction = Vector3Int.down;
                            break;
                        case 3:
                            direction = Vector3Int.left;
                            break;
                        case 4:
                            direction = Vector3Int.right;
                            break;

                        default:
                            break;
                    }

                    MovePlayer(direction);
                }
                else
                {
                    direction = PlayerMovement.GetTilePosition() - mTilePosition;
                    direction = DirectionToCardinalDirection(direction);

                    MovePlayer(direction);
                }
            }
            else
            {
                bool randomMovementThisFrame = RNGCore.CoinFlip();
                if (randomMovementThisFrame)
                {
                    int dir = Random.Range(1, 4);

                    switch (dir)
                    {
                        case 1:
                            direction = Vector3Int.up;
                            break;
                        case 2:
                            direction = Vector3Int.down;
                            break;
                        case 3:
                            direction = Vector3Int.left;
                            break;
                        case 4:
                            direction = Vector3Int.right;
                            break;

                        default:
                            break;
                    }

                    MovePlayer(direction);
                }
            }
        }

        
    }

    protected bool CanMove(Vector3Int position)
    {
        TileBase tile = WorldTilemap.GetTile(position);

        if (tile == mWorld.ExitTile)
        {
            Kill();
        }

        if (tile == mWorld.DoorTile)
            return true;
        if (tile == mWorld.FloorTile)
            return true;
        if (tile == mWorld.TunnelTile)
            return true;

        return false;
    }
    
    public void Kill()
    {
        if (IsAlive)
        {
            IsAlive = false;
            FindObjectOfType<Statistics>().EnemiesKilled++;
            CharacterMap.SetTile(mTilePosition, null);
            Destroy(this.gameObject);
        }
    }
}
