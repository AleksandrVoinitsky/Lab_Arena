using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ModelManager : MonoBehaviour
{
    [SerializeField] string EntityStateMonitor;
    [SerializeField] string BaseAnimationIdle;
    [SerializeField] string BaseAnimationRun;
    [SerializeField] string BaseAnimationAttack;
    [SerializeField] string BaseAnimationDeath;
    [SerializeField] string BaseAnimationHit;
    [SerializeField] string BaseAnimationStan;
    [SerializeField] bool StartRandomMutation;
    [Space(10)]
    [SerializeField] GameObject BaseBody;
    [SerializeField] GameObject BaseLags;
    public Parts[] parts;
    public List<Parts> ActiveParts = new List<Parts>();
    Dictionary<State, string> animDictionary = new Dictionary<State, string>();
    Animator animator;
    Entity entity;



    private void Awake()
    {
        animator = GetComponent<Animator>();
        entity = transform.parent.GetComponent<Entity>();
        animDictionary.Add(State.Idle, BaseAnimationIdle);
        animDictionary.Add(State.Move, BaseAnimationRun);
        animDictionary.Add(State.Attack, BaseAnimationAttack);
        animDictionary.Add(State.Death, BaseAnimationDeath);
        animDictionary.Add(State.Hit, BaseAnimationHit);
        animDictionary.Add(State.Stan, BaseAnimationStan);
        foreach (var item in parts)
        {
            SetSwichPart(item.partType, item.StartActive);
        }
    }

    private void Start()
    {
       
        if(entity != null)
        {
            StartCoroutine(AnimationPlayer());
        }

        if (StartRandomMutation)
        {
            SetSwichPart(parts[Random.Range(0, parts.Length - 1)].partType, true);
        }
    }

    public void Play(State state)
    {
        animator.Play(animDictionary[state]);
    }

    public void SetSwichPart(MutantParts partType,bool active)
    {
        //State temp;
        //string animTemp;


        foreach (var item in parts)
        {
            if(item.partType == partType)
            {
                if(item.PartReferense.active != active)
                {

                    if (partType == MutantParts.Ghost || partType == MutantParts.SpiderFoots)
                    {
                        BaseLags.SetActive(!active);
                    }
                    if (partType == MutantParts.Armor)
                    {
                        BaseBody.SetActive(!active);
                    }

                    item.PartReferense.SetActive(active);
                    SkinnedMeshRenderer skin = item.PartReferense.GetComponent<SkinnedMeshRenderer>();

                    if (active)
                    {
                        skin.SetBlendShapeWeight(0, 0);
                        StartCoroutine(ActivatePart(skin));
                        ActiveParts.Add(item);
                        if(item.AnimationName != "")
                        {

                            //animDictionary.Remove(item.StateAnimation);
                            //animDictionary.Add(item.StateAnimation, item.AnimationName);
                            animDictionary[item.StateAnimation] = item.AnimationName;
                            return;
                        }
                    }
                    else
                    {
                        skin.SetBlendShapeWeight(0, 0);
                        ActiveParts.Remove(item);
                    }
                }
            }
        }
        
    }

    IEnumerator ActivatePart(SkinnedMeshRenderer skin)
    {
        while (skin.GetBlendShapeWeight(0) < 100)
        {
            skin.SetBlendShapeWeight(0, skin.GetBlendShapeWeight(0) + 1) ;
            yield return new WaitForSeconds(0.02f);
        }
    }

    IEnumerator AnimationPlayer()
    {
        while(true)
        {
            EntityStateMonitor = entity.state.ToString();
            Play(entity.state);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void AnimationEventHit()
    {
        entity.Hit();
    }

    public void AnimationEventAttackFinished()
    {
        entity.EndAttack();
    }
}

[System.Serializable]
public struct Parts
{
    public MutantParts partType;
    public GameObject PartReferense;
    public State StateAnimation;
    public string AnimationName;
    public bool StartActive;
}

