using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlaskCollections : MonoBehaviour
{
    [SerializeField] GameData gameData;
    [SerializeField] Vector3 step = new Vector3 (0.4f, 0, 0);
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
        List<GameObject> items = new List<GameObject>();

        for (int i = 0; i < gameData.MutagenGroups.Length; i++)
        {
            items.Clear();
            GameObject parent = Instantiate(ItemSetParentPrefab, transform.position, transform.rotation, transform);
            parent.name = gameData.MutagenGroups[i].GroupName;
            for (int j = 0; j < gameData.MutagenGroups[i].items.Count; j++)
            {
                GameObject temp = GetFlask(gameData.MutagenGroups[i].items[j]);
                if (temp != null)
                {
                    items.Add(Instantiate(temp, transform.localPosition, transform.rotation, parent.transform));
                }
            }
            for (int j = 0; j < items.Count; j++)
            {
                items[j].transform.localPosition = items[j].transform.localPosition +
                                                    new Vector3(step.x * (-items.Count / 2) + j * step.x, step.y, step.z);
            }

            if (items.Count > 0)
            {
                tempList.Add(parent);
            }
            else
            {
                Destroy(parent);
            }
        }

        ItemSetParents = tempList.ToArray();

        if (ItemSetParents.Length < 1)
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
