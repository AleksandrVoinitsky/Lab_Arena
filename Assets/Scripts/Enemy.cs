using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] Transform targetTransform;
    [SerializeField] Vector3 target;
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] CapsuleCollider collider;
    [SerializeField] float Speed = 2;
    [SerializeField] float AngularSpeed = 20;
    [SerializeField] float WaitTime = 2;
    [SerializeField] float StopDistance = 2;
    [SerializeField] float ViewDistance = 5;
    [SerializeField] float AttackDistance = 2;
    [Space(10)]
    [SerializeField] Transform FirePoint;
    [SerializeField] Transform DamagePoint;
    [SerializeField] GameObject ParticleMelee;
    [SerializeField] GameObject ParticleRange;
    [SerializeField] GameObject ParticleHit;
    [SerializeField] GameObject ParticleDamage;

    private GameObject[] targets;
    private GameObject[] enemies;
    private GameObject player;

    private float waitTimer;
    private bool stop;
    private float stanTimer;
    private float attackTimer;

    void OnDrawGizmos()
    {
         Gizmos.color = Color.yellow;
         Gizmos.DrawWireSphere(transform.position, ViewDistance);
         Gizmos.color = Color.red;
         Gizmos.DrawWireSphere(transform.position, AttackDistance);
    }

    void Start()
    {
        waitTimer = WaitTime;
        stanTimer = waitTimer * 5;
       // StartCoroutine(FindEnemy());
        SetTarget();
    }

    IEnumerator FindEnemy()
    {
        while (true)
        {
            if (!stop)
            {
                Collider[] Enemies = Physics.OverlapSphere(transform.position, ViewDistance);
                foreach (var item in Enemies)
                {
                    if (item != collider)
                    {
                        if (item.tag == "Player" || item.tag == "Enemy")
                        {
                            if (Vector3.Distance(transform.position, item.transform.position) <= AttackDistance)
                            {
                                targetTransform = item.transform;
                                state = State.Attack;
                            }
                            else if (Vector3.Distance(transform.position, item.transform.position) <= ViewDistance)
                            {
                                targetTransform = item.transform;
                                state = State.Move;
                            }
                        }
                    }
                }
            }
            yield return new WaitForSeconds(Random.Range(0.2f,0.7f));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.relativeVelocity.magnitude > rigidbody.mass * 20)
        {
            stop = true;
            state = State.Stan;
            collider.enabled = false;
            rigidbody.velocity = new Vector3(0, 0, 0);
            rigidbody.angularVelocity = new Vector3(0, 0, 0);
        }
    }

    private void FixedUpdate()
    {
        if(targetTransform != null)
        {
            target = targetTransform.position;
            
        }

        Vector3 direction = target - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 NewRotation = rotation.eulerAngles;

        switch (state)
        {
            case State.Attack:
                if (Vector3.Distance(transform.position, target) <= AttackDistance)
                {
                    rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, Quaternion.Euler(new Vector3(0, NewRotation.y, 0)), AngularSpeed * Time.fixedDeltaTime);
                }
                else{state = State.Idle;}
                break;
            case State.Death:
                break;
            case State.Hit:
                break;
            case State.Idle:
                //StartCoroutine(FindEnemy());
                target = transform.forward * 2;
                rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, Quaternion.Euler(new Vector3(0, NewRotation.y, 0)), AngularSpeed * Time.fixedDeltaTime);
                if (waitTimer > 0){waitTimer -= Time.deltaTime;}
                if (waitTimer <= 0){SetTarget();waitTimer = WaitTime;/* StopCoroutine(FindEnemy()); */}
                break;
            case State.Move:
                if (targetTransform == null){state = State.Idle;}
                if (Vector3.Distance(transform.position, target) <= AttackDistance){state = State.Attack;}
                rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, Quaternion.Euler(new Vector3(0, NewRotation.y, 0)), AngularSpeed * Time.fixedDeltaTime);
                rigidbody.velocity = transform.forward * Speed;
                break;
            case State.Stan:
                if (stanTimer > 0)
                { 
                    stanTimer -= Time.deltaTime;
                }
                if (stanTimer <= 0) 
                {
                    stop = false;
                    collider.enabled = true;
                    rigidbody.WakeUp();
                    SetTarget(); stanTimer = waitTimer * 5; 
                }
                break;

            default:
                break;
        }
    }

    void SetTarget()
    {
       
        player = GameObject.FindGameObjectWithTag("Player");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        targets = GameObject.FindGameObjectsWithTag("Target");
        if (Chance(10))
        {
            if(player != null)
            {
                targetTransform = player.transform;
                state = State.Move;
                return;
            } 
        }
        if (Chance(20))
        {
            if (enemies.Length > 0)
            {
                targetTransform = enemies[Random.Range(0, enemies.Length)].transform;
                if(targetTransform == transform)
                {
                    state = State.Idle;
                    return;
                }
                state = State.Move;
                return;
            }
        }
        if (Chance(60))
        {
            if (targets.Length > 0)
            {
                targetTransform = targets[Random.Range(0, targets.Length)].transform;
                state = State.Move;
                return;
            }
        }
        state = State.Idle;
    }

    bool Chance(int percent)
    {
        if (percent >= 100) return true;
        return Random.Range(1, 101) < percent ? true : false;
    }

    public override void MeleeAttack()
    {
        base.MeleeAttack();
        Instantiate(ParticleMelee, FirePoint.position, FirePoint.rotation);
    }

    public override void RangeAttack()
    {
        base.RangeAttack();
        Instantiate(ParticleRange, FirePoint.position, FirePoint.rotation);
    }

    public override void Hit()
    {
        base.Hit();
        Instantiate(ParticleHit, FirePoint.position, FirePoint.rotation);
    }

    public override void EndAttack()
    {
        base.EndAttack();
        SetTarget();
    }

    public override void Damage(int damage)
    {
        base.Damage(damage);
        Instantiate(ParticleDamage, FirePoint.position, FirePoint.rotation);
    }
}


