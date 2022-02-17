using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public State state;
    public bool isSpiked;
    public int health;
    public int AttackPower;
    public float moveSpeed;
    public int level = 1;

    public virtual bool Damage(int damage, Entity attacker = null)
    {
        health -= damage;
        if (attacker != null)
        {
            if (isSpiked)
                attacker.Damage(damage / 2);
        }
        if (health <= 0)
        {
            health = 0;
            state = State.Death;
        }
        return health <= 0;
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
