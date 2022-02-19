using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [Header("level referenсes")]
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
    [SerializeField] TMP_Text mutantLabel;
    [SerializeField] string mutantText;
    [SerializeField] TextMeshProUGUI tmpTextInfo;
    [SerializeField] CanvasGroup tmpTextInfoCanvasGroup, mutagenCanvas;
    [Header("VFX")]
    [SerializeField] List<Animator> teslaVFX;
    [SerializeField] GameObject diffusion, electricity;

    public bool isMoving;



    int mutagenCount = 0;
    Queue<string> TextInfoQueue = new Queue<string>();
    bool tmpTextInfoFlag;

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
       // CreateFlaskSet();
        tmpTextInfoCanvasGroup.alpha = 0;
        tmpTextInfoFlag = true;
    }

    public void ShowStartButton()
    {
        mutantLabel.text = mutantText;
        mutagenCanvas.gameObject.SetActive(true);
        mutagenCanvas.DOFade(1, 0.5f);
        StartButtonRootObject.transform.DOScale(1, 0.5f);
    }

    public void HideStartButton()
    {
        StartButtonRootObject.transform.DOScale(0, 0.5f);
    }

    public void UpLboratoryTank()
    {
        MainCamera.transform.DORotate(new Vector3(-30f, 0, 0), 2);
        MainCamera.transform.DOShakePosition(0.3f, 0.3f);
        LboratoryTank.transform.DOMoveY(LboratoryTank.transform.position.y + 6f, 1f);
        blackoutCanvas.gameObject.SetActive(true);
        blackoutCanvas.DOFade(1, 2f).OnComplete(() => { LoadNextScene(); });
    }

    public void ShootTeslaGuns()
    {
        foreach (var t in teslaVFX)
        {
            t.Play("CoilShoot", -1, 0f);
            var e = Instantiate(electricity, t.transform.position, electricity.transform.rotation);
            e.transform.DOMove(LboratoryTank.transform.position + Vector3.up, 0.2f);
        }
    }

    public void ShakePlayer()
    {
        playerModel.GetComponent<ModelManager>().Play(State.Mutate);
    }

    public void SpawnDiffusion (Color color)
    {
        var tmp = Instantiate(diffusion, LboratoryTank.transform.position + Vector3.up, diffusion.transform.rotation);
        ParticleSystem.MainModule main = tmp.GetComponent<ParticleSystem>().main;
        main.startColor = color;
    }

    public void AddMutagen(Color color, MutantParts part, GameObject mutationImage, string mutagenName = "")
    {
        isMoving = false;
        //LboratoryTank.transform.DOShakePosition(0.3f, 0.3f);
        LboratoryTankGlassMaterial.DOColor(color, 3);
        playerModel.GetComponent<ModelManager>().SetSwitchPart(part, true);
        mutagenCount++;
        gameData.PlayerPartsSet.Add(part);
        GameObject uiMutation;
        switch (mutagenCount)
        {
            default:
                ObjectArrayActivator(FlaskUiGroup1, true);
                uiMutation = FlaskUiGroup1[0];
                Instantiate(mutationImage, FlaskUiGroup1[0].transform.position, FlaskUiGroup1[0].transform.rotation, FlaskUiGroup1[0].transform);
                break;
            case 2:
                ObjectArrayActivator(FlaskUiGroup2, true);
                uiMutation = FlaskUiGroup2[1];
                Instantiate(mutationImage, FlaskUiGroup2[1].transform.position, FlaskUiGroup2[1].transform.rotation, FlaskUiGroup2[1].transform);
                break;
            case 3:
                ObjectArrayActivator(FlaskUiGroup3, true);
                uiMutation = FlaskUiGroup3[1];
                Instantiate(mutationImage, FlaskUiGroup3[1].transform.position, FlaskUiGroup3[1].transform.rotation, FlaskUiGroup3[1].transform);
                break;
        }

        uiMutation.GetComponent<Image>().color = new Color (color.r, color.g, color.b, 0.7f);

        if (mutagenName == string.Empty)
        {
            StartCoroutine(CheckTextInfo(part.ToString()));
        }
        else
        {
            StartCoroutine(CheckTextInfo(mutagenName));
        }
    }

    public void AddMutagenText(string _text)
    {
        mutantText += _text + " ";
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
        //Instantiate(gameData.Mutagens[gameData.LevelNumber], FlaskSetSpawnPoint.position, FlaskSetSpawnPoint.rotation);
    }

    public Transform GetTankObjectTransform()
    {
        return LboratoryTank.transform;
    }
}
