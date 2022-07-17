using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistics : MonoBehaviour
{
    [HideInInspector] public int LevelsComplete = 0;
    [HideInInspector] public int ChestsOpened = 0;
    [HideInInspector] public int EnemiesKilled = 0;


    [SerializeField] private GameObject mStatsUIRoot;
    [SerializeField] private TMPro.TextMeshProUGUI mLevelsCompleteUI;
    [SerializeField] private TMPro.TextMeshProUGUI mChestsOpenedUI;
    [SerializeField] private TMPro.TextMeshProUGUI mEnemiesKilledUI;

    private void Start()
    {
        mStatsUIRoot.SetActive(false);

    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.CapsLock))
        {
            mStatsUIRoot.SetActive(true);
        }
        else
        {
            mStatsUIRoot.SetActive(false);
        }

        mLevelsCompleteUI.SetText("Levels Completed: " + LevelsComplete.ToString());
        mChestsOpenedUI.SetText("Chests Opened: " + ChestsOpened.ToString());
        mEnemiesKilledUI.SetText("Enemies Killed: " + EnemiesKilled.ToString());
    }
}
