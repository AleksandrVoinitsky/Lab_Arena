using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] GameData gamedata;
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
                ShopItemsDictionary.Add(item.ItemType, index);
                index++;
            }
            catch (System.Exception)
            {
                Debug.LogError("Невозможно добавить одинаковые типы ItemShop");
                throw;
            }
        }
    }

    /// <summary>
    /// Операция покупки в магазине c проверкой баланса
    /// </summary>
    /// <param name="type">ShopItemsType</param>
    /// <returns>Результат выполнения операции Bool</returns>
    public bool Buy(ShopItemsType type)
    {
        ShopItem item = GetShopItem(type);
        if (gamedata.ShopCoins >= item.Price)
        {
            if(item.itemState == ShopItemState.sell)
            {
                gamedata.ShopCoins -= item.Price;
                OpenItem(type);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Открытие Item по его типу
    /// </summary>
    /// <param name="type">ShopItemsType</param>
    public void OpenItem(ShopItemsType type)
    {
        ShopItem item = GetShopItem(type);
        item.itemState = ShopItemState.bought;
        SetShopItem(item);
    }

    /// <summary>
    /// Вернет ShopItemState структуры ShopItem по ее ShopItemsType
    /// </summary>
    /// <param name="type">ShopItemsType</param>
    /// <returns></returns>
    public ShopItemState CheckShopItemState(ShopItemsType type)
    {
        ShopItem item = GetShopItem(type);
        return item.itemState;
    }

    /// <summary>
    /// Получить ShopItem по его типу
    /// </summary>
    /// <param name="type">ShopItemsType</param>
    /// <returns>Struct ShopItem</returns>
    public ShopItem GetShopItem(ShopItemsType type)
    {
        return gamedata.ShopItems[ShopItemsDictionary[type]];
    }

    /// <summary>
    /// Возвращает массив ShopItemsType соответствующих состоянию ShopItemState
    /// </summary>
    /// <param name="requestItemState">ShopItemState структуры ShopItem</param>
    /// <returns></returns>
    public ShopItemsType[] GetShopItemsType(ShopItemState requestItemState)
    {
        List<ShopItemsType> items = new List<ShopItemsType>();
        foreach (var item in gamedata.ShopItems)
        {
            if(item.itemState == requestItemState)
            {
                items.Add(item.ItemType);
            }
        }
        return items.ToArray();
    }

    /// <summary>
    /// Возвращает count случайных ShopItemsType
    /// </summary>
    /// <returns>ShopItemsType[]</returns>
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
    /// <summary>
    /// Заменить ShopItem на новый
    /// </summary>
    /// <param name="newShopItem">ShopItemsType</param>
    private void SetShopItem(ShopItem newShopItem)
    {
        gamedata.ShopItems[ShopItemsDictionary[newShopItem.ItemType]] = newShopItem;
    }
    #endregion

}
