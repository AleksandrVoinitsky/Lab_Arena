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
            ShopBlocks[i].itemName.text = tempItem.name;
            ShopBlocks[i].itemImage.sprite = tempItem.picture;
            ShopBlocks[i].itemDescription.text = tempItem.description;
            ShopBlocks[i].priceText.text = tempItem.Price.ToString();
            ShopBlocks[i].type = tempItem.itemType;
        }
    }

    public void TryToBuy(int index)
    {
        if (shop.Buy(ShopBlocks[index].type))
            ShopBlocks[index].priceText.text = "Bought!";
    }

}

[System.Serializable]
public struct ShopUiBlock
{
    public TMP_Text itemName;
    public TMP_Text itemDescription;
    public TMP_Text priceText;
    public Image itemImage;
    public ShopItemsType type;
    public GameObject ItemBlock;
}
