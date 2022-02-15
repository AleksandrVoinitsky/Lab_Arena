using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    [SerializeField] GameObject Button;
    [SerializeField] GameObject Hand;
    [SerializeField] UnityEvent Use;

    private bool flag = true;

    private void OnMouseDown()
    {
        if (flag)
        {
            flag = false;
            Hand.SetActive(false);
            Vector3 StartPos = Button.transform.position;
            Button.transform.DOScale(new Vector3(0.8f, 0.8f, 1), 0.25f);
            Button.transform.DOMove(new Vector3(Button.transform.position.x, Button.transform.position.y, Button.transform.position.z - -0.190f), 0.25f).OnComplete(() =>
            {
                Button.transform.DOScale(new Vector3(1, 1, 1), 0.25f);
                Button.transform.DOMove(StartPos, 0.25f).OnComplete(() => { Use.Invoke(); }); 
            });
        }
        
    }
}
