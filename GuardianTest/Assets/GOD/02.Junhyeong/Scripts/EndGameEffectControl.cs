using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameEffectControl : MonoBehaviour
{
    ParticleSystem[] particles;
    [SerializeField]float delayTime = 0.5f;
    private void Awake()
    {
        particles = new ParticleSystem[transform.childCount];

        particles = GetComponentsInChildren<ParticleSystem>();
    }

    private void OnEnable()
    {
    }
}
