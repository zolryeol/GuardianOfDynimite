using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ź�� �����ų� ���� �� ����Ȯ���� ���� �︰��.
/// </summary>

public class Calling : MonoBehaviour
{
    [SerializeField] Animator animator_Call;

    [SerializeField] GameObject balloon;

    Material red_Material;
    Color red_OriColor;
    Material green_Material;
    Color green_OriColor;

    [Header("���� ���ϸ�")]
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
        // ���� �︮�°�;
        //Debug.Log("���̿︮��");

        balloon.SetActive(true);

        animator_Call.SetBool("isCalled", true);

        if (!SoundMG.Instance.IsPlayingSFX(callingSound)) SoundMG.Instance.PlaySFX(callingSound); //������ ����ϴ°� ���ٸ� �����Ŵ

        ChangeEmissionForOnOff.ChangeEmissionColor(red_Material, green_Material, red_OriColor);
    }

    public void OffBell() // 
    {
        //Debug.Log("���̲�����");

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
