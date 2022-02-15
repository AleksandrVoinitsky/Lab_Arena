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

    void Awake()
    {
        mutagenColor = transform.Find("liquid").GetComponent<MeshRenderer>().material.color;
        mutagenColor = new Color(mutagenColor.r, mutagenColor.g, mutagenColor.b, 0.25f);
    }


    private void OnMouseDown()
    {
        MainLaboratory main = FindObjectOfType<MainLaboratory>();
        Transform tank = main.GetTankObjectTransform();
        transform.parent = null;
        transform.DORotate(Vector3.zero, 0.5f);
        transform.DOMoveY(transform.position.y + 0.5f, 0.5f).OnComplete(() =>
        {
            transform.DOMove(tank.position + Vector3.up * 3.5f, 0.5f).OnComplete(() =>
            {
                StartCoroutine(Movement(tank, main));
            });
        });
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
        yield return new WaitForSecondsRealtime(0.25f);
        transform.DOMoveY(_tank.position.y + 1, 0.75f).OnComplete(() =>
        {
            AddMutagen(_main);
        });
    }

    private void AddMutagen (MainLaboratory _main)
    {
        if (FluidParticle != null)
            Instantiate(FluidParticle, FluidSpawnPoint.position, FluidSpawnPoint.rotation);
        transform.DOScale(new Vector3(0, 0, 0), 0.25f).OnComplete(() =>
        {
            _main.AddMutagen(mutagenColor, part, Image);
            setParent.NextSet();
            OnUse.Invoke();
        });
    }
}
