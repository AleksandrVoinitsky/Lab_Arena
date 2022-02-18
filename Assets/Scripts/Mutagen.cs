using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;


public class Mutagen : MonoBehaviour
{
    [SerializeField] Color mutagenColor;
    [SerializeField] MutantParts part;
    [SerializeField] MutagenSetParent setParent;
    [SerializeField] UnityEvent OnUse;
    [SerializeField] GameObject Image;
    [SerializeField] GameData FluidParticle;
    [SerializeField] Transform FluidSpawnPoint;
    [SerializeField] GameObject shatteredFlask;
    [SerializeField] string MutagenName, MutantName;
    [SerializeField] GameObject pour;

    void Awake()
    {
        mutagenColor = transform.Find("liquid").GetComponent<MeshRenderer>().material.color;
        mutagenColor = new Color(mutagenColor.r, mutagenColor.g, mutagenColor.b, 0.25f);
    }


    private void OnMouseDown()
    {
        setParent.HideHand();
        MainLaboratory main = FindObjectOfType<MainLaboratory>();
        if (!main.isMoving)
        {
            main.isMoving = true;
            Transform tank = main.GetTankObjectTransform();
            Destroy(transform.Find("cap").gameObject);
            transform.parent = null;
            transform.DORotate(new Vector3(0, transform.rotation.y, transform.rotation.z), 0.4f);
            transform.DOMoveY(transform.position.y + 0.5f, 0.4f).OnComplete(() =>
            {
                transform.DOMove(tank.position + Vector3.left * 0.5f + Vector3.up * 3f, 0.4f).OnComplete(() =>
                {
                    StartCoroutine(Movement(tank, main));
                });
            });
        }
        /*transform.DOMove(new Vector3(0, transform.position.y + 2f, transform.position.z), 0.25f).OnComplete(() => 
        {
            transform.DOMove(new Vector3(transform.position.x, transform.position.y, transform.position.z), 0.25f).OnComplete(() =>
            {
                transform.DOMove(new Vector3(tank.position.x - 0.5f, tank.position.y + 3, tank.position.z), 0.5f).OnComplete(() =>
                {
                    transform.DORotate(new Vector3(90f, 90f, 0f), 0.5f);
                    transform.DOMove(new Vector3(transform.position.x, transform.position.y, transform.position.z), 1f).OnComplete(() =>
                    {
                        Instantiate(FluidParticle, FluidSpawnPoint.position, FluidSpawnPoint.rotation);
                        transform.DOScale(new Vector3(0,0,0), 0.25f);
                        main.AddMutagen(mutagenColor, part,Image);
                    });
                });

            });
        });*/
    }

    private IEnumerator Movement(Transform _tank, MainLaboratory _main)
    {
        yield return new WaitForSecondsRealtime(0.2f);
        var p = Instantiate(pour, transform.position + Vector3.right * 0.25f, pour.transform.rotation);
        ParticleSystem.MainModule main = p.GetComponent<ParticleSystem>().main;
        main.startColor = mutagenColor;
        p.GetComponent<ParticleSystemRenderer>().material.color = mutagenColor;
        p.GetComponent<ParticleSystemRenderer>().trailMaterial.color = mutagenColor;
        transform.DORotate(new Vector3 (0, 0, -75), 0.25f).OnComplete(() =>
        {
            StartCoroutine(AddMutagen(_main));
        });
    }

    private IEnumerator AddMutagen (MainLaboratory _main)
    {
        yield return new WaitForSecondsRealtime(0.2f);
        _main.SpawnDiffusion(mutagenColor);
        yield return new WaitForSecondsRealtime(0.75f);
        _main.ShootTeslaGuns();
        if (FluidParticle != null)
            Instantiate(FluidParticle, FluidSpawnPoint.position, FluidSpawnPoint.rotation);
        transform.DOScale(new Vector3(0, 0, 0), 0.25f).OnComplete(() => Instantiate(shatteredFlask, transform.position, Quaternion.identity));
        yield return new WaitForSecondsRealtime(0.5f);
        _main.AddMutagen(mutagenColor, part, Image, MutagenName);
        setParent.NextSet();
        OnUse.Invoke();
        _main.AddMutagenText(MutantName);
        Destroy(gameObject);
    }
}
