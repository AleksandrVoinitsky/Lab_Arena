using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;

public class MainLaboratory : MonoBehaviour
{
    [Header("MainLaboratory")]
    [Space(10)]
    [SerializeField] CanvasGroup blackoutCanvas;
    [SerializeField] LaboratoryUi laboratoryUi;
    [SerializeField] Camera MainCamera;
    [SerializeField] string NextSceneName;
    [Header("data")]
    [SerializeField] GameData gameData;
    [Header("level referenses")]
    [SerializeField] GameObject LboratoryTank;
    [SerializeField] Material LboratoryTankGlassMaterial;
    [SerializeField] Color LboratoryTankGlassMaterialColor;
    [SerializeField] GameObject playerModel;
    [Space(10)]
    [SerializeField] GameObject StartButtonRootObject;
    [SerializeField] GameObject StartButton;
    [Space(10)]
    [SerializeField] Transform PlayerSpawnPoint;
    [SerializeField] Transform FlaskSetSpawnPoint;
    [Header("Ui")]
    [SerializeField] GameObject[] FlaskUiGroup1;
    [SerializeField] GameObject[] FlaskUiGroup2;
    [SerializeField] GameObject[] FlaskUiGroup3;
    [SerializeField] TextMeshProUGUI tmpTextInfo;
    [SerializeField] CanvasGroup tmpTextInfoCanvasGroup;



    int mutagenCount = 0;
    Queue<string> TextInfoQueue = new Queue<string>();
    bool tmpTextInfoFlag;
    // Start is called before the first frame update
    void Start()
    {
        InitScene();
    }

    void InitScene()
    {
        ObjectArrayActivator(FlaskUiGroup1, false);
        ObjectArrayActivator(FlaskUiGroup2, false);
        ObjectArrayActivator(FlaskUiGroup3, false);
        gameData.PlayerPartsSet.Clear();
        blackoutCanvas.alpha = 1;
        blackoutCanvas.DOFade(0, 1f);
        StartButtonRootObject.transform.localScale = new Vector3(0, 0, 0);
        LboratoryTankGlassMaterial.color = LboratoryTankGlassMaterialColor;
        CreatePlayerCharacter();
        CreateFlaskSet();
        tmpTextInfoCanvasGroup.alpha = 0;
        tmpTextInfoFlag = true;
    }

    public void ShowStartButton()
    {
        StartButtonRootObject.transform.DOScale(new Vector3(1, 1, 1), 0.5f);
    }

    public void HideStartButton()
    {
        StartButtonRootObject.transform.DOScale(new Vector3(0, 0, 0), 0.5f);
    }

    public void UpLboratoryTank()
    {
        MainCamera.transform.DORotate(new Vector3(-30f, 0, 0), 2);
        MainCamera.transform.DOShakePosition(0.3f, 0.3f);
        LboratoryTank.transform.DOMoveY(LboratoryTank.transform.position.y + 6f, 1f);
        blackoutCanvas.DOFade(1, 2f).OnComplete(() => { LoadNextScene(); });
    }

    public void AddMutagen(Color color, MutantParts part, GameObject Image, string mutagenName = "")
    {
        LboratoryTank.transform.DOShakePosition(0.3f, 0.3f);
        LboratoryTankGlassMaterial.DOColor(color, 1);
        playerModel.GetComponent<ModelManager>().SetSwitchPart(part, true);
        mutagenCount++;
        gameData.PlayerPartsSet.Add(part);
        switch (mutagenCount)
        {
            case 1:
                ObjectArrayActivator(FlaskUiGroup1, true);
                Instantiate(Image, FlaskUiGroup1[0].transform.position, FlaskUiGroup1[0].transform.rotation, FlaskUiGroup1[0].transform);
                break;
            case 2:
                ObjectArrayActivator(FlaskUiGroup2, true);
                Instantiate(Image, FlaskUiGroup2[0].transform.position, FlaskUiGroup2[0].transform.rotation, FlaskUiGroup2[0].transform);
                break;
            case 3:
                ObjectArrayActivator(FlaskUiGroup3, true);
                Instantiate(Image, FlaskUiGroup3[0].transform.position, FlaskUiGroup3[0].transform.rotation, FlaskUiGroup3[0].transform);
                break;
        }

        if(mutagenName == "")
        {
            StartCoroutine(CheckTextInfo(part.ToString()));
        }
        else
        {
            StartCoroutine(CheckTextInfo(mutagenName));
        }
    }

    IEnumerator CheckTextInfo(string text)
    {
        TextInfoQueue.Enqueue(text);

        while (TextInfoQueue.Count > 0)
        {
            if (tmpTextInfoFlag)
            {
                ShowTextInfo(TextInfoQueue.Dequeue());
            }
            yield return null;
        }
    }

    void ShowTextInfo(string text)
    {

        tmpTextInfoFlag = false;
        tmpTextInfo.text = text;
        tmpTextInfoCanvasGroup.DOFade(1, 1f).OnComplete(() => 
        {
            tmpTextInfoCanvasGroup.DOFade(0, 0.5f).OnComplete(() =>
            {
                tmpTextInfoFlag = true;
            });
        });
    }


    void ObjectArrayActivator(GameObject[] arr, bool active)
    {

        foreach (var item in arr)
        {
            item.SetActive(active);
        }
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(NextSceneName);
    }

    public void CreatePlayerCharacter()
    {
       //player = Instantiate(gameData.PlayerModel, PlayerSpawnPoint.position, PlayerSpawnPoint.rotation,LboratoryTank.transform);
    }

    public void CreateFlaskSet()
    {
        Instantiate(gameData.Mutagens[0], FlaskSetSpawnPoint.position, FlaskSetSpawnPoint.rotation);
    }

    public Transform GetTankObjectTransform()
    {
        return LboratoryTank.transform;
    }
}
