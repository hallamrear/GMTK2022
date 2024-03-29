using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RerollItem : MonoBehaviour, Item
{
    [SerializeField] Tile mRerollTile;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Use(GameObject player, Vector3Int TileUsedOn)
    {
        GridMovement movement = player.GetComponent<GridMovement>();
        movement.mDamageRollList.Clear();
        movement.mDamageRollList.Add(Random.Range(1, 6));
        movement.mDamageRollList.Add(Random.Range(1, 6));
        movement.mDamageRollList.Add(Random.Range(1, 6));
        movement.mDamageRollList.Add(Random.Range(1, 6));
        movement.mDamageRollList.Add(Random.Range(1, 6));
        Destroy(this.gameObject);
    }
}
