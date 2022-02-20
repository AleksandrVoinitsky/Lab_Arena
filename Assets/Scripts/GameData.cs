using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public void AddGems (int _amount = 1, TMP_Text gemText = null)
    {
        gems += _amount;
        gemText.text = gems.ToString();
    }

    public void RemoveGems (int _amount = 1, TMP_Text gemText = null)
    {
        gems -= _amount;
        gemText.text = gems.ToString();
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

public enum MutantGroup
{
    Attack,
    Speed,
    Defence
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
    public List<ShopItemsType> items;
}



