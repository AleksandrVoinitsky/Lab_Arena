using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GemScript : MonoBehaviour
{
    [SerializeField] private float distance = 3.5f;
    private Transform player;
    private bool moving;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!moving)
        {
            if (Vector3.Distance(transform.position, player.position) <= distance)
            {
                moving = true;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, 10 * Time.deltaTime);
            if (Vector3.Distance (transform.position, player.position) <= 0)
            {
                player.GetComponent<Player>().AddHealth();
                player.GetComponent<Player>().AddGems(1);
                Destroy(gameObject);
            }
        }
    }
}
