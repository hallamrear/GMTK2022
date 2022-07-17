using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LadderItem : MonoBehaviour, Item
{
    [SerializeField] Tile mLadderTile;

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

    }
}
