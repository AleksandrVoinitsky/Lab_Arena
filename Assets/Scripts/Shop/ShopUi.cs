using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
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
            if (i < types.Length)
            {
                ShopItem tempItem = shop.GetShopItem(types[i]);
                ShopBlocks[i].itemName.text = tempItem.name;
                ShopBlocks[i].itemImage.sprite = tempItem.picture;
                ShopBlocks[i].itemDescription.text = tempItem.description;
                ShopBlocks[i].priceText.text = tempItem.Price.ToString();
                ShopBlocks[i].type = tempItem.itemType;
                ShopBlocks[i].CheckAvailability(shop.EnoughGems(ShopBlocks[i].type));
            }
            else
            {
                ShopItem tempItem = new ShopItem();
                ShopBlocks[i].itemName.text = "Bought!";
                ShopBlocks[i].itemDescription.text = "Bought!";
                ShopBlocks[i].priceText.text = "-";
                ShopBlocks[i].tick.gameObject.SetActive(true);
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
                for (int i = 0; i < ShopBlocks.Length; i++)
                {
                    if (!shop.IsBought(ShopBlocks[i].type))
                    {
                        ShopBlocks[i].CheckAvailability(shop.EnoughGems(ShopBlocks[i].type));
                    }
                }
                ShopBlocks[index].button.transform.DOScale(1.2f, 0.25f).SetUpdate(true);
                ShopBlocks[index].button.transform.DORotate(new Vector3(0, 0, -20), 0.25f).SetUpdate(true).OnComplete(() =>
                {
                    ShopBlocks[index].button.transform.DOScale(1f, 0.15f).SetUpdate(true);
                    ShopBlocks[index].button.transform.DORotate(Vector3.zero, 0.15f).SetUpdate(true);
                    ShopBlocks[index].priceText.text = "Bought!";
                    ShopBlocks[index].tick.gameObject.SetActive(true);
                    ShopBlocks[index].tick.DOFade(1, 0.15f).SetUpdate(true);
                });
            }
        }
    }
}

[System.Serializable]
public struct ShopUiBlock
{
    public Image button;
    public Sprite availableSprite, unavailableSprite;
    public TMP_Text itemName;
    public TMP_Text itemDescription;
    public TMP_Text priceText;
    public Image itemImage;
    public ShopItemsType type;
    public GameObject ItemBlock;
    public CanvasGroup tick;

    public void CheckAvailability (bool _available)
    {
        button.sprite = _available ? availableSprite : unavailableSprite;
    }
}
