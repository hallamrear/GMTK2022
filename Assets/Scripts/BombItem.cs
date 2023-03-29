using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BombItem : MonoBehaviour, Item
{
    [SerializeField] Tile mBombTile;
    private Tile mTileReplaced;
    [SerializeField] private Tile mEnemyTile;
    public int mSize = 3;
    [SerializeField] ParticleSystem mParticles;

    public void Use(GameObject player, Vector3Int TileUsedOn)
    {
        Tilemap world = player.GetComponent<GridMovement>().WorldTilemap;
        mTileReplaced = (Tile)world.GetTile(TileUsedOn);
        world.SetTile(TileUsedOn, mBombTile);

        StartCoroutine(Explosion(world, TileUsedOn));
    }

    private IEnumerator Explosion(Tilemap map, Vector3Int tile)
    {
        yield return new WaitForSeconds(1.0f);
        map.SetTile(tile, mTileReplaced);
        var obj = Instantiate(mParticles);
        Vector3 pos = transform.position;
        pos.z = 1.0f;
        obj.gameObject.transform.position = pos;
        obj.Play();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        if (colliders.Length > 0)
        {
            // enemies within 1m of the player
            foreach (var col in colliders)
            {
                if (col.GetComponent<EnemyAI>() != null)
                {
                    col.gameObject.GetComponent<EnemyAI>().Kill();
                }
            }
        }


        Destroy(this.gameObject, 1.1f);
        yield return new WaitForSeconds(1.0f);
        Destroy(obj.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
