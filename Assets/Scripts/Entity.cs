using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Entity : MonoBehaviour
{
    public State state;
    public int Health;
    public int AttackPower;
    public float MoveSpeed;
    public int level;

    public virtual bool Damage(int damage)
    {
        Health -= damage;
        if(Health<= 0)
        {
            Health = 0;
            state = State.Death;
            return false;
        }
        return true;
    }

    public virtual void Hit()
    {

    }

    public virtual void EndAttack()
    {

    }

    public virtual void MeleeAttack()
    {

    }

    public virtual void RangeAttack()
    {

    }

}


public enum State
{
    Idle,
    Move,
    Attack,
    Death,
    Hit,
    Stun,
    Mutate
}
