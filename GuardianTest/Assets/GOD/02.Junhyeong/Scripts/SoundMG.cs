using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// ����Ŵ��� ���� �̱���
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

    [Header("���� �߻���")]
    [SerializeField] AudioSource bgmPlayer;
    [SerializeField] AudioSource[] sfxPlayer;

    List<AudioSource> tempSource;

    [Header("�������� �ִ� ��")]
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
            mixerOri.FindMatchingGroups("Master/SFX")[i + 1].name = lSfxAudioClip[i].name;  // �ͼ��� ������� ����ִ´�.

            var c = mainCam.gameObject.AddComponent<AudioSource>();    // �ϴ� ī�޶� �Ҹ��߻��� �߰�

            c.clip = lSfxAudioClip[i];  // �ش� �߻����� Ŭ�� �߰�

            sfxPlayer[i] = c;

            if (mixerOri.FindMatchingGroups("Master/SFX")[i + 1].name == lSfxAudioClip[i].name)
            {
                sfxPlayer[i].outputAudioMixerGroup = mixerOri.FindMatchingGroups("Master/SFX")[i + 1];
            }
            else { Debug.LogError("������ �ִ�"); }

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
                //Debug.Log("�����ض�" + _sound);
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
                if (sfxp.isPlaying) // �̹� ������ҽ��� ������϶� ���θ�����־����
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
                    // ������ �����µ� ����?
                    // �׷� ���� ������־���Ѵ�.
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
                //Debug.Log(clips.clip.name + "�Ҹ� ���");
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
