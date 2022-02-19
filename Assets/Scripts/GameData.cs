using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName ="GAME_DATA",menuName = "Data/ Game data object")]
public class GameData : ScriptableObject
{

    [Header("General")]
    public int levelNumber;
    public int gems;

    [Header("Player")]
    public GameObject PlayerModel;
    public List<MutantParts> PlayerPartsSet = new List<MutantParts>();

    [Header("Shop")]
    public ShopItem[] ShopItems;

    [Header("Mutagens")]
    //public GameObject[] Mutagens;
    public ItemsGroup[] MutagenGroups;

    public void AddLevel(int _amount = 1)
    {
        levelNumber += _amount;
    }

    public void AddGems (int _amount = 1)
    {
        gems += _amount;
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
    [Header("[ITEM]")]
    public string name;
    public Sprite picture;
    public ShopItemsType itemType;
    [TextArea] public string description;
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



