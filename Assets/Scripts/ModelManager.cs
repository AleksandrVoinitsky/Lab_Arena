using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public enum ModelType { PLAYER, ENEMY }

public class ModelManager : MonoBehaviour
{
    [SerializeField] private ModelType type;
    [SerializeField] string EntityStateMonitor;
    [SerializeField] string BaseAnimationIdle;
    [SerializeField] string BaseAnimationRun;
    [SerializeField] string BaseAnimationAttack;
    [SerializeField] string BaseAnimationDeath;
    [SerializeField] string BaseAnimationHit;
    [SerializeField] string BaseAnimationStun;
    [SerializeField] string BaseAnimatuonMutation;
    public bool isActive;
    [SerializeField] bool StartRandomMutation;
    [Space(10)]
    [SerializeField] SkinnedMeshRenderer BaseBody, BaseCostume;
    [SerializeField] SkinnedMeshRenderer BaseLegs, SpiderLegs, EyeMask;
    [SerializeField] SkinnedMeshRenderer Wings, Tentacles, Blades, Ghost;
    [SerializeField] private List<Material> enemyMaterials, costumeMaterials, extraMaterials;
    [SerializeField] private Material deathMaterial;
    public Parts[] parts;
    public List<Parts> ActiveParts = new List<Parts>();
    Dictionary<State, string> animDictionary = new Dictionary<State, string>();
    Animator animator;
    Entity entity;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (SceneManager.GetActiveScene().name == "Laboratory")
            animator.Play("Swim");
        entity = transform.parent.GetComponent<Entity>();
        animDictionary.Add(State.Idle, BaseAnimationIdle);
        animDictionary.Add(State.Move, BaseAnimationRun);
        animDictionary.Add(State.Attack, BaseAnimationAttack);
        animDictionary.Add(State.Death, BaseAnimationDeath);
        animDictionary.Add(State.Hit, BaseAnimationHit);
        animDictionary.Add(State.Stun, BaseAnimationStun);
        animDictionary.Add(State.Mutate, BaseAnimatuonMutation);
        foreach (var item in parts)
        {
            SetSwitchPart(item.partType, item.StartActive);
        }
        if (type == ModelType.ENEMY)
        {
            transform.DOMoveY(transform.position.y, 0.75f);
            int costumeMats = Random.Range(0, costumeMaterials.Count);
            List<Material> bodyMaterials = new List<Material>();
            List<Material> legsMaterials = new List<Material>();
            int bodyMaterial = Random.Range(0, enemyMaterials.Count);
            bodyMaterials.Add(enemyMaterials[bodyMaterial]);
            bodyMaterials.Add(costumeMaterials[costumeMats]);
            bodyMaterials.Add(extraMaterials[costumeMats]);
            legsMaterials.Add(costumeMaterials[costumeMats]);
            legsMaterials.Add(extraMaterials[costumeMats]);
            legsMaterials.Add(costumeMaterials[costumeMats]);
            BaseBody.materials = bodyMaterials.ToArray();
            BaseCostume.materials = bodyMaterials.ToArray();
            BaseLegs.materials = legsMaterials.ToArray();
            var tmp = SpiderLegs.materials;
            tmp[0] = enemyMaterials[bodyMaterial];
            tmp[1] = costumeMaterials[costumeMats];
            SpiderLegs.materials = tmp;
            EyeMask.material = costumeMaterials[costumeMats];
            var tmpWings = Wings.materials.Length;
            List<Material> wingsMats = new List<Material>();
            for (int i = 0; i < Wings.materials.Length; i++)
                wingsMats.Add(enemyMaterials[bodyMaterial]);
            List<Material> tentacleMats = new List<Material>();
            for (int i = 0; i < Tentacles.materials.Length; i++)
                tentacleMats.Add(enemyMaterials[bodyMaterial]);
            Wings.materials = wingsMats.ToArray();
            Tentacles.materials = tentacleMats.ToArray();
            Blades.material = enemyMaterials[bodyMaterial];
            Ghost.material = enemyMaterials[bodyMaterial];
        }
    }

    public bool IsSpiked()
    {
        return ActiveParts.Exists(x => x.partType == MutantParts.Spike);
    }

    public bool IsRanged()
    {
        return ActiveParts.Exists(x => x.partType == MutantParts.Range);
    }

    public int GetHealth()
    {
        int health = 100;
        if (ActiveParts.Exists(x => x.partType == MutantParts.Armor))
            health = 200;
        if (type == ModelType.ENEMY)
            health = Mathf.FloorToInt(health * 0.2f);
        return health;
    }

    public float GetSpeed()
    {
        float speed = 5;
        if (ActiveParts.Exists(x => x.partType == MutantParts.Wings))
            speed = 8;
        if (ActiveParts.Exists(x => x.partType == MutantParts.Ghost))
            speed = 8;
        if (ActiveParts.Exists(x => x.partType == MutantParts.SpiderFoots))
            speed = 8;
        if (type == ModelType.ENEMY)
            speed = Mathf.FloorToInt(speed * 0.75f);
        return speed;
    }

    public float GetDistance()
    {
        float distance = 2.1f;
        if (ActiveParts.Exists(x => x.partType == MutantParts.Tentacle))
            distance = 3f;
        if (ActiveParts.Exists(x => x.partType == MutantParts.Range))
            distance = 6;
        return distance;
    }

    private void Start()
    {
        if (entity != null)
        {
            StartCoroutine(AnimationPlayer());
        }
        if (StartRandomMutation)
        {
            SetSwitchPart(parts[Random.Range(0, parts.Length - 1)].partType, true);
        }
    }

    public void Death()
    {
        var tmp = BaseBody.materials;
        tmp[0].DOColor(deathMaterial.color, 1f);
        BaseCostume.materials = tmp;
        BaseBody.materials = tmp;
        var tmpSpider = SpiderLegs.materials;
        tmpSpider[0].DOColor(deathMaterial.color, 1f);
        SpiderLegs.materials = tmpSpider;
        var tmpWings = Wings.materials;
        foreach (var t in tmpWings)
            t.DOColor(deathMaterial.color, 1f);
        Wings.materials = tmpWings;
        var tmpTentacles = Tentacles.materials;
        foreach (var t in tmpTentacles)
            t.DOColor(deathMaterial.color, 1f);
        Tentacles.materials = tmpTentacles;
        Ghost.material.DOColor(deathMaterial.color, 1f);
    }

    public void Play(State state)
    {
        animator.Play(animDictionary[state]);
    }

    public void SetSwitchPart(MutantParts partType, bool active)
    {
        foreach (var item in parts)
        {
            if(item.partType == partType)
            {
                if(item.PartReferense.active != active)
                {
                    if (partType == MutantParts.Ghost || partType == MutantParts.SpiderFoots)
                    {
                        BaseLegs.gameObject.SetActive(!active);
                    }
                    if (partType == MutantParts.Armor)
                    {
                        BaseBody.gameObject.SetActive(!active);
                    }

                    item.PartReferense.SetActive(active);
                    SkinnedMeshRenderer skin = item.PartReferense.GetComponent<SkinnedMeshRenderer>();
                    Mesh skinnedMesh = skin.sharedMesh;
                    if (active)
                    {
                        if (skinnedMesh.blendShapeCount > 0)
                        {
                            float Shape = skin.GetBlendShapeWeight(0);
                            skin.SetBlendShapeWeight(0, 0);
                            StartCoroutine(ActivatePart(skin));
                        }

                        ActiveParts.Add(item);
                        if(item.AnimationName != "")
                        {
                            animDictionary[item.StateAnimation] = item.AnimationName;
                            return;
                        }
                    }
                    else
                    {
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
            yield return new WaitForSecondsRealtime(0.005f);
        }
    }

    IEnumerator AnimationPlayer()
    {
        while (true)
        {
            EntityStateMonitor = entity.state.ToString();
            if (entity.state != State.Mutate || SceneManager.GetActiveScene().name == "Laboratory")
            {
                Play(entity.state);
            }
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

