using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndButton : MonoBehaviour
{
    [SerializeField] GameObject clearEffect;

    [Header("���� ���ϸ�")]
    [SerializeField] string clearEffectSound; // ���׻���
    [SerializeField] string clearCelebritySound; // ȯȣ����

    [SerializeField] string OpenEffectSound; // ��ư������ ����

    bool isPushed = false;

    [SerializeField] float delayTime = 4.0f; // �˾�â �ߴ� �ð�

    private void Start()
    {
        SoundMG.Instance.LoopSound(clearEffectSound);
    }

    private void OnMouseDown()
    {

        if (isPushed == false)
        {
            isPushed = true;

            GameManager.Instance.acAllStop();

            SoundMG.Instance.PlaySFX(OpenEffectSound);

            ClearEffect();

            Invoke(nameof(DefuseSuccessUI), 2f);

        }
    }

    void ClearEffect()
    {
        GameManager.Instance.nowPlaying = false;
        clearEffect.SetActive(true);
        SoundMG.Instance.PlaySFX(clearEffectSound);
        SoundMG.Instance.PlaySFX(clearCelebritySound);

        Achievement.Instance.AchieveCheck(eAchieveState.DefuseSuccess);
    }

    void ResetTryCount()
    {
        PlayerPrefs.SetInt("TryCount", 0);

        PlayerPrefs.Save();
    }

    void DefuseSuccessUI()
    {

        UIManager.Instance.DefuseSuccess();

        UIManager.Instance.TryCountUI(); // �������� Tryī��Ʈ ����ִ� ��

        LeaderBoard.Instance.SendTryCount();

        //PlayerPrefs.SetInt("TryCount", 0);
    }
}
