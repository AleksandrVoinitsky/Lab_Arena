using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEves : MonoBehaviour
{
    SkinnedMeshRenderer skinned;
    // Start is called before the first frame update
    void Start()
    {
        skinned = GetComponent<SkinnedMeshRenderer>();
        StartCoroutine(EveClosed());
    }

    IEnumerator EveClosed()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2,4));
            skinned.SetBlendShapeWeight(0, 0);
            yield return new WaitForSeconds(0.2f);
            skinned.SetBlendShapeWeight(0, 100);
            yield return null;
        }
    }
}
