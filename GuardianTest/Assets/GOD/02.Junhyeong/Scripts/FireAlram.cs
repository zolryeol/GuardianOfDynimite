using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ȭ��溸�Ⱑ �︮�� ���ܱⰡ 3�� ��������.
/// �ٽô����� �ö󰣴�.
/// </summary>

public class FireAlram : MonoBehaviour
{
    [SerializeField] Animator animator_FireAlram;
    [SerializeField] Transform alramLight;
    [SerializeField] float alramLightRotationSpeed = 5f;

    bool stopRight = true;

    [Header("���� ���ϸ�")]
    [SerializeField] public string shutterDownSound;
    [SerializeField] public string shutterUpSound;
    [SerializeField] string siren;
    [SerializeField] string siren2;

    private void Start()
    {
        GameManager.Instance.attacked += AlramOn;
        GameManager.Instance.acAllStop += AlarmForEnding;
        //GameManager.Instance.acFireAlram += AlramOn;
        SoundMG.Instance.LoopSound(siren);
        SoundMG.Instance.LoopSound(siren2);
    }

    void AlramOn()
    {
        if (animator_FireAlram.GetBool("IsClose"))
        {
            return;
        }

        animator_FireAlram.SetBool("IsClose", true);

        SoundMG.Instance.PlaySFX(shutterDownSound); // �������� ����

        GameManager.Instance.acCloseDoorWhenAlram(); // ����ó��

        stopRight = false;

        Achievement.Instance.AchieveCheck(eAchieveState.Alarm);

        StartCoroutine(RotateRedLight());
    }

    void AlarmForEnding()
    {
        if (animator_FireAlram.GetBool("IsClose"))
        {
            AlramOff();
        }
    }

    void AlramOff()
    {
        animator_FireAlram.SetBool("IsClose", false);
        stopRight = true;
        SoundMG.Instance.PlaySFX(shutterUpSound);
        SoundMG.Instance.StopSFX(siren);
        SoundMG.Instance.StopSFX(siren2);
    }
    IEnumerator RotateRedLight()
    {
        WaitForFixedUpdate wffu = new WaitForFixedUpdate();

        if (!SoundMG.Instance.IsPlayingSFX(siren))
        {
            SoundMG.Instance.PlaySFX(siren);
        }
        if (!SoundMG.Instance.IsPlayingSFX(siren2))
        {
            SoundMG.Instance.PlaySFX(siren2);
        }


        while (stopRight == false)
        {
            alramLight.Rotate(Vector3.right, alramLightRotationSpeed);

            yield return wffu;
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("��Ŭ");
        if (animator_FireAlram.GetBool("IsClose"))
        {
            AlramOff();   // �Ʒ��� ������������ Ŭ���ϸ� �ø���
        }
    }
    // 
    //     private void Update()
    //     {
    //         if (Input.GetMouseButtonDown(1))
    //         {
    //             AlramOn();
    //         }
    //     }
}
