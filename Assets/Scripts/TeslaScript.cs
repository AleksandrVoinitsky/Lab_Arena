using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;
using DG.Tweening;

public class TeslaScript : MonoBehaviour
{
    [SerializeField] private int type;

    private void Shot()
    {
        if (type == 1)
        {
            Camera.main.transform.DOShakePosition(0.1f, 0.3f);
            MMVibrationManager.Haptic(HapticTypes.LightImpact);
            FindObjectOfType<MainLaboratory>().ShakePlayer();
        }
    }
}
