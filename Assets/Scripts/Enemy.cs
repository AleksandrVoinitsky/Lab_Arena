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
    //private GameObject[] enemies;
    //private GameObject player;

    List<GameObject> enemies = new List<GameObject>();
    GameObject enemy;

    private float waitTimer;
    private bool stop;
    private float stanTimer;

    void OnDrawGizmos()
    {
         Gizmos.color = Color.yellow;
         Gizmos.DrawWireSphere(transform.position, ViewDistance);
         Gizmos.color = Color.red;
         Gizmos.DrawWireSphere(transform.position, AttackDistance);
    }

    void Start()
    {
        state = State.Idle;
        waitTimer = WaitTime;
        stanTimer = waitTimer * 5;
        StartCoroutine( UpdateBehaviour());
    }

    public IEnumerator UpdateBehaviour()
    {
        while (true)
        {
            if (DeadState()) { state = State.Death; }
            else if (HitState()) { state = State.Hit; }
            else if (AttackState()) { state = State.Attack; }
            else if (MovelState()) { state = State.Move; }
            else if (IdleState()) { state = State.Idle; }
            else { IdleState(); }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private bool DeadState()
    {
        if(Health <= 0)
        {
            return true;
        }
        return false;
    }

    private bool HitState()
    {
        return false;
    }

    private bool AttackState()
    {
        FindEnemy();
        GameObject tmp = FindClosestEnemy();
        if(Vector3.Distance(tmp.transform.position,transform.position) <= AttackDistance)
        {
            return true;
        }
        return false;
    }

    private bool MovelState()
    {
        return false;
    }

    private bool IdleState()
    {
        return false;
    }

    GameObject FindClosestEnemy()
    {
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in enemies)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                enemy = go;
                distance = curDistance;
            }
        }
        return enemy;
    }


    IEnumerator FindEnemy2()
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
                               // state = State.Attack;
                            }
                            else if (Vector3.Distance(transform.position, item.transform.position) <= ViewDistance)
                            {
                                targetTransform = item.transform;
                                //state = State.Move;
                            }
                        }
                    }
                }
            }
            yield return new WaitForSeconds(Random.Range(0.2f,1f));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.relativeVelocity.magnitude > rigidbody.mass * 20)
        {
            stop = true;
            state = State.Stun;
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
                else
                {
                    //state = State.Idle;
                }
                break;
            case State.Death:
                break;
            case State.Hit:
                break;
            case State.Idle:
                //StopCoroutine(FindEnemy2());
                target = transform.forward * 2;
                rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, Quaternion.Euler(new Vector3(0, NewRotation.y, 0)), AngularSpeed * Time.fixedDeltaTime);
                if (waitTimer > 0){waitTimer -= Time.deltaTime;}
                if (waitTimer <= 0)
                {
                    waitTimer = WaitTime;

                    if (Chance(20))
                    {
                        FindEnemy();
                    }
                    else
                    {
                        RandomizeTarget();
                    }
                    //StartCoroutine(FindEnemy2());
                }
                break;
            case State.Move:
                //if (targetTransform == null){state = State.Idle;}
                if (Vector3.Distance(transform.position, target) <= AttackDistance)
                {
                    //state = State.Attack;
                }
                rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, Quaternion.Euler(new Vector3(0, NewRotation.y, 0)), AngularSpeed * Time.fixedDeltaTime);
                rigidbody.velocity = transform.forward * Speed;
                break;
            case State.Stun:
                if (stanTimer > 0)
                { 
                    stanTimer -= Time.deltaTime;
                }
                if (stanTimer <= 0) 
                {
                    stop = false;
                    collider.enabled = true;
                    rigidbody.WakeUp();
                    //SetTarget();
                    stanTimer = waitTimer * 5; 
                }
                break;

            default:
                break;
        }
    }


    private void FindEnemy()
    {
        enemies.Clear();

        var en = FindObjectsOfType<Entity>();
        if (en.Length == 1) return;
        foreach (var e in en)
        {
            if (e != this)
                enemies.Add(e.gameObject);
        }
        enemy = enemies[Random.Range(0, enemies.Count)];
        SetTarget(enemy.transform.position);
    }

    private void RandomizeTarget()
    {
        targets = GameObject.FindGameObjectsWithTag("Target");
        SetTarget(targets[Random.Range(0, targets.Length)].transform.position);
    }

    private void SetTarget(Vector3 _target)
    {
        target = new Vector3(_target.x, this.transform.position.y, _target.z);
        //state = State.Move;
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
        //state = State.Idle;
    }

    public override void Damage(int damage)
    {
        base.Damage(damage);
        Instantiate(ParticleDamage, FirePoint.position, FirePoint.rotation);
    }
}


