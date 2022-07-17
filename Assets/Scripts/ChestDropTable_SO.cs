using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chest Drop Table", menuName = "ScriptableObjects/Chest Drop Table", order = 1)]
public class ChestDropTable_SO : ScriptableObject
{
    public List<GameObject> Items;
}
