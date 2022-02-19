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
        for (int i = 0; i < ShopBlocks.Length; i++)
        {
            if(i < types.Length - 1)
            {
                ShopItem tempItem = shop.GetShopItem(types[i]);
                ShopBlocks[i].itemName.text = tempItem.name;
                ShopBlocks[i].itemImage.sprite = tempItem.picture;
                ShopBlocks[i].itemDescription.text = tempItem.description;
                ShopBlocks[i].priceText.text = tempItem.Price.ToString();
                ShopBlocks[i].type = tempItem.itemType;
            }
            else
            {
                ShopItem tempItem = new ShopItem();
                ShopBlocks[i].itemName.text = "Bought";
                ShopBlocks[i].itemDescription.text = "Bought";
                ShopBlocks[i].priceText.text = "Bought";
                ShopBlocks[i].type = ShopItemsType.None;
            }
            
        }
    }

    public void TryToBuy(int index)
    {
        if (ShopBlocks[index].type != ShopItemsType.None)
        {
            if (shop.Buy(ShopBlocks[index].type))
            {
                ShopBlocks[index].priceText.text = "Bought!";
            }
                
        }
       
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
