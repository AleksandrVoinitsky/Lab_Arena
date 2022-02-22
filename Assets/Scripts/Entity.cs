using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Entity : MonoBehaviour
{
    public bool isActive;
    public State state;
    public bool isSpiked;
    public int health, maxHealth;
    public int damage;
    public float moveSpeed;
    public int level = 1;

    public virtual bool Damage(int damage, Entity attacker = null)
    {
       // health -= damage;
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

    public virtual void AddLevel()
    {
        level++;
        transform.DOScale(transform.localScale.x + 0.1f, 0.25f).SetUpdate (true);
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
    Mutate,
    Dance
}
