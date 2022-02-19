using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


// ÍÅ ÀÊÒÓÀËÅÍ (ÈÑÏÎËÜÇÓÅÒÑß FlaskCollections)
public class MutagenSetParent : MonoBehaviour
{
    [SerializeField] FluskSet[] mutagenSet;

    private int counter;
    

    private void Start()
    {
        foreach (var item in mutagenSet)
        {
            item.SomeObject.transform.localScale = new Vector3(0, 0, 0);
            item.SomeObject.SetActive(false);
        }
        ShowSet(0);
    }

    void ShowSet(int index)
    {
        mutagenSet[index].SomeObject.SetActive(true);
        mutagenSet[index].SomeObject.transform.DOScale(new Vector3(1, 1, 1), 0.25f);
    }

    void HideSet(int index)
    {
        mutagenSet[index].SomeObject.transform.DOScale(new Vector3(0, 0, 0), 0.25f).OnComplete(() => { mutagenSet[index].SomeObject.SetActive(false); });
    }

    public void NextSet()
    {
        HideSet(counter);
        if(counter < mutagenSet.Length-1)
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

    public void HideHand()
    {
        if(mutagenSet[counter].hand != null)
        {
            mutagenSet[counter].hand.transform.DOScale(0, 0.25f);
        }
    }


}

[System.Serializable]
public struct FluskSet
{
    public GameObject SomeObject;
    public GameObject hand;
}
