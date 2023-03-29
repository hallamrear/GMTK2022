using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public WorldGeneration mWorld;

    public Tile EnemyTile;
    public Tile PlayerTile;

    [HideInInspector] public GridMovement Player;
    public Tile BulletTile;
    private int mDamageValue;
    private Vector3Int mTilePosition;
    private Vector3Int mPreviousPosition;
    private Vector3Int mTravelDirection;
    private bool mMovementEnabled;
    protected float mTimeSinceLastMovement;
    protected float mMovementCooldown = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        
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

        if (Player != null && mMovementEnabled)
        {
            Move();
        }
    }

    public void SetTilePosition(Vector3Int pos)
    {
        mPreviousPosition = mTilePosition;
        mTilePosition = pos;

        Player.CharacterMap.SetTile(mPreviousPosition, null);
        Player.CharacterMap.SetTile(mTilePosition, BulletTile);

        transform.position = Player.CharacterMap.CellToWorld(mTilePosition);
    }

    private void Move()
    {
        mTimeSinceLastMovement = 0.0f;
        Vector3Int target = mTilePosition + mTravelDirection;

        if (CanMove(target))
        {
            SetTilePosition(target);
        }
        else
        {
            Kill();
        }
    }

    void Kill()
    {
        Player.CharacterMap.SetTile(mTilePosition, null);
        Destroy(this.gameObject);
    }

    public void SetPlayerWhoFired(GridMovement player)
    {
        Player = player;
    }

    protected bool CanMove(Vector3Int position)
    {
        TileBase worldTile = Player.WorldTilemap.GetTile(position);
        TileBase characterTile = Player.CharacterMap.GetTile(position);

        if (characterTile == PlayerTile)
        {
            Player.GetComponent<GridMovement>().Kill();
            return false;
        }

        if (characterTile == EnemyTile)
        {
            EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
            foreach (EnemyAI enemy in enemies)
            {
                if (enemy.GetTilePosition() == position)
                {
                    enemy.Kill();
                    break;
                }
            }

            return false;
        }

        if (worldTile == mWorld.WallTile)
            return false;
        if (worldTile == mWorld.DoorTile)
            return false;
        if (worldTile == mWorld.TunnelTile)
            return false;

        return true;
    }


    public void SetDirection(Vector3Int travelDirection)
    {
        mTravelDirection = travelDirection;
    }

    public void SetDamageValue(int damage)
    {
        mDamageValue = damage;
    }
}
