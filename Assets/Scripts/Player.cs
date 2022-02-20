using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using MoreMountains.NiceVibrations;

public class Player : Entity
{
    [SerializeField] ModelManager model;
    [SerializeField] Enemy enemy;
    [SerializeField] FloatingJoystick joystick;
    [SerializeField] Animator animator;
    [SerializeField] Vector3 MoveTarget;
    [SerializeField] Vector3 RotationTarget;
    [Space(10)]
    [SerializeField] float ViewDistance = 5;
    [SerializeField] float AttackDistance = 2;
    [Space(10)]
    [SerializeField] Transform FirePoint;
    [SerializeField] private ProjectileScript laserProjectile;
    [SerializeField] GameObject ParticleDamage, deathSplash;
    [SerializeField] private int gems, kills;
    [SerializeField] private TMP_Text gemCounter, killCounter;
    [SerializeField] private CanvasGroup damageCanvas;
    [SerializeField] HpBar hpBar;

    private bool ActivePlayer = false;
    private bool EnemyDetected = false;
    private bool jump = false;

    public void ActiveCharacterMovement(bool var)
    {
        ActivePlayer = var;
        /*if (var)
        {
            StartCoroutine(FindEnemy());
        }
        else
        {
            StopCoroutine(FindEnemy());
        }*/
    }

    private bool IsRanged()
    {
        if (model != null)
            return model.IsRanged();
        return false;
    }

    public void SetModel (ModelManager _model)
    {
        model = _model;
        moveSpeed = model.GetSpeed();
        maxHealth = model.GetHealth();
        health = maxHealth;
        isSpiked = model.IsSpiked();
        AttackDistance = model.GetDistance();
        hpBar.Init(health);
        StartCoroutine(FindEnemy());
    }

    public void InitKillCounter(int _amount)
    {
        killCounter.text = string.Format("{0}/{1}", kills, _amount);
        //gems = MainArena.Instance.GetGems();
        //gemCounter.text = gems.ToString();
    }

    private IEnumerator FindEnemy()
    {
        while (true)
        {
            if (joystick.Direction == Vector2.zero)
            {
                if (enemy == null)
                {
                    var enemies = FindObjectsOfType<Enemy>();
                    foreach (var e in enemies)
                    {
                        if (enemies.Length > 0)
                        {
                            if (enemy == null || Vector3.Distance(new Vector3(e.transform.position.x, transform.position.y, e.transform.position.z), transform.position) <
                                Vector3.Distance(new Vector3(enemy.transform.position.x, transform.position.y, enemy.transform.position.z), transform.position))
                            {
                                if (e.health > 0 && e.isActive)
                                    enemy = e;
                            }
                        }
                    }
                }
                else
                {
                    if (Vector3.Distance(transform.position, new Vector3 (enemy.transform.position.x, transform.position.y, enemy.transform.position.z)) <= AttackDistance)
                    {
                        state = State.Attack;
                        if (enemy.health <= 0 || !enemy.isActive)
                        {
                            enemy = null;
                            state = State.Idle;
                        }
                    }
                }
            }
            else
            {
                enemy = null;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private bool CanMove(Vector3 direction)
    {
        if (Physics.Raycast(transform.position + Vector3.up + transform.forward, direction, 5, LayerMask.GetMask("Location")))
            return true;
        return false;
    }

    private void Update()
    {
        //if (!ActivePlayer)
        //  return;

        //Vector3 lookDirection = new Vector3(rigidbody.position.x + inputX, 0.0f, rigidbody.position.z + inputZ);
        //JoystickMagnitude = joystick.Direction.magnitude;
        if (!MainArena.Instance.victory)
        {
            if (joystick.Direction != Vector2.zero)
            {
                enemy = null;
                float inputZ = joystick.Direction.y;
                float inputX = joystick.Direction.x;

                Vector3 lookDirection = new Vector3(inputX, 0, inputZ);
                Quaternion lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);

                float step = 10 * Time.deltaTime;

                transform.rotation = Quaternion.RotateTowards(lookRotation, transform.rotation, step);
                if (CanMove((transform.forward * 2.5f + Vector3.down * 2 - Vector3.up).normalized))
                    transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
                state = State.Move;
            }
            else
            {
                if (enemy == null)
                    state = State.Idle;
                else
                {
                    Quaternion lookRotation = Quaternion.LookRotation(enemy.transform.position - transform.position, Vector3.up);
                    lookRotation.x = 0;
                    lookRotation.z = 0;
                    transform.rotation = Quaternion.RotateTowards(lookRotation, transform.rotation, 30 * Time.deltaTime);
                }
                /*Vector3 direction;
                if (EnemyDetected)
                {
                    direction = RotationTarget - transform.position;
                    // Sphere.transform.position = RotationTarget;
                }
                else
                {
                    //direction = MoveTarget - transform.position;
                    //Sphere.transform.position = MoveTarget;
                    //rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, new Vector3(0, 0, 0), Time.fixedDeltaTime * Speed);

                }
                Quaternion rotation = Quaternion.LookRotation(direction);
                Vector3 NewRotation = rotation.eulerAngles;
                transform.rotation = Quaternion.Slerp(rigidbody.rotation, Quaternion.Euler(new Vector3(0, NewRotation.y, 0)), AngularSpeed * Time.fixedDeltaTime);*/

            }
        }
        else
        {
            if (state != State.Death)
            {

            }
        }

        /*if (joystick.OnDown)
        {
            MoveTarget = lookDirection;
            Vector3 direction = MoveTarget - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            Vector3 NewRotation = rotation.eulerAngles;
            rigidbody.rotation = Quaternion.Euler(0, NewRotation.y, 0);
            if (JoystickMagnitude > 0)
            {
                rigidbody.velocity = transform.forward * Speed * JoystickMagnitude;
            }
            else
            {
                rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, new Vector3(0, 0, 0), Time.fixedDeltaTime * Speed);
            }
           // Sphere.transform.position = MoveTarget;
            state = State.Move;
        }
        else
        {
            Vector3 direction;
            if (EnemyDetected)
            {
                direction = RotationTarget - transform.position;
               // Sphere.transform.position = RotationTarget;
            }
            else
            {
                direction = MoveTarget - transform.position;
                //Sphere.transform.position = MoveTarget;
                rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, new Vector3(0, 0, 0), Time.fixedDeltaTime * Speed);
                state = State.Idle;
            }
            Quaternion rotation = Quaternion.LookRotation(direction);
            Vector3 NewRotation = rotation.eulerAngles;
            rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, Quaternion.Euler(new Vector3(0, NewRotation.y, 0)), AngularSpeed * Time.fixedDeltaTime);
        }*/
    }

    public override void MeleeAttack()
    {
        if (enemy != null)
        {
            MMVibrationManager.Haptic(HapticTypes.LightImpact);
            if (enemy.Damage(damage, this))
            {
                AddLevel();
                AddKills();
                AddGems(5);
            }
            else
            {
                enemy.SetTarget(this);
            }
        }
        base.MeleeAttack();
        //Instantiate(ParticleMelee, FirePoint.position, FirePoint.rotation);
    }

    public override void AddLevel()
    {
        level++;
        transform.DOScale(transform.localScale.x + 0.03f, 0.25f).SetUpdate(true);
        hpBar.SetValue(health, level);
    }

    public void AddKills()
    {
        kills++;
        killCounter.text = string.Format("{0}/{1}", kills, MainArena.Instance.maxEnemiesCount);
        killCounter.transform.DOScale(1.1f, 0.3f).OnComplete(() => killCounter.transform.DOScale(1f, 0.2f));
        if (kills >= MainArena.Instance.maxEnemiesCount)
        {
            hpBar.Deinit();
            MainArena.Instance.Win();
        }
        if (kills > MainArena.Instance.maxEnemiesCount)
            kills = MainArena.Instance.maxEnemiesCount;
    }

    public void AddGems (int _amount)
    {
        gemCounter.transform.DOScale (1.25f, _amount * 0.05f).OnComplete (() => gemCounter.transform.DOScale(1f, 0.1f));
        MainArena.Instance.AddGems(_amount);
        StartCoroutine(AddingGems(_amount));
    }

    public int GetGems ()
    {
        return gems;
    }

    public void SpendGems (int _amount)
    {
        gems -= _amount;
        MainArena.Instance.SpendGems(_amount);
        gemCounter.text = gems.ToString();
    }

    private IEnumerator AddingGems (int _amount)
    {
        _amount--;
        gems++;
        gemCounter.text = gems.ToString();
        yield return new WaitForSecondsRealtime(0.05f);
        if (_amount > 0)
            StartCoroutine(AddingGems(_amount));
        else
            UpgradeHandler.Instance.CheckUpgrades(gems);
    }

    public void Upgrade(UpgradeType _type)
    {
        switch (_type)
        {
            case UpgradeType.HEALTH:
                maxHealth += 20;
                health = maxHealth;
                hpBar.SetValue(health, level);
                break;
            case UpgradeType.DAMAGE:
                damage += 5;
                break;
            case UpgradeType.SPEED:
                moveSpeed += 0.5f;
                break;
        }
    }

    public override void RangeAttack()
    {
        if (enemy != null)
        {
            var l = Instantiate(laserProjectile, model.transform.position + Vector3.up, transform.rotation);
            var targetVector = new Vector3(enemy.transform.position.x, l.transform.position.y, enemy.transform.position.z);
            l.SetDirection(targetVector, damage, this);
            /*l.transform.DOMove(targetVector, 0.25f).OnComplete(() =>
            {
                Debug.Log(Vector3.Distance(l.transform.position, targetVector));
                if (Vector3.Distance (l.transform.position, targetVector) <= 0.25f)
                {
                    if (enemy.Damage(AttackPower, this))
                    {
                        level++;
                    }
                    else
                    {
                        enemy.SetTarget(this);
                    }
                }
                StartCoroutine(LaserDestruction(l));
            });*/
        }
        //Instantiate(ParticleRange, FirePoint.position, FirePoint.rotation);
    }

    public override void Hit()
    {
        if (IsRanged())
            RangeAttack();
        else
            MeleeAttack();
        base.Hit();
        //Instantiate(ParticleHit, FirePoint.position, FirePoint.rotation);
    }

    public override void EndAttack()
    {
        if (enemy != null)
        {
            if (Vector3.Distance(transform.position, new Vector3(enemy.transform.position.x, transform.position.y, enemy.transform.position.z)) <= AttackDistance)
            {
                enemy = null;
                state = State.Idle;
            }
            if (enemy.health <= 0)
            {
                enemy = null;
                state = State.Idle;
            }
        }
        base.EndAttack();
    }

    public override bool Damage(int damage, Entity attacker)
    {
        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        health -= damage;
        hpBar.SetValue(health, level);
        if (health <= 0)
        {
            Time.timeScale = 0.25f;
            MainArena.Instance.Defeat();
            model.Death();
            state = State.Death;
            Instantiate(deathSplash, model.transform.position + Vector3.up * 0.1f, transform.rotation);
        }
        else
        {
            damageCanvas.gameObject.SetActive(true);
            damageCanvas.DOFade(0, 0.5f).OnComplete(() =>
            {
                damageCanvas.gameObject.SetActive(false);
                damageCanvas.alpha = 1;
            });
            Camera.main.DOShakePosition(0.1f, 0.3f);
        }
        Instantiate(ParticleDamage, transform.position, Quaternion.Euler(0, 270, 0));
        return base.Damage(damage, attacker);
    }
}
