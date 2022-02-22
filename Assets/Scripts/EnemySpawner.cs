using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Enemy enemy;
    [SerializeField] private GameObject spawnEffect;
    [SerializeField] float waitTimer;

    void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTimer);
            if (!MainArena.Instance.victory)
            {
                Instantiate(enemy, transform.position - Vector3.up * 3, transform.rotation);
                Instantiate(spawnEffect, transform.position, transform.rotation);
            }
        }
    }
}
