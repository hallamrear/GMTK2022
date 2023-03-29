using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageRollsUI : MonoBehaviour
{
    GridMovement mMovement;
    public List<Image> UIItems;
    public List<Sprite> DiceSprites;

    // Start is called before the first frame update
    void Start()
    {
        mMovement = GetComponent<GridMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < mMovement.mDamageRollList.Count; i++)
        {
            UIItems[i].gameObject.SetActive(true);
            UIItems[i].sprite = DiceSprites[mMovement.mDamageRollList[i] - 1];
        }

        if (mMovement.mDamageRollList.Count != 5)
        {
            for (int i = mMovement.mDamageRollList.Count; i < 5; i++)
            {
                UIItems[i].gameObject.SetActive(false);
            }
        }
    }
}
