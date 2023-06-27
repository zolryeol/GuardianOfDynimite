using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ΰ� �� ���̵�ƿ�
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
        float fadeCount = 0; // ���İ�

        WaitForSeconds wfs = new WaitForSeconds(0.01f);



        while (fadeCount <= 1.0f) // 255 ���ƴ϶� 0~1���̰����� ����.
        {
            fadeCount += 0.01f;
            yield return wfs; // 0.01 �ʸ��� �����ų���̴�.

            logo.color = new Color(0, 0, 0, fadeCount);
        }

        SceneMG.Instance.NextScene();
    }


    IEnumerator FadeInCoroutine()
    {
        float fadeCount = 1; // ���İ�

        WaitForSeconds wfs = new WaitForSeconds(0.01f);

        while (fadeCount >= 0.0f) // 255 ���ƴ϶� 0~1���̰����� ����.
        {
            fadeCount -= 0.01f;
            yield return wfs; // 0.01 �ʸ��� �����ų���̴�.

            logo.color = new Color(0, 0, 0, fadeCount);
        }

        FadeOut();
    }
}
