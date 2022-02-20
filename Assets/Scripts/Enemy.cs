using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : Entity
{
    [SerializeField] ModelManager model;
    [SerializeField] Transform target;
    [SerializeField] float ViewDistance = 5;
    [SerializeField] float AttackDistance = 2;
    [Space(10)]
    [SerializeField] Transform FirePoint;
    [SerializeField] GameObject ParticleDamage, deathSplash;
    [SerializeField] ProjectileScript laserProjectile;
    [SerializeField] HpBar hpBar;

    private GameObject[] targets;
    //private GameObject[] enemies;
    //private GameObject player;

    List<Entity> enemies = new List<Entity>();
    Entity enemyEntity;

    private bool selectedTarget = false;

    void Start()
    {
        state = State.Idle;
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
        maxHealth = model.GetHealth();
        health = maxHealth;
        isSpiked = model.IsSpiked();
        AttackDistance = model.GetDistance();
        hpBar.Init(health);
        isActive = true;
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
        if (tmp == null)
            return false;
        if (Vector3.Distance(new Vector3(tmp.transform.position.x, transform.position.y, tmp.transform.position.z), transform.position) <= AttackDistance)
        {
            if (tmp.GetComponent<Entity>().health > 0)
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
            if (Random.Range (1, 101) <= 85)
            {
                FindEnemy();
                var tmp = FindClosestEnemy();
                if (tmp == null)
                    return false;
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
        if (enemyEntity != null)
        {
            if (enemyEntity.health <= 0)
                enemyEntity = null;
        }
        if (enemyEntity == null)
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
                if (enemyEntity == null || Vector3.Distance (transform.position,
                    new Vector3 (target.transform.position.x, transform.position.y, target.transform.position.z)) > AttackDistance)
                {
                    transform.position = Vector3.MoveTowards(transform.position,
                        new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z), moveSpeed * Time.deltaTime);
                }
                break;
            case State.Stun:
                break;
            default:
                break;
        }
    }

    public void SetTarget (Entity _enemy)
    {
        if (state != State.Death)
        {
            enemyEntity = _enemy;
            target = _enemy.transform;
        }
    }

    public override void AddLevel()
    {
        level++;
        hpBar.SetValue(health, level);
        transform.DOScale(transform.localScale.x + 0.05f, 0.25f).SetUpdate(true);
    }


    private void FindEnemy()
    {
        enemies.Clear();
        var en = FindObjectsOfType<Entity>();
        if (en.Length <= 1)
            return;
        foreach (var e in en)
        {
            if (e != this && e.health > 0 && e.isActive)
                enemies.Add(e);
        }
    }

    private Transform RandomizeTarget()
    {
        return targets[Random.Range(0, targets.Length)].transform;
    }

    public override void MeleeAttack()
    {
        if (enemyEntity.Damage(damage, this))
        {
            AddLevel();
        }
        base.MeleeAttack();
        //Instantiate(ParticleMelee, FirePoint.position, FirePoint.rotation);
    }

    public override void RangeAttack()
    {
        var l = Instantiate(laserProjectile, model.transform.position + Vector3.up, transform.rotation);
        var targetVector = new Vector3(enemyEntity.transform.position.x, l.transform.position.y, enemyEntity.transform.position.z);
        l.SetDirection(targetVector, damage, this);
    }

    public override void Hit()
    {
        if (IsRanged())
            RangeAttack();
        else
            MeleeAttack();
        if (enemyEntity != null)
        {
            if (enemyEntity.GetComponent<Enemy>() != null)
                enemyEntity.GetComponent<Enemy>().SetTarget(this);
        }
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
        if (model != null)
            Instantiate(ParticleDamage, model.transform.position, transform.rotation);
        if (health <= 0)
        {
            if (model != null)
                Instantiate(deathSplash, model.transform.position + Vector3.up * 0.1f, transform.rotation);
            Invoke("Destruction", 10);
            if (model != null)
                model.Death();
            hpBar.Deinit();
        }
        return base.Damage(damage, attacker);
    }

    void Destruction()
    {
        transform.DOScale(0, 1f).OnComplete(() => Destroy(gameObject));
    }
}


