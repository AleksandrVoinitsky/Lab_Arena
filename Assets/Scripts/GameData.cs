using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="GAME_DATA",menuName = "Data/ Game data object")]
public class GameData : ScriptableObject
{
    [Header("GENERAL")]
    public int LevelNumber;


    [Header("PLAYER")]
    public GameObject PlayerModel;
    public List<MutantParts> PlayerPartsSet = new List<MutantParts>();

    [Header("MUTAGENS")]
    public GameObject[] Mutagens;



    [Header("ENEMIES")]
    public GameObject[] Enemies;
}

public enum MutantParts
{
    Blabes,
    Range,
    Tentacle,
    Wings,
    SpiderFoots,
    Ghost,
    Armor,
    Spike
}
