using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndButton : MonoBehaviour
{
    [SerializeField] GameObject clearEffect;

    [Header("사운드 파일명")]
    [SerializeField] string clearEffectSound; // 폭죽사운드
    [SerializeField] string clearCelebritySound; // 환호사운드

    [SerializeField] string OpenEffectSound; // 버튼누를때 사운드

    bool isPushed = false;

    [SerializeField] float delayTime = 4.0f; // 팝업창 뜨는 시간

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

        UIManager.Instance.TryCountUI(); // 마지막에 Try카운트 띄어주는 것

        LeaderBoard.Instance.SendTryCount();

        //PlayerPrefs.SetInt("TryCount", 0);
    }
}
