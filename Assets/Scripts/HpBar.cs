using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class HpBar : MonoBehaviour
{
    [SerializeField] CanvasGroup canvas;
    [SerializeField] Image hp;
    [SerializeField] Image Fill;
    [SerializeField] TextMeshProUGUI tmp;
    Transform LookObject;
    float maxValue;
    float hpBahValue;
    [Range(0, 1)] [SerializeField] float ImageColor;

    private void Awake()
    {
        canvas = GetComponent<CanvasGroup>();
    }


    void LateUpdate()
    {
        transform.LookAt(LookObject);
    }

    public void Init(int max)
    {
        canvas.DOFade(1, 1f);
        LookObject = Camera.main.transform;
        maxValue = max;
    }
    public void SetValue(int value, int level)
    {
        float val = value;

        if (val > maxValue)
        {
            maxValue = val;
        }

        if (val == 0)
        {
            hpBahValue = val;
        }
        else
        {
            hpBahValue = ((val / maxValue) * 100);
        }
        hp.fillAmount = hpBahValue / 100;
        Fill.fillAmount = hpBahValue / 100;
        SetImageColor(hpBahValue / 100);
        tmp.text = "Lv." + level.ToString();
    }

    public void SetImageColor(float t)
    {
        if (t > 0.6)
        {
            hp.color = Color32.Lerp(new Color32(123, 255, 0, 255), new Color32(0, 255, 33, 255), t);
        }
        else if (t > 0.3 && t < 0.6)
        {
            hp.color = Color32.Lerp(new Color32(225, 188, 0, 255), new Color32(221, 255, 0, 255), t * 2);
        }
        else
        {
            hp.color = Color32.Lerp(new Color32(255, 0, 0, 255), new Color32(225, 97, 0, 255), t * 2);
        }
    }
}
