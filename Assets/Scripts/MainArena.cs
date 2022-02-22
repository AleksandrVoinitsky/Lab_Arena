using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;

public class MainArena : Singleton<MainArena>
{
    [Header("MainArena")]
    public bool victory;
    [Space(10)]
    [SerializeField] CanvasGroup blackoutCanvas;
    [SerializeField] string NextSceneName;
    [Header("data")]
    [SerializeField] GameData gameData;
    public int maxEnemiesCount;
    [Header("level objects")]
    [SerializeField] GameObject PlayerRoot;
    [SerializeField] GameObject ArenaTank;
    [SerializeField] private MeshRenderer arenaFloor, arenaWalls, water;
    [SerializeField] private List<Material> arenaMain, arenaMainTwo, arenaSecondary, waterMaterials;
    [Space(10)]
    [SerializeField] TMP_Text gemShopCounter, levelText;
    [SerializeField] CanvasGroup mainCanvas, shopCanvas, WinCanvasGroup;
    [SerializeField] GameObject WinPanel;
    [SerializeField] GameObject DefeatPanel;
    [SerializeField] private GameObject confetti;
    [SerializeField] ShopUi shopUi;

    GameObject playerModel;
    ModelManager playerMutantPartActivator;

    void Start()
    {
        InitScene();
    }

    public int GetLevel()
    {
        return gameData.levelNumber + 1;
    }

    public int GetGems()
    {
        return gameData.gems;
    }

    public void AddGems(int _amount)
    {
        gameData.AddGems(_amount, gemShopCounter);
    }

    public void SpendGems (int _amount)
    {
        gameData.gems -= _amount;
        gemShopCounter.text = gameData.gems.ToString();
    }

    void InitScene()
    {
        int randMat = Random.Range(0, arenaMain.Count);
        var tmp = new List<Material>();
        tmp.Add(arenaMain[randMat]);
        tmp.Add(arenaMainTwo[randMat]);
        arenaFloor.materials = tmp.ToArray();
        arenaWalls.material = arenaSecondary[randMat];
        water.material = waterMaterials[randMat];
        levelText.text = string.Format("Level {0}", (gameData.levelNumber+1));
        playerModel = Instantiate(gameData.PlayerModel, PlayerRoot.transform.position, PlayerRoot.transform.rotation, PlayerRoot.transform);
        playerMutantPartActivator = playerModel.GetComponent<ModelManager>();
        foreach (var item in gameData.PlayerPartsSet)
        {
            playerMutantPartActivator.SetSwitchPart(item, true);
        }
        blackoutCanvas.alpha = 1;
        blackoutCanvas.DOFade(0, 1f).OnComplete(() => 
        {
            ArenaTank.transform.DOMove(new Vector3(ArenaTank.transform.position.x, ArenaTank.transform.position.y + 5, ArenaTank.transform.position.z), 1).OnComplete(() =>
            {
                PlayerRoot.transform.parent = null;
                PlayerRoot.GetComponent<Player>().SetModel(playerMutantPartActivator);
                PlayerRoot.GetComponent<Player>().ActiveCharacterMovement(true);
                ArenaTank.transform.DOScale(new Vector3(0, 0, 0), 0.25f);
                maxEnemiesCount = gameData.levelNumber * 5 + 10;
                CameraController.Instance.FocusOnPlayer();
                PlayerRoot.GetComponent<Player>().InitKillCounter(maxEnemiesCount);
                UpgradeHandler.Instance.CheckUpgrades(PlayerRoot.GetComponent<Player>().GetGems());
                mainCanvas.gameObject.SetActive(true);
                mainCanvas.DOFade(1, 1f);
            });
        });
    }

    public void Win()
    {
        confetti.SetActive(true);
        victory = true;
        foreach (var e in FindObjectsOfType<Enemy>())
        {
            e.Damage(e.health, null);
        }
        CameraController.Instance.Victory();
        mainCanvas.DOFade(0, 1f).OnComplete(() => mainCanvas.gameObject.SetActive(false));
        Invoke("WinUI", 2.5f);
    }

    private void WinUI()
    {
        gameData.AddLevel();
        WinCanvasGroup.gameObject.SetActive(true);
        WinCanvasGroup.DOFade(1, 1f).SetUpdate(true).OnComplete(() => Time.timeScale = 0);
        WinPanel.SetActive(true);
    }

    public void Defeat()
    {
        victory = true;
        Time.timeScale = 0.5f;
        Invoke("DefeatUI", 2f);
    }

    private void DefeatUI()
    {
        WinCanvasGroup.gameObject.SetActive(true);
        WinCanvasGroup.DOFade(1, 1f).SetUpdate(true).OnComplete(() => Time.timeScale = 0);
        DefeatPanel.SetActive(true);
    }

    public void LoadShop()
    {
        shopCanvas.gameObject.SetActive(true);
        shopUi.InitShopUi();
        shopCanvas.DOFade(1, 0.5f).SetUpdate(true).OnComplete (() =>
        {
            WinCanvasGroup.gameObject.SetActive(false);
        });
    }

    public void LoadNextScene()
    {
        blackoutCanvas.gameObject.SetActive(true);
        blackoutCanvas.DOFade(1, 1f).SetUpdate(true).OnComplete(() =>
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(NextSceneName);
        });
    }
}
