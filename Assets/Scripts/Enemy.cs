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
    [SerializeField] HpBar hpBar;

    private GameObject[] targets;
    //private GameObject[] enemies;
    //private GameObject player;

    List<GameObject> enemies = new List<GameObject>();
    GameObject enemy;
    Entity ent;

    private float waitTimer;
    private bool stop;
    private float stanTimer;
    private bool selectedTarget = false;

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
            UpdateState();
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void UpdateState()
    {
        if (DeadState()) { state = State.Death; }
        else if (HitState()) { state = State.Hit; }
        else if (AttackState()) { state = State.Attack; }
        else if (MovelState()) { state = State.Move; }
        else if (IdleState()) { state = State.Idle; }
        else { IdleState(); }
    }

    private bool DeadState()
    {
        if(Health <= 0)
        {
            collider.enabled = false;
            rigidbody.velocity = new Vector3(0, 0, 0);
            rigidbody.angularVelocity = new Vector3(0, 0, 0);
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
            if (tmp.GetComponent<Entity>().state != State.Death)
            {
                if (selectedTarget && (targetTransform.tag == "Player" || targetTransform.tag == "Enemy")) return true;
                if (selectedTarget == false)
                {
                    selectedTarget = true;
                    targetTransform = tmp.transform;
                    ent = tmp.GetComponent<Entity>();
                    return true;
                }
            }
        }
        return false;
    }

    private bool MovelState()
    {
        if (state != State.Move)
        {
            if (Chance(80))
            {
                FindEnemy();
                GameObject tmp = FindClosestEnemy();
                if (Vector3.Distance(tmp.transform.position, transform.position) <= ViewDistance)
                {
                    targetTransform = tmp.transform;
                    stop = false;
                    return true;
                }
            }
            else
            {
                targetTransform = RandomizeTarget();
                stop = false;
                return true;
            }
        }
        else
        {
            if(Vector3.Distance(target, transform.position) >= AttackDistance)
            {
                return true;
            }
            
        }
        return false;
    }

    private bool IdleState()
    {
        
        return true;
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

    private void OnCollisionEnter(Collision collision)
    { // на случай столкновений. Не активно
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
                rigidbody.rotation = Quaternion.Euler(0, NewRotation.y, 0);
                break;
            case State.Death:
                break;
            case State.Hit:
                break;
            case State.Idle:
                target = transform.forward * 2;
                rigidbody.rotation = Quaternion.Euler(0, NewRotation.y, 0);
                break;
            case State.Move:
                rigidbody.rotation = Quaternion.Euler(0, NewRotation.y, 0);
                rigidbody.velocity = transform.forward * Speed;
                break;
            case State.Stun:
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
    }

    private Transform RandomizeTarget()
    {
        targets = GameObject.FindGameObjectsWithTag("Target");
        return targets[Random.Range(0, targets.Length)].transform;
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
        /*
        if (selectedTarget == true)
        {
            if(targetTransform == null)
            {
                return;
            }
            if(targetTransform.tag == "Enemy" && targetTransform.TryGetComponent<Entity>(out ent))
            {
               if (ent.Damage(AttackPower))
               {
                   level += 1;
               }
            }  
        }
        */
        if (ent.Damage(AttackPower))
        {
            level += 1;
        }

        base.Hit();
        Instantiate(ParticleHit, FirePoint.position, FirePoint.rotation);
        hpBar.SetValue(Health,level);
    }

    public override void EndAttack()
    {
        selectedTarget = false;
        base.EndAttack();
    }

    public override bool Damage(int damage)
    {
        Health -= damage;
        hpBar.SetValue(Health, level);
        Instantiate(ParticleDamage, FirePoint.position, FirePoint.rotation);
        return base.Damage(damage);
    }
}


