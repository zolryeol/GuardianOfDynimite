using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAlramSirenSoundControl : MonoBehaviour
{
    // Start is called before the first frame update
    FireAlram sFireAlram;
    Animator animator;
    private void Awake()
    {
        sFireAlram = GetComponentInChildren<FireAlram>();
        animator = this.GetComponent<Animator>();
    }

    public void StopShutterDownSound()
    {
        SoundMG.Instance.StopSFX(sFireAlram.shutterDownSound);
    }

    public void StopShutterUpSound()
    {
        SoundMG.Instance.StopSFX(sFireAlram.shutterUpSound);
    }
}
