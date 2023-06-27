using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 폭탄이 터지거나 진행 중 일정확률로 벨이 울린다.
/// </summary>

public class Calling : MonoBehaviour
{
    [SerializeField] Animator animator_Call;

    [SerializeField] GameObject balloon;

    Material red_Material;
    Color red_OriColor;
    Material green_Material;
    Color green_OriColor;

    [Header("사운드 파일명")]
    [SerializeField] string callingSound;
    [SerializeField] string cutCalling;

    private void Start()
    {
        GameManager.Instance.attacked += OnBell;
        GameManager.Instance.acCalling += OnBell;
        GameManager.Instance.acAllStop += OffBellForEnding;

        animator_Call.SetBool("isCalled", false);
        balloon.SetActive(false);

        ChangeEmissionForOnOff.InitImissionColor(this.transform, ref red_Material, ref green_Material, ref red_OriColor, ref green_OriColor);

        SoundMG.Instance.LoopSound(callingSound);
    }

    public void OnBell()
    {
        // 사운드 울리는것;
        //Debug.Log("벨이울리다");

        balloon.SetActive(true);

        animator_Call.SetBool("isCalled", true);

        if (!SoundMG.Instance.IsPlayingSFX(callingSound)) SoundMG.Instance.PlaySFX(callingSound); //기존에 재생하는게 없다면 재생시킴

        ChangeEmissionForOnOff.ChangeEmissionColor(red_Material, green_Material, red_OriColor);
    }

    public void OffBell() // 
    {
        //Debug.Log("벨이꺼지다");

        balloon.SetActive(false);

        animator_Call.SetBool("isCalled", false);

        SoundMG.Instance.StopSFX(callingSound);

        SoundMG.Instance.PlaySFX(cutCalling);

        Achievement.Instance.AchieveCheck(eAchieveState.PhoneCall);

        ChangeEmissionForOnOff.ChangeEmissionColor(green_Material, red_Material, green_OriColor);
    }

    public void OffBellForEnding() // 
    {
        balloon.SetActive(false);

        animator_Call.SetBool("isCalled", false);

        SoundMG.Instance.StopSFX(callingSound);

        ChangeEmissionForOnOff.ChangeEmissionColor(green_Material, red_Material, green_OriColor);
    }

    private void OnMouseDown()
    {
        if (animator_Call.GetBool("isCalled")) OffBell();
        //else OnBell();
    }

}
