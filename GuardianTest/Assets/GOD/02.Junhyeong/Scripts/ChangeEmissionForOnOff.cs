using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 머터리얼 이미션 색상 바꾸는 코드 공용으로 쓰기위함
/// </summary>

static class ChangeEmissionForOnOff
{
    public static void ChangeEmissionColor(Material _OnMaterial, Material _OffMaterial, Color _OnOriColor)
    {
        _OnMaterial.SetColor("_EmissionColor", _OnOriColor);
        _OffMaterial.SetColor("_EmissionColor", Color.black);
    }

    public static void OffEmissionColor(Material _OnMaterial, Material _OnMaterial2)
    {
        _OnMaterial.SetColor("_EmissionColor", Color.black);
        _OnMaterial2.SetColor("_EmissionColor", Color.black);
    }
    public static void OnEmissionColor(Material _OnMaterial, Material _OnMaterial2,Color _OriColor)
    {
        _OnMaterial.SetColor("_EmissionColor", _OriColor);
        _OnMaterial2.SetColor("_EmissionColor", _OriColor);
    }

    public static void InitImissionColor(Transform _that, ref Material _redMaterial, ref Material _greenMaterial, ref Color _redOri, ref Color _greenOri)
    {
        _redMaterial = _that.GetComponent<MeshRenderer>().materials[1];
        _greenMaterial = _that.GetComponent<MeshRenderer>().materials[2];

        _redOri = _redMaterial.GetColor("_EmissionColor");
        _greenOri = _greenMaterial.GetColor("_EmissionColor");

        _redMaterial.SetColor("_EmissionColor", Color.black);
    }


    // 스피드머신용
    public static void InitImissionColor(Transform _that, ref Material _redMaterial, ref Material _greenMaterial, ref Color _Ori)
    {
        _redMaterial = _that.GetComponent<MeshRenderer>().materials[1];
        _greenMaterial = _that.GetComponent<MeshRenderer>().materials[2];

        _Ori = _redMaterial.GetColor("_EmissionColor");

        _redMaterial.SetColor("_EmissionColor", Color.black);
        _greenMaterial.SetColor("_EmissionColor", Color.black);
    }
}
