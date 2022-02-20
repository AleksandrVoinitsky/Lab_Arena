using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UpgradeHandler : Singleton<UpgradeHandler>
{
    [SerializeField] private List<Upgrade> upgrades;
    private Player player;

    public void CheckUpgrades (int _amount)
    {
        foreach (var u in upgrades)
        {
            u.SetActive(_amount);
        }
    }

    public void Buy (int _type)
    {
        if (player == null)
            player = FindObjectOfType<Player>();
        var chosenUpgrade = upgrades[_type];
        if (player.GetGems() >= chosenUpgrade.price)
        {
            player.SpendGems(chosenUpgrade.price);
            chosenUpgrade.level++;
            chosenUpgrade.price *= 2;
            chosenUpgrade.priceText.text = chosenUpgrade.price.ToString();
            chosenUpgrade.button.transform.DOScale(0.9f, 0.2f).OnComplete(() =>
            {
                chosenUpgrade.button.transform.DOScale(0.75f, 0.3f);
            });
            player.Upgrade((UpgradeType)_type);
            CheckUpgrades(player.GetGems());
        }
    }
}

public enum UpgradeType { DAMAGE, HEALTH, SPEED }

[System.Serializable]
public class Upgrade
{
    public UpgradeType type;
    public int level;
    public int price;
    public TMP_Text priceText;
    public Image button;
    [SerializeField] private Sprite active, inActive;

    public void SetActive (int _amount)
    {
        button.sprite = (_amount >= price) ? active : inActive;
    }
}
