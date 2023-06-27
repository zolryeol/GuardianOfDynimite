using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 사운드매니저 역시 싱글톤
/// </summary>

public class SoundMG : MonoBehaviour
{
    private static SoundMG instance = null;
    public static SoundMG Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SoundMG>();
            }
            return instance;
        }
    }

    [Header("사운드 발생지")]
    [SerializeField] AudioSource bgmPlayer;
    [SerializeField] AudioSource[] sfxPlayer;

    List<AudioSource> tempSource;

    [Header("사운드파일 넣는 곳")]
    [SerializeField]
    List<AudioClip> lBgmAudioClip;
    [SerializeField]
    List<AudioClip> lSfxAudioClip;

    AudioMixer mixerOri;

    [SerializeField] GameObject mainCam;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        tempSource = new List<AudioSource>();

        mixerOri = Resources.Load<AudioMixer>("SoundResource/AudioMixer");

        lSfxAudioClip = new List<AudioClip>(Resources.LoadAll<AudioClip>("SoundResource/SFX"));

        sfxPlayer = new AudioSource[lSfxAudioClip.Count];

        mainCam = GameObject.Find("Main Camera").gameObject;

        for (int i = 0; i < lSfxAudioClip.Count; ++i)
        {
            mixerOri.FindMatchingGroups("Master/SFX")[i + 1].name = lSfxAudioClip[i].name;  // 믹서에 순서대로 집어넣는다.

            var c = mainCam.gameObject.AddComponent<AudioSource>();    // 일단 카메라에 소리발생지 추가

            c.clip = lSfxAudioClip[i];  // 해당 발생지에 클립 추가

            sfxPlayer[i] = c;

            if (mixerOri.FindMatchingGroups("Master/SFX")[i + 1].name == lSfxAudioClip[i].name)
            {
                sfxPlayer[i].outputAudioMixerGroup = mixerOri.FindMatchingGroups("Master/SFX")[i + 1];
            }
            else { Debug.LogError("문제가 있다"); }

            Debug.LogWarning(mixerOri.FindMatchingGroups("Master/SFX")[i].name);
        }


        UnPauseSound();
    }

    public void LoopSound(string _sound)
    {
        foreach (var sfxp in sfxPlayer)
        {
            if (sfxp.clip.name == _sound)
            {
                //Debug.Log("루프해라" + _sound);
                sfxp.loop = true;
            }
        }
    }

    public void StopSFX(string _sound)
    {
        foreach (var sfxp in sfxPlayer)
        {
            if (sfxp.clip.name == _sound && sfxp.isPlaying == true)
            {

                sfxp.Stop();
            }
        }
    }

    public void StopAllSound()
    {
        foreach (var sfxp in sfxPlayer)
        {
            if (sfxp.isPlaying == true)
            {
                sfxp.Stop();
            }
        }
    }

    public void PauseAllSound()
    {
        foreach (var sfxp in sfxPlayer)
        {
            if (sfxp.isPlaying == true)
            {
                sfxp.Pause();
            }
        }
    }
    public void UnPauseAllSound()
    {
        foreach (var sfxp in sfxPlayer)
        {
            if (sfxp.isPlaying == true)
            {
                sfxp.UnPause();
            }
        }
    }
    public void AnotherPlay(string _sound)
    {
        foreach (var sfxp in sfxPlayer)
        {
            if (sfxp.clip.name == _sound)
            {
                if (sfxp.isPlaying) // 이미 오디오소스를 사용중일때 새로만들어주어야함
                {
                    foreach (var temps in tempSource)
                    {
                        if (!temps.isPlaying)
                        {
                            temps.Play();
                            return;
                        }
                        else continue;
                    }
                    // 탬프를 뒤졌는데 없다?
                    // 그럼 새로 만들어주어야한다.
                    tempSource.Add(new AudioSource());
                    tempSource[tempSource.Count - 1] = sfxp;
                    tempSource[tempSource.Count - 1].Play();

                    
                }
                else
                {
                    sfxp.Play();
                }
            }
        }
    }

    public bool IsPlayingSFX(string _sound)
    {
        foreach (var sfxp in sfxPlayer)
        {
            if (sfxp.clip.name == _sound)
            {
                return sfxp.isPlaying;
            }
        }
        return false;
    }

    public void PlaySFX(string _SoundName)
    {
        foreach (var clips in sfxPlayer)
        {
            if (clips.clip.name == _SoundName)
            {
                clips.Play();
                //Debug.Log(clips.clip.name + "소리 재생");
                return;
            }
        }
    }

    public void PauseSound()
    {
        AudioListener.pause = true;
    }
    public void UnPauseSound()
    {
        AudioListener.pause = false;
    }
}
