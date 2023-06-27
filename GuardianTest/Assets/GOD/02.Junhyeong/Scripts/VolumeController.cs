using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VolumeController : MonoBehaviour
{
    // Start is called before the first frame update

    static VolumeController instance = null;

    public static VolumeController Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<VolumeController>();

                if (instance == null) instance = new VolumeController();
            }
                return instance;
        }
    }


    LiftGammaGain lgg;
    Volume volume;
    Vector4 oriGain;

    private void Awake()
    {
        volume = GetComponent<Volume>();
        volume.profile.TryGet<LiftGammaGain>(out lgg);

        oriGain = lgg.gain.value;
    }

    public void LGGainSwitch(bool _onOff)
    {
        lgg.gain.overrideState = _onOff;    // 트루가 꺼지는것;
    }

}
