using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] ModelManager model;
    [SerializeField] Transform target;
    [SerializeField] private float waitTimer, maxWaitTimer = 2;
    [SerializeField] float ViewDistance = 5;
    [SerializeField] float AttackDistance = 2;
    [Space(10)]
    [SerializeField] Transform FirePoint;
    [SerializeField] GameObject ParticleDamage;
    [SerializeField] HpBar hpBar;

    private GameObject[] targets;
    //private GameObject[] enemies;
    //private GameObject player;

    List<Entity> enemies = new List<Entity>();
    Entity enemyEntity;

    Vector3 direction;
    private bool selectedTarget = false;

    void Start()
    {
        state = State.Idle;
        waitTimer = maxWaitTimer;
        Invoke("Init", 1f);
        targets = GameObject.FindGameObjectsWithTag("Target");
        StartCoroutine(UpdateBehaviour());
    }

    void Init()
    {
        SetModel(transform.GetComponentInChildren<ModelManager>());
    }

    public IEnumerator UpdateBehaviour()
    {
        while (true)
        {
            UpdateState();
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void SetModel(ModelManager _model)
    {
        model = _model;
        moveSpeed = model.GetSpeed();
        health = model.GetHealth();
        isSpiked = model.IsSpiked();
        AttackDistance = model.GetDistance();
        hpBar.Init(health);
    }

    private bool IsRanged()
    {
        if (model != null)
            return model.IsRanged();
        return false;
    }

    public void UpdateState()
    {
        if (DeadState())
            state = State.Death;
        else if (HitState())
            state = State.Hit;
        else if (AttackState())
            state = State.Attack;
        else if (MovingState())
            state = State.Move;
        else
            state = State.Idle;
    }

    private bool DeadState()
    {
        return health <= 0;
    }

    private bool HitState()
    {
        return false;
    }

    private bool AttackState()
    {
        FindEnemy();
        var tmp = FindClosestEnemy();
        if (Vector3.Distance(tmp.transform.position,transform.position) <= AttackDistance)
        {
            if (tmp.GetComponent<Entity>().state != State.Death)
            {
                if (selectedTarget && target.GetComponent<Entity>() != null)
                    return true;
                if (!selectedTarget)
                {
                    selectedTarget = true;
                    target = tmp.transform;
                    enemyEntity = tmp.GetComponent<Entity>();
                    return true;
                }
            }
        }
        return false;
    }

    private bool MovingState()
    {
        if (state != State.Move)
        {
            if (Random.Range (1, 101) <= 80)
            {
                FindEnemy();
                var tmp = FindClosestEnemy();
                if (Vector3.Distance(tmp.transform.position, transform.position) <= ViewDistance)
                {
                    target = tmp.transform;
                    return true;
                }
            }
            else
            {
                target = RandomizeTarget();
                return true;
            }
        }
        else
        {
            if (Vector3.Distance(target.position, transform.position) >= AttackDistance)
            {
                return true;
            }
        }
        return false;
    }

    Entity FindClosestEnemy()
    {
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (var go in enemies)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                enemyEntity = go;
                distance = curDistance;
            }
        }
        return enemyEntity;
    }

    private void FixedUpdate()
    {
       /*if (target != null)
            direction = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 NewRotation = rotation.eulerAngles;*/

        switch (state)
        {
            case State.Attack:
                transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                break;
            case State.Death:
                break;
            case State.Hit:
                break;
            case State.Idle:
                //target = transform.forward * 2;
                //transform.rotation = Quaternion.Euler(0, NewRotation.y, 0);
                break;
            case State.Move:
                transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                transform.position = Vector3.MoveTowards (transform.position,
                    new Vector3 (target.transform.position.x, transform.position.y, target.transform.position.z), moveSpeed * Time.deltaTime);
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
        if (en.Length <= 1)
            return;
        foreach (var e in en)
        {
            if (e != this && e.health > 0)
                enemies.Add(e);
        }
    }

    private Transform RandomizeTarget()
    {
        return targets[Random.Range(0, targets.Length)].transform;
    }

    public override void MeleeAttack()
    {
        if (enemyEntity.Damage(AttackPower, this))
        {
            level++;
        }
        base.MeleeAttack();
        //Instantiate(ParticleMelee, FirePoint.position, FirePoint.rotation);
    }

    public override void RangeAttack()
    {
        if (enemyEntity.Damage(AttackPower, this))
        {
            level++;
        }
        base.RangeAttack();
        //Instantiate(ParticleRange, FirePoint.position, FirePoint.rotation);
    }

    public override void Hit()
    {
        if (IsRanged())
            RangeAttack();
        else
            MeleeAttack();
        base.Hit();
        hpBar.SetValue(health, level);
    }

    public override void EndAttack()
    {
        selectedTarget = false;
        base.EndAttack();
    }

    public override bool Damage (int damage, Entity attacker)
    {
        health -= damage;
        hpBar.SetValue(health, level);
        Instantiate(ParticleDamage, transform.position, ParticleDamage.transform.rotation);
        return base.Damage(damage, attacker);
    }
}


