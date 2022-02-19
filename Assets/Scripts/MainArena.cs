using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;

public class MainArena : Singleton<MainArena>
{
    [Header("MainArena")]
    [Space(10)]
    [SerializeField] CanvasGroup blackoutCanvas;
    [SerializeField] string NextSceneName;
    [Header("data")]
    [SerializeField] GameData gameData;
    [Header("level objects")]
    [SerializeField] GameObject PlayerRoot;
    [SerializeField] GameObject ArenaTank;
    [SerializeField] private MeshRenderer arenaFloor, arenaWalls, water;
    [SerializeField] private List<Material> arenaMain, arenaSecondary, waterMaterials;
    [Space(10)]
    [SerializeField] float RoundTime = 60;
    [SerializeField] TMP_Text timerText;
    [Space(10)]
    [SerializeField] CanvasGroup mainCanvas, WinCanvasGroup;
    [SerializeField] GameObject WinPanel;
    [SerializeField] GameObject DefeatPanel;
    [SerializeField] ShopUi shopUi;

    GameObject playerModel;
    ModelManager playerMutantPartActivator;

    void Start()
    {
        InitScene();
    }

    void InitScene()
    {
        int randMat = Random.Range(0, arenaMain.Count);
        var tmp = new List<Material>();
        tmp.Add(arenaMain[randMat]);
        tmp.Add(arenaSecondary[randMat]);
        arenaFloor.materials = tmp.ToArray();
        arenaWalls.material = arenaSecondary[randMat];
        water.material = waterMaterials[Random.Range (0, waterMaterials.Count)];
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
                StartCoroutine(ArenaTimer());
                CameraController.Instance.FocusOnPlayer();
                mainCanvas.gameObject.SetActive(true);
                mainCanvas.DOFade(1, 1f);
            });
        });
    }

    IEnumerator ArenaTimer()
    {
        while (RoundTime > 0)
        {
            RoundTime -= Time.deltaTime;
            if (RoundTime > 0)
                timerText.text = string.Format("{0}:{1:00}", Mathf.FloorToInt(RoundTime/60), Mathf.FloorToInt(RoundTime%60));
            else
            {
                timerText.text = "0:00";
                Win();
            }
            yield return null;
        }
    }

    public void Win()
    {
        gameData.AddLevel();
        WinCanvasGroup.gameObject.SetActive(true);
        WinCanvasGroup.DOFade(1, 1f).SetUpdate (true).OnComplete (() => Time.timeScale = 0);
        WinPanel.SetActive(true);
        shopUi.gameObject.SetActive(true);
        shopUi.InitShopUi();
    }

    public void Defeat()
    {
        WinCanvasGroup.gameObject.SetActive(true);
        WinCanvasGroup.DOFade(1, 1f).SetUpdate (true).OnComplete (() => Time.timeScale = 0);
        DefeatPanel.SetActive(true);
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
