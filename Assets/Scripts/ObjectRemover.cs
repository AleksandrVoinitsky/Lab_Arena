using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRemover : MonoBehaviour
{
    [SerializeField] float LifeTime = 1;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, LifeTime);
    }


}
