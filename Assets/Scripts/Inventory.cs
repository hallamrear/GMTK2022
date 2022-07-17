using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject mHoldingItemUI;
    public List<Sprite> mItemSprites = new List<Sprite>(5);
    public GameObject[] mItemPrefabs = new GameObject[5];
    private int mCurrentItem = 0;
    GridMovement mMovement;
    private int mMaxStackSize = 5;
    [SerializeField] private GameObject mInventoryUI;
    [SerializeField] List<TMPro.TextMeshProUGUI> mInventoryCountUI;
    List<int> mInventoryList = new List<int>(5) { 0, 0, 0, 0, 0 };

    // Start is called before the first frame update
    void Start()
    {
        mMovement = GetComponent<GridMovement>();
    }

    public bool AddItem(GameObject item)
    {
        Destroy(item);

        switch (item.GetComponent<Item>())
        {
            case RerollItem:
                if (mInventoryList[0] < mMaxStackSize)
                {
                    mInventoryList[0]++;
                    return true;
                }
                break;

            case BombItem:
                if (mInventoryList[1] < mMaxStackSize)
                {
                    mInventoryList[1]++;
                    return true;
                }
                break;

            case LadderItem:
                if (mInventoryList[2] < mMaxStackSize)
                {
                    mInventoryList[2]++;
                    return true;
                }
                break;

            case FanItem:
                if (mInventoryList[3] < mMaxStackSize)
                {
                    mInventoryList[3]++;
                    return true;
                }
                break;

            case PotionItem:
                if (mInventoryList[4] < mMaxStackSize)
                {
                    mInventoryList[4]++;
                    return true;
                }
                break;
        }

        return false;
    }
    public bool RemoveItem(GameObject item)
    {
        switch (item.GetComponent<Item>())
        {
            case RerollItem:
                if (mInventoryList[0] > 0)
                {
                    mInventoryList[0]--;
                    return true;
                }
                break;

            case BombItem:
                if (mInventoryList[1] > 0)
                {
                    mInventoryList[1]--;
                    return true;
                }
                break;

            case LadderItem:
                if (mInventoryList[2] > 0)
                {
                    mInventoryList[2]--;
                    return true;
                }
                break;

            case FanItem:
                if (mInventoryList[3] > 0)
                {
                    mInventoryList[3]--;
                    return true;
                }
                break;

            case PotionItem:
                if (mInventoryList[4] > 0)
                {
                    mInventoryList[4]--;
                    return true;
                }
                break;
        }

        return false;
    }

    bool CanUseItem(int itemIndex)
    {
        return (mInventoryList[itemIndex] > 0);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Y))
        //    AddItem(mItemPrefabs[0]);
        //if (Input.GetKeyDown(KeyCode.U))
        //    AddItem(mItemPrefabs[1]);
        //if (Input.GetKeyDown(KeyCode.I))
        //    AddItem(mItemPrefabs[2]);
        //if (Input.GetKeyDown(KeyCode.O))
        //    AddItem(mItemPrefabs[3]);
        //if (Input.GetKeyDown(KeyCode.P))
        //    AddItem(mItemPrefabs[4]);
        //
        //if (Input.GetKeyDown(KeyCode.G))
        //    RemoveItem(mItemPrefabs[0]);
        //if (Input.GetKeyDown(KeyCode.H))
        //    RemoveItem(mItemPrefabs[1]);
        //if (Input.GetKeyDown(KeyCode.J))
        //    RemoveItem(mItemPrefabs[2]);
        //if (Input.GetKeyDown(KeyCode.K))
        //    RemoveItem(mItemPrefabs[3]);
        //if (Input.GetKeyDown(KeyCode.L))
        //    RemoveItem(mItemPrefabs[4]);


        if (Input.GetKeyDown(KeyCode.Q))
        {
            mCurrentItem--;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            mCurrentItem++;
        }

        if (mCurrentItem > mMaxStackSize)
        {
            mCurrentItem = 0;
        }
        else if(mCurrentItem < 0)
        {
            mCurrentItem = 4;
        }

        mHoldingItemUI.GetComponent<Image>().sprite = mItemSprites[mCurrentItem];

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (CanUseItem(mCurrentItem))
            {
                GameObject obj = Instantiate(mItemPrefabs[mCurrentItem]);
                obj.transform.position = mMovement.WorldTilemap.WorldToCell(mMovement.GetTilePosition());
                obj.GetComponent<Item>().Use(this.gameObject, mMovement.GetTilePosition());
            }
            else
            {
                GetComponent<AudioSource>().Play();
            }
        }

        if(Input.GetKey(KeyCode.I))
        {
            mInventoryUI.SetActive(true);
        }
        else
        {
            mInventoryUI.SetActive(false);
        }

        mInventoryCountUI[0].SetText(mInventoryList[0].ToString());
        mInventoryCountUI[1].SetText(mInventoryList[1].ToString());
        mInventoryCountUI[2].SetText(mInventoryList[2].ToString());
        mInventoryCountUI[3].SetText(mInventoryList[3].ToString());
        mInventoryCountUI[4].SetText(mInventoryList[4].ToString());
    }
}
