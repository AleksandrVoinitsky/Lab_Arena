using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUi : MonoBehaviour
{
    [SerializeField] Shop shop;
    [SerializeField] ShopUiBlock[] ShopBlocks;

    public void InitShopUi()
    {
        shop.InitShop();
        ShopItemsType[] types = shop.GetRandomSellShopItemsTypes(ShopBlocks.Length);

        for (int i = 0; i < types.Length; i++)
        {
            ShopItem tempItem = shop.GetShopItem(types[i]);

            ShopBlocks[i].ItemName.text = tempItem.Name;
            ShopBlocks[i].ItemDescript.text = tempItem.Description;
            ShopBlocks[i].Price.text = tempItem.Price.ToString();
            ShopBlocks[i].type = tempItem.ItemType;
        }
    }

    public void TryBue(int index)
    {
        shop.Buy(ShopBlocks[index].type);//на случай проверки покупки возвращает bool
        ShopBlocks[index].Price.text = "Bought!";
    }

}

[System.Serializable]
public struct ShopUiBlock
{
    public TMP_Text ItemName;
    public TMP_Text ItemDescript;
    public TMP_Text Price;
    public ShopItemsType type;
    public GameObject ItemBlock;
}
