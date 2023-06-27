using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 로고에 쓸 페이드아웃
/// </summary>

public class LogoFadeInOut : MonoBehaviour
{
    public Image logo;

    bool isFadeDone = false;

    private void Awake()
    {
        logo = GameObject.Find("Panel").GetComponent<Image>();
    }

    private void Start()
    {
        Invoke("FadeIn", 0.5f);
    }

    public void GoTitle()
    {
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }

    IEnumerator FadeOutCoroutine()
    {
        float fadeCount = 0; // 알파값

        WaitForSeconds wfs = new WaitForSeconds(0.01f);



        while (fadeCount <= 1.0f) // 255 가아니라 0~1사이값으로 쓴다.
        {
            fadeCount += 0.01f;
            yield return wfs; // 0.01 초마다 수행시킬것이다.

            logo.color = new Color(0, 0, 0, fadeCount);
        }

        SceneMG.Instance.NextScene();
    }


    IEnumerator FadeInCoroutine()
    {
        float fadeCount = 1; // 알파값

        WaitForSeconds wfs = new WaitForSeconds(0.01f);

        while (fadeCount >= 0.0f) // 255 가아니라 0~1사이값으로 쓴다.
        {
            fadeCount -= 0.01f;
            yield return wfs; // 0.01 초마다 수행시킬것이다.

            logo.color = new Color(0, 0, 0, fadeCount);
        }

        FadeOut();
    }
}
