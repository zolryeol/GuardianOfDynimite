using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 화재경보기가 울리면 차단기가 3개 내려간다.
/// 다시누르면 올라간다.
/// </summary>

public class FireAlram : MonoBehaviour
{
    [SerializeField] Animator animator_FireAlram;
    [SerializeField] Transform alramLight;
    [SerializeField] float alramLightRotationSpeed = 5f;

    bool stopRight = true;

    [Header("사운드 파일명")]
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

        SoundMG.Instance.PlaySFX(shutterDownSound); // 내려가는 사운드

        GameManager.Instance.acCloseDoorWhenAlram(); // 예외처리

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
        Debug.Log("마클");
        if (animator_FireAlram.GetBool("IsClose"))
        {
            AlramOff();   // 아래로 내려가있을때 클릭하면 올린다
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
