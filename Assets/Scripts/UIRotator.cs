using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRotator : MonoBehaviour
{
    [SerializeField] private Vector3 direction = Vector3.back;
    [SerializeField] private float speed = 30;

    void Update()
    {
        transform.Rotate(direction * Time.unscaledDeltaTime * speed);
    }
}
