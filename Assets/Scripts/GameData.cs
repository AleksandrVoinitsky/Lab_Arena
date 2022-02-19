using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName ="GAME_DATA",menuName = "Data/ Game data object")]
public class GameData : ScriptableObject
{

    [Header("__GENERAL__")]
    public int LevelNumber;
    public int ShopCoins;

    [Header("__PLAYER__")]
    public GameObject PlayerModel;
    public List<MutantParts> PlayerPartsSet = new List<MutantParts>();

    [Header("__SHOP__")]
    public ShopItem[] ShopItems;

    [Header("__MUTAGENS__")]
    //public GameObject[] Mutagens;
    public ItemsGroup[] MutagenGroups;

    public void AddLevel()
    {
        LevelNumber += 1;
    }
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

public enum ShopItemsType
{
    Blabes,
    Range,
    Tentacle,
    Wings,
    SpiderFoots,
    Ghost,
    Armor,
    Spike,
    None
}

public enum ShopItemState
{
    sell,
    bought,
    notSell
}

[System.Serializable]
public struct ShopItem
{
    [Header("[ITEM]_______________________________________________________________________________________________________")]
    public string Name;
    public ShopItemsType ItemType;
    [TextArea] public string Description;
    [Space(10)]
    public int Price;
    public ShopItemState itemState;
    public GameObject Prefab;
    
}

[System.Serializable]
public struct ItemsGroup
{
    public string GroupName;
    public ShopItemsType Item_1;
    public ShopItemsType Item_2;
    public ShopItemsType Item_3;
}



