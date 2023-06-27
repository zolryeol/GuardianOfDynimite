using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndDoorEmissionControl : MonoBehaviour
{
    [SerializeField] Material[] material;

    [Header("사운드 파일명")]
    [SerializeField] string EmissionOnSound;



    private void Awake()
    {
        material = new Material[4];

        material = GetComponent<Renderer>().materials;
    }

    private void Start()
    {
        GameManager.Instance.acEndDoorEmission += OnEmission;
    }

    public void OnEmission(eEndDoorEmission num)
    {
        material[(int)num].EnableKeyword("_EMISSION");
        SoundMG.Instance.PlaySFX(EmissionOnSound);
    }

    void OnCenter()
    {
        material[1].EnableKeyword("_EMISSION"); // 가운데
    }
    void OnRight() // 오른쪽
    {
        material[2].EnableKeyword("_EMISSION");
    }
    void OnLeft() // 왼쪽
    {
        material[3].EnableKeyword("_EMISSION");
    }
}
