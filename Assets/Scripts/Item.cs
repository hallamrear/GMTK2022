using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface Item
{
    public void Use(GameObject player, Vector3Int TileUsedOn);
}
