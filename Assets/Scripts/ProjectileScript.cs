using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;

public class ProjectileScript : MonoBehaviour
{
    private int damage;
    private Entity owner;
    private bool stopped;

    public void SetDirection (Vector3 _target, int _damage, Entity _owner)
    {
        damage = _damage;
        transform.LookAt(_target);
        owner = _owner;
    }

    private void Update()
    {
        if (!stopped)
            transform.Translate(Vector3.forward * Time.deltaTime * 50);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Entity>() != owner)
        {
            if (other.GetComponent<Enemy>() != null && !stopped)
            {
                if (other.GetComponent<Enemy>().health > 0)
                {
                    Stop();
                    if (owner.GetComponent<Player>() != null)
                        MMVibrationManager.Haptic(HapticTypes.LightImpact);
                    if (other.GetComponent<Enemy>().Damage(damage, owner))
                    {
                        if (owner.GetComponent<Player>() != null)
                        {
                            owner.GetComponent<Player>().AddGems(5);
                            owner.GetComponent<Player>().AddKills();
                        }
                        owner.AddLevel();
                    }
                    other.GetComponent<Enemy>().SetTarget(owner);
                }
            }
            if (other.GetComponent<Player>() != null && !stopped)
            {
                if (other.GetComponent<Player>().health > 0)
                {
                    if (other.GetComponent<Player>().Damage(damage, owner))
                    {
                        owner.AddLevel();
                    }
                }
            }
        }
    }

    private void Stop()
    {
        stopped = true;
        Invoke("Destruction", 3f);
    }

    void Destruction()
    {
        Destroy(gameObject);
    }
}
