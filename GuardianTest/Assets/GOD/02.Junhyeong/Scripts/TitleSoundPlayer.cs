using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSoundPlayer : MonoBehaviour
{
    [SerializeField] AudioSource titleBgm;
    private void Awake()
    {
        titleBgm = this.GetComponent<AudioSource>();
    }
    private void Start()
    {
        AudioListener.pause = false;
        titleBgm.Play();
    }

}
