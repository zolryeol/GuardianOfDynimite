using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ElectroSwitch : MonoBehaviour
{
    [SerializeField] GameObject outLine;
    [SerializeField] ParticleSystem particle;
    [SerializeField] Animator animator_ElecSwitch;
    [SerializeField] GameObject realTimeLight;

    Material red_Material;
    Color red_OriColor;
    Material green_Material;
    Color green_OriColor;

    [Header("���� ���ϸ�")]
    [SerializeField] string switchDown;
    [SerializeField] string switchUp;

    private void Start()
    {
        GameManager.Instance.attacked += HandleDown;
        GameManager.Instance.acElectroSwitch += HandleDown;
        GameManager.Instance.acAllStop += HandleUp;


        ChangeEmissionForOnOff.InitImissionColor(this.transform, ref red_Material, ref green_Material, ref red_OriColor, ref green_OriColor);
    }

    void HandleDown()
    {
        if (!animator_ElecSwitch.GetBool("HandleOn")) SoundMG.Instance.PlaySFX(switchDown); //handleOn == false �� �ߵ�

        animator_ElecSwitch.SetBool("HandleOn", true);

        particle.Play();

        VolumeController.Instance.LGGainSwitch(true);

        ChangeEmissionForOnOff.ChangeEmissionColor(red_Material, green_Material, red_OriColor);

        realTimeLight.SetActive(false);

    }

    void HandleUp()
    {
        animator_ElecSwitch.SetBool("HandleOn", false);
        VolumeController.Instance.LGGainSwitch(false);

        ChangeEmissionForOnOff.ChangeEmissionColor(green_Material, red_Material, green_OriColor);

        Achievement.Instance.AchieveCheck(eAchieveState.TurnOnLight);

        realTimeLight.SetActive(true);

        SoundMG.Instance.PlaySFX(switchUp);
    }
    private void OnMouseDown()
    {
        Debug.Log("��Ŭ");
        if (animator_ElecSwitch.GetBool("HandleOn")) HandleUp();   // �Ʒ��� ������������ Ŭ���ϸ� �ø���
        else HandleDown();
    }

}
