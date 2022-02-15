using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] Image ProgressBarImage;
    [SerializeField] Text ProgressText;
    bool flag = true;
    float MaxValue = 0;
    float progress;

    public void UpdateProdressBar(int value)
    {
        float val = value;

        if (flag)
        {
            MaxValue = val;
            flag = false;
        }
        else
        {
            val = MaxValue - value;
            ProgressText.text = value.ToString();
            //Debug.Log(((val / MaxValue) * 100) / 100);
            ProgressBarImage.DOFillAmount(1 -( val / MaxValue), 0.25f);

           // ProgressBarImage.fillAmount =  ((val / MaxValue) * 100) / 100;
            
        }
       
    }
}
