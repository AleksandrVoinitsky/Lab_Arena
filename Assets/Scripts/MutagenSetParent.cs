using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;



public class MutagenSetParent : MonoBehaviour
{
    [SerializeField] GameObject[] mutagenSet;

    private int counter;
    

    private void Start()
    {
        foreach (var item in mutagenSet)
        {
            item.transform.localScale = new Vector3(0, 0, 0);
            item.SetActive(false);
        }
        ShowSet(0);
    }

    void ShowSet(int index)
    {
        mutagenSet[index].SetActive(true);
        mutagenSet[index].transform.DOScale(new Vector3(1, 1, 1), 0.25f);
    }

    void HideSet(int index)
    {
        mutagenSet[index].transform.DOScale(new Vector3(0, 0, 0), 0.25f).OnComplete(() => { mutagenSet[index].SetActive(false); });
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


}
