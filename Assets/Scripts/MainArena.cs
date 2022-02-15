using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MainArena : MonoBehaviour
{
    [Header("MainArena")]
    [Space(10)]
    [SerializeField] CanvasGroup blackoutCanvas;
    [SerializeField] ArenaUI arenaUI;
    [SerializeField] Camera mainCamera;
    [SerializeField] string NextSceneName;
    [Header("data")]
    [SerializeField] GameData gameData;
    [Header("level objects")]
    [SerializeField] GameObject PlayerRoot;
    [SerializeField] GameObject ArenaTank;
    [Space(10)]
    [SerializeField] float RoundTime = 60;
    [SerializeField] ProgressBar progressBar;
    [Space(10)]
    [SerializeField] CanvasGroup WinCanvasGroup;
    [SerializeField] GameObject WinPanel;
    [SerializeField] GameObject DefeatPanel;

    GameObject playerModel;
    ModelManager playerMutantPartActivator;

    void Start()
    {
        InitScene();
    }

    void InitScene()
    {
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
                PlayerRoot.GetComponent<Player>().ActiveCharaterMovement(true);
                ArenaTank.transform.DOScale(new Vector3(0, 0, 0), 0.25f);
                StartCoroutine(ArenaTimer());
            });
        });
    }

    IEnumerator ArenaTimer()
    {
        while(RoundTime > 0)
        {
            RoundTime -= Time.deltaTime;
            progressBar.UpdateProdressBar(((int)RoundTime));
            if (RoundTime <= 0)
            {
                Win();
            }
            yield return null;
        }
    }

    public void Win()
    {
        WinCanvasGroup.DOFade(1, 1f);
        WinPanel.SetActive(true);
    }

    public void Defeat()
    {
        WinCanvasGroup.DOFade(1, 1f);
        DefeatPanel.SetActive(true);
    }

    public void LoadNextScene()
    {
        blackoutCanvas.DOFade(1, 1f).OnComplete(() => 
        {
            SceneManager.LoadScene(NextSceneName);
        });
       
    }
}
