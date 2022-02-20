using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Shop : MonoBehaviour
{
    [SerializeField] GameData gamedata;
    [SerializeField] TMP_Text shopGemText;
    private Dictionary<ShopItemsType, int> ShopItemsDictionary; 

    private void Awake()
    {
        //InitShop();
    }
   
    public void InitShop()
    {
        ShopItemsDictionary = new Dictionary<ShopItemsType, int>();
        int index = 0;
        foreach (var item in gamedata.ShopItems)
        {
            try
            {
                ShopItemsDictionary.Add(item.itemType, index);
                index++;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }

    public bool IsBought (ShopItemsType type)
    {
        return GetShopItem(type).itemState == ShopItemState.bought;
    }

    public bool EnoughGems(ShopItemsType type)
    {
        return gamedata.gems >= GetShopItem(type).Price;
    }

    public bool Buy(ShopItemsType type)
    {
        ShopItem item = GetShopItem(type);
        if (gamedata.gems >= item.Price)
        {
            if(item.itemState == ShopItemState.sell)
            {
                gamedata.RemoveGems(item.Price, shopGemText);
                OpenItem(type);
                return true;
            }
        }
        return false;
    }

    public void OpenItem(ShopItemsType type)
    {
        ShopItem item = GetShopItem(type);
        item.itemState = ShopItemState.bought;
        SetShopItem(item);
    }

    public ShopItemState CheckShopItemState(ShopItemsType type)
    {
        ShopItem item = GetShopItem(type);
        return item.itemState;
    }

    public ShopItem GetShopItem(ShopItemsType type)
    {
        return gamedata.ShopItems[ShopItemsDictionary[type]];
    }

    public ShopItemsType[] GetShopItemsType(ShopItemState requestItemState)
    {
        List<ShopItemsType> items = new List<ShopItemsType>();
        foreach (var item in gamedata.ShopItems)
        {
            if(item.itemState == requestItemState)
            {
                items.Add(item.itemType);
            }
        }
        return items.ToArray();
    }

    public ShopItemsType[] GetRandomSellShopItemsTypes(int count)
    {
        ShopItemsType[] types = GetShopItemsType(ShopItemState.sell);

        if(types.Length - 1 < count)
        {
            return types;
        }
        else
        {
            List<ShopItemsType> ShopItemsTypes = new List<ShopItemsType>();
            ShopItemsType[] resultTypes = new ShopItemsType[count];

            foreach (var item in types)
            {
                ShopItemsTypes.Add(item);
            }

            for (int i = 0; i < count; i++)
            {

                resultTypes[i] = ShopItemsTypes[Random.Range(0, ShopItemsTypes.Count)];
                ShopItemsTypes.Remove(resultTypes[i]);
            }

            return resultTypes;
        }
    }

    #region private
    private void SetShopItem(ShopItem newShopItem)
    {
        gamedata.ShopItems[ShopItemsDictionary[newShopItem.itemType]] = newShopItem;
    }
    #endregion

}
