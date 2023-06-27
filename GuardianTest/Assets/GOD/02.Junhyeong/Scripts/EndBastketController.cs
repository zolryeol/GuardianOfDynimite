using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBastketController : MonoBehaviour
{
    [SerializeField] Animator animator_EndBastket;

    [SerializeField] GameObject clearSmokeEffect;

    [Header("사운드 파일명")]
    [SerializeField] string endDoorOpenSound;


    private void Start()
    {
        GameManager.Instance.acFinalButton += FinalButton;
    }

    public void FinalButton()
    {
        animator_EndBastket.SetBool("EndBasketOpen", true);

        clearSmokeEffect.SetActive(true);

        SoundMG.Instance.PlaySFX(endDoorOpenSound);
    }

}
