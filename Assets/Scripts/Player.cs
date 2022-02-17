using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public GameObject Sphere;
    [SerializeField] GameObject Target;
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] CapsuleCollider capsuleCollider;
    [SerializeField] FloatingJoystick joystick;
    [SerializeField] Animator animator;
    [SerializeField] Vector3 MoveTarget;
    [SerializeField] Vector3 RotationTarget;
    [Space(10)]
    [SerializeField] float ViewDistance = 5;
    [SerializeField] float AttackDistance = 2;
    [SerializeField] float Speed;
    [SerializeField] float AngularSpeed;
    [Space(10)]
    [SerializeField] Transform FirePoint;
    [SerializeField] Transform DamagePoint;
    [SerializeField] GameObject ParticleMelee;
    [SerializeField] GameObject ParticleRange;
    [SerializeField] GameObject ParticleHit;
    [SerializeField] GameObject ParticleDamage;

    private bool ActivePlayer = false;
    private bool EnemyDetected = false;
    private bool jump = false;

    private void Start()
    {

    }

    public void ActiveCharaterMovement(bool var)
    {
        ActivePlayer = var;
        if (var)
        {
            StartCoroutine(FindEnemy());
        }
        else
        {
            StopCoroutine(FindEnemy());
        }
    }

    IEnumerator FindEnemy()
    {
        while (true)
        {
            Collider[] Enemies = Physics.OverlapSphere(transform.position, ViewDistance);
            foreach (var item in Enemies)
            {
                if (item != capsuleCollider && Enemies.Length > 0)
                {
                    if (item.tag == "Enemy")
                    {
                        if (Vector3.Distance(transform.position, item.transform.position) <= AttackDistance)
                        {
                            EnemyDetected = true;
                            RotationTarget = item.transform.position;
                            if(Target == null)
                            {
                                Target = item.transform.gameObject;
                            }
                            if (Vector3.Distance(item.transform.position, transform.position) < Vector3.Distance(Target.transform.position, transform.position))
                            {
                                Target = item.transform.gameObject;
                            }
                            state = State.Attack;
                            RotationTarget = Target.transform.position;
                        }

                        if (Vector3.Distance(transform.position, item.transform.position) <= ViewDistance || Vector3.Distance(transform.position, item.transform.position) > AttackDistance )
                        {
                            EnemyDetected = true;
                            RotationTarget = item.transform.position;
                        }
                       
                    }
                }
                else
                {
                    EnemyDetected = false;
                    RotationTarget = MoveTarget;
                }
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
        if (joystick.Direction != Vector2.zero)
        {
            Debug.Log("1");
            float inputZ = joystick.Direction.y;
            float inputX = joystick.Direction.x;

            Vector3 lookDirection = new Vector3(inputX, 0, inputZ);
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);

            float step = 10 * Time.deltaTime;

            transform.rotation = Quaternion.RotateTowards(lookRotation, transform.rotation, step);
            if (CanMove((transform.forward * 2.5f + Vector3.down * 2 - Vector3.up).normalized))
                transform.Translate(Vector3.forward * Time.deltaTime * Speed);
            state = State.Move;
        }
        else
        {
            state = State.Idle;
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
        
    }

    public override bool Damage(int damage)
    {
        
        Instantiate(ParticleDamage, FirePoint.position, FirePoint.rotation);
        return base.Damage(damage);
    }
}
