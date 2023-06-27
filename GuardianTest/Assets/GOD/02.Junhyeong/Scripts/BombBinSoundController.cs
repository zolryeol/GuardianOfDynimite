using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBinSoundController : MonoBehaviour
{
    BombGarbageBin sDoor;
    private void Awake()
    {
        sDoor = GetComponentInParent<BombGarbageBin>();
    }

    public void CloseDoorSound()
    {
        SoundMG.Instance.PlaySFX(sDoor.closeDoor);
    }
}
