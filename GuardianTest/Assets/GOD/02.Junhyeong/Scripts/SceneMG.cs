using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneMG : MonoBehaviour
{
    private static SceneMG instance = null;

    public static SceneMG Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SceneMG>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void NextScene()
    {
        SceneManager.LoadScene(GetNowScene() + 1);
    }

    public void GoGameScene()
    {
        SceneManager.LoadScene("Game");
    }

    public void PreScene()
    {
        SceneManager.LoadScene(GetNowScene() - 1);
    }

    public int GetNowScene()
    {
        var nowScene = SceneManager.GetActiveScene();

        return nowScene.buildIndex;
    }
    public void RestartScene()
    {
        UIManager.Instance.OFF_All_UI();
        Time.timeScale = 1;
        GameManager.Instance.retryCount++;
        PlayerPrefs.SetInt("TryCount", PlayerPrefs.GetInt("TryCount") + 1);
        Debug.Log("리스타트시 트라이카운트 " + PlayerPrefs.GetInt("TryCount"));
        PlayerPrefs.Save();
        SceneManager.LoadScene(GetNowScene());
    }

    public void GoToTitle()
    {
        SceneManager.LoadScene("Intro");
    }
}
