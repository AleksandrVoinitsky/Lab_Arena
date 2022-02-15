using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] Enemies;
    [SerializeField] float WaitTime;
    private int counter;
    void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(WaitTime);
            Instantiate(Enemies[counter], transform.position, transform.rotation);
            if(counter == Enemies.Length - 1)
            {
                counter = 0;
            }
            else
            {
                counter++;
            }
        }
    }
}
