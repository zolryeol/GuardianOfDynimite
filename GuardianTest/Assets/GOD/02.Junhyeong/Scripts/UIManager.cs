using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
            }
            return instance;
        }
    }
    [SerializeField] GameObject PressAnyKey;

    [SerializeField] GameObject TitleMenu;

    [SerializeField] GameObject PauseMenu;

    [SerializeField] GameObject Ranking;

    [SerializeField] GameObject MissionFailed_ui;

    [SerializeField] GameObject DefuseSuccess_ui;

    List<GameObject> lUISet;

    [SerializeField] Text tryCount_ui;

    [SerializeField] GameObject RankParent;
    public Text[] RankTexts;
    public int rankingPageIndex = 0;

    [SerializeField] Transform introCamera;
    [SerializeField] float cameraMovespeed = 1;
    Vector3 cameraOriPos;
    private void Awake()
    {
        cameraOriPos = introCamera.position;


        if (null == instance)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
        {
            if (instance != this) //instance가 내가 아니라면 이미 instance가 하나 존재하고 있다는 의미
                Destroy(this.gameObject); //둘 이상 존재하면 안되는 객체이니 방금 AWake된 자신을 삭제
        }

        lUISet = new List<GameObject>
        {
            PressAnyKey,
            TitleMenu,
            PauseMenu,
            Ranking,
            MissionFailed_ui,
            DefuseSuccess_ui
        };

        RankTexts = new Text[15];
        RankTexts = RankParent.GetComponentsInChildren<Text>();

    }
    private void Update()
    {
        OFFPressAnyKey();

        if (Input.GetKeyDown(KeyCode.Escape) && SceneMG.Instance.GetNowScene() == 2 && GameManager.Instance.nowPlaying)
        {
            if (PauseMenu.activeSelf)
            {
                ContinueButton();
            }
            else
            {
                Time.timeScale = 0;
                PauseMenu.SetActive(true);
                SoundMG.Instance.PauseSound();
            }
        }
    }

    void OFFPressAnyKey()
    {
        if (PressAnyKey.activeSelf)
        {
            if (Input.anyKeyDown)
            {
                PressAnyKey.SetActive(false);

                StartCoroutine(CameraMove());
            }
        }
    }

    IEnumerator CameraMove()
    {
        WaitForFixedUpdate wffu = new WaitForFixedUpdate();

        while (0 <= introCamera.transform.position.x)
        {
            introCamera.transform.Translate(Vector3.left * cameraMovespeed);
            yield return wffu;
        }

        TitleMenu.SetActive(true);

        introCamera.gameObject.SetActive(false);

        introCamera.position = cameraOriPos;

    }
    public void ContinueButton()
    {
        Time.timeScale = 1;

        PauseMenu.SetActive(false);
        SoundMG.Instance.UnPauseSound();
    }

    public void RestartButton()
    {
        PauseMenu.SetActive(false);

        OFF_All_UI();

        SceneMG.Instance.RestartScene();
    }

    public void GoTitle()
    {
        OFF_All_UI();

        TitleMenu.SetActive(true);

        SceneMG.Instance.GoToTitle();

        //        introCamera.gameObject.SetActive(true);
    }

    public void OFF_All_UI()
    {
        foreach (var ui in lUISet)
        {
            if (ui.activeSelf) ui.SetActive(false);
        }
    }

    public void GameStart()
    {
        OFF_All_UI();
        Time.timeScale = 1;
        introCamera.gameObject.SetActive(false);
        PlayerPrefs.SetInt("TryCount", PlayerPrefs.GetInt("TryCount") + 1);


        SceneMG.Instance.GoGameScene();
    }

    public void HowToPlay()
    {
        OFF_All_UI();
        TitleMenu.SetActive(true);
        var panel = TitleMenu.transform.GetChild(0).gameObject;
        panel.SetActive(true);
        panel.transform.GetChild(1).gameObject.SetActive(true);

    }
    public void RankingUI()
    {
        OFF_All_UI();
        Ranking.SetActive(true);

        if (SceneMG.Instance.GetNowScene() == 1)
        {
            Ranking.transform.GetChild(0).transform.GetChild(5).gameObject.SetActive(false); // 리스타트 뜨냐 안뜨냐
        }

        LeaderBoard.Instance.GetLeaderBoardInfo();
    }
    public void RankingNext()
    {
        rankingPageIndex++;
        LeaderBoard.Instance.GetLeaderBoardInfo();
    }
    public void RankingPrev()
    {
        if (rankingPageIndex <= 0)
        {
            rankingPageIndex = 0;
            return;
        }
        else
        {
            rankingPageIndex--;
            LeaderBoard.Instance.GetLeaderBoardInfo();
        }
    }

    public void MissionFail()
    {
        OFF_All_UI();
        MissionFailed_ui.SetActive(true);
    }

    public void DefuseSuccess()
    {
        OFF_All_UI();
        DefuseSuccess_ui.SetActive(true);
    }

    public void ExitUI()
    {
        Application.Quit();
    }

    public void TryCountUI()
    {
        int _tryCount = PlayerPrefs.GetInt("TryCount");
        tryCount_ui.text = _tryCount.ToString();
    }

    public void RankingTextColorReset()
    {
        foreach (var text in RankTexts)
        {
            text.color = Color.white;
        }
    }
}
