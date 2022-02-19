using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropSpawner : MonoBehaviour
{
    [SerializeField] private float spawnTimer;
    [SerializeField] private List<PropScript> props;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Random.Range(3, 7); i++)
            Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            Spawn();
            spawnTimer = Random.Range(3, 7);
        }
    }

    private void Spawn()
    {
        var propToSpawn = props[Random.Range(0, props.Count)];
        Instantiate(propToSpawn, new Vector3(Random.Range(-16f, 16f), 0, Random.Range(-16, 16f)), propToSpawn.transform.rotation);
    }
}
