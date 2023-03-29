using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LadderItem : MonoBehaviour, Item
{
    [SerializeField] Tile mLadderTile;
    private bool timerRunning;
    private float timerElapsed = 0.0f;
    private float timerTotal = 6.5f;

    // Start is called before the first frame update
    void Start()
    {
        timerRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(timerRunning)
        {
            timerElapsed += Time.deltaTime;

            if(timerElapsed > timerTotal)
            {
                Destroy(this.gameObject);
            }
        }

    }
    public void Use(GameObject player, Vector3Int TileUsedOn)
    {
        WorldGeneration generator = FindObjectOfType<WorldGeneration>();
        generator.GenerateMap();
        player.GetComponent<Statistics>().LevelsComplete++;
        timerRunning = true;
    }

}
