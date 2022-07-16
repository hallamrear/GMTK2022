using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RNGCore : MonoBehaviour
{
    public static RNGCore Instance { get;  private set; }

    [HideInInspector] public readonly int mPlayerRolls;
    public static int Roll = -1;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        int time = System.DateTime.Now.Second;
        SetSeed(time);
    }

    public void SetSeed(int seed)
    {
        Random.InitState(seed);
    }

    public int RandomRoll(int lowerInc = 1, int upperInc = 6)
    {
        Roll = Random.Range(lowerInc, upperInc);
        return Roll;
    }
    public int RollSixSided()
    {
        return RandomRoll(1, 6);
    }

    public int RollTwentySided()
    {
        return RandomRoll(1, 20);
    }

    public int RollTwelveSided()
    {
        return RandomRoll(1, 12);
    }


    public bool CoinFlip()
    {
        if(Random.Range(0.0f, 1.0f) < 0.5f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
