using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSpawner : MonoBehaviour
{
    [SerializeField] private float gemTimer;
    [SerializeField] private GemScript gem;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gemTimer -= Time.deltaTime;
        if (gemTimer <= 0)
        {
            Instantiate(gem, new Vector3(Random.Range(-18, 18f), 0.5f, Random.Range(-18, 18f)), gem.transform.rotation);
            gemTimer = Random.Range(3, 7);
        }
    }
}
