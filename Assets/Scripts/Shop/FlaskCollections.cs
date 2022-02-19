using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlaskCollections : MonoBehaviour
{
    [SerializeField] GameData gameData;
    [SerializeField] Vector3 Item_1Pos;
    [SerializeField] Vector3 Item_2Pos;
    [SerializeField] Vector3 Item_3Pos;
    [SerializeField] GameObject ItemSetParentPrefab;
    GameObject[] ItemSetParents;

    private int counter;

    private void Start()
    {
        InitFlaskCollections();
    }

    public void InitFlaskCollections()
    {
        List<GameObject> tempList = new List<GameObject>();

        for (int i = 0; i < gameData.MutagenGroups.Length; i++)
        {
            bool active = false;
            GameObject Parent = Instantiate(ItemSetParentPrefab, this.transform.position, this.transform.rotation, this.transform);
            Parent.name = gameData.MutagenGroups[i].GroupName;
            GameObject temp;
            temp = GetFlask(gameData.MutagenGroups[i].Item_1);
            if(temp != null)
            {
                active = true;
                GameObject item1 = Instantiate(temp, this.transform.localPosition + Item_1Pos, this.transform.rotation, Parent.transform);
            }
            temp = GetFlask(gameData.MutagenGroups[i].Item_2);
            if (temp != null)
            {
                active = true;
                GameObject item2 = Instantiate(temp, this.transform.localPosition + Item_2Pos, this.transform.rotation, Parent.transform);
            }
            temp = GetFlask(gameData.MutagenGroups[i].Item_3);
            if (temp != null)
            {
                active = true;
                GameObject item3 = Instantiate(temp, this.transform.localPosition + Item_3Pos, this.transform.rotation, Parent.transform);
            }

            if (active)
            {
                tempList.Add(Parent);
            }
            else
            {
                Destroy(Parent);
            }
        }

        ItemSetParents = tempList.ToArray();

        if (ItemSetParents.Length == 0)
        {
            Invoke("Complete", 3);
        }
        else
        {
            foreach (var item in ItemSetParents)
            {
                item.SetActive(false);
            }

            ShowSet(0);
        }
    }

    void ShowSet(int index)
    {
        ItemSetParents[index].SetActive(true);
        ItemSetParents[index].transform.DOScale(new Vector3(1, 1, 1), 0.25f);
    }

    void HideSet(int index)
    {
        ItemSetParents[index].transform.DOScale(new Vector3(0, 0, 0), 0.25f).OnComplete(() => { ItemSetParents[index].SetActive(false); });
    }

    public void NextSet()
    {
        HideSet(counter);
        if (counter < ItemSetParents.Length - 1)
        {
            counter += 1;
            ShowSet(counter);
        }
        else
        {
            Invoke("Complete", 3);
        }
    }

    public void Complete()
    {
        FindObjectOfType<MainLaboratory>().ShowStartButton();
    }


    private GameObject GetFlask(ShopItemsType type)
    {
        foreach (var item in gameData.ShopItems)
        {

            if(item.itemType == ShopItemsType.None)
            {
                return null;
            }

            if(item.itemType == type)
            {
                if(item.itemState == ShopItemState.bought)
                {
                    return item.Prefab;
                }
            }
        }

        return null;
    }
}
