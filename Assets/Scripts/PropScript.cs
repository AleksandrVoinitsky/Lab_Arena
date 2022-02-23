using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PropScript : Entity
{
    [SerializeField] private GemScript gem;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private List<Material> materials;
    private int chosenMat;
    private bool destroyed;

    private void Awake()
    {
        if (materials.Count > 0)
        {
            chosenMat = Random.Range(0, materials.Count);
            GetComponent<MeshRenderer>().material = materials[chosenMat];
        }
        health = 1;
        isActive = true;
        state = State.Idle;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Enemy>() != null)
        {
            destroyed = true;
            StartCoroutine(GemSpawner(Random.Range(3, 7)));
        }
    }


    public override bool Damage(int damage, Entity attacker = null)
    {
        //return base.Damage(damage, attacker);
        health = 0;
        if (!destroyed)
        {
            destroyed = true;
            StartCoroutine(GemSpawner(Random.Range(3, 7)));
        }

        return true;
    }


    private IEnumerator GemSpawner(int _money)
    {
        state = State.Death;
        isActive = false ;
        while (_money > 0)
        {
            var m = Instantiate(gem, transform.position, gem.transform.rotation);
            var startPos = m.transform.position;
            float randomX = Random.Range(-2.25f, 2.25f);
            float randomZ = Random.Range(-2.25f, 2.25f);
            float randomY = Random.Range(1.5f, 2.25f);
            Vector3[] path = new[] { new Vector3(startPos.x + 0.2f * randomX, randomY * 0.5f, startPos.z + 0.2f * randomZ),
                new Vector3(startPos.x + 0.5f * randomX, randomY, startPos.z + 0.5f * randomZ),
                new Vector3(startPos.x + 0.8f * randomX, randomY * 0.5f, startPos.z + 0.8f * randomZ),
            new Vector3(startPos.x + randomX, 0.25f, startPos.z + randomZ)};
            m.transform.DOPath(path, 0.8f);
            _money--;
        }
        yield return new WaitForSecondsRealtime(0.03f);
        if (_money > 0)
            StartCoroutine(GemSpawner(_money));
        else
        {
            var p  = Instantiate(particles, transform.position + Vector3.up * 0.75f, particles.transform.rotation);
            if (materials.Count > 0)
            {
                p.GetComponent<Renderer>().material.color = materials[chosenMat].color;
            }
            Destroy(gameObject);
        }
    }
}
