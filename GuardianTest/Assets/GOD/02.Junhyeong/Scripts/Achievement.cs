using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
/// <summary>
/// 스팀 도전과제
/// </summary>

public enum eAchieveState
{ Default, Alarm, ExplosionBomb, TurnOnLight, PhoneCall, IntoHolder, DeleteKey, ActivateSpeed, DefuseSuccess, }

public class Achievement : MonoBehaviour
{
    private static Achievement instance = null;

    public eAchieveState nowAchieveState = eAchieveState.Default;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void AchieveCheck(eAchieveState _nowState)
    {

        //return; /// 테스트용

        nowAchieveState = _nowState;

        switch (nowAchieveState)
        {
            case eAchieveState.Default:
                break;
            case eAchieveState.Alarm:
                SteamUserStats.SetAchievement("Alarm");
                break;
            case eAchieveState.ExplosionBomb:
                SteamUserStats.SetAchievement("ExplosionBomb");
                break;
            case eAchieveState.TurnOnLight:
                SteamUserStats.SetAchievement("TurnOnLight");
                break;
            case eAchieveState.PhoneCall:
                SteamUserStats.SetAchievement("PhoneCall");
                break;
            case eAchieveState.IntoHolder:
                SteamUserStats.SetAchievement("IntoHolder");
                break;
            case eAchieveState.DeleteKey:
                SteamUserStats.SetAchievement("DeleteKey");
                break;
            case eAchieveState.ActivateSpeed:
                SteamUserStats.SetAchievement("ActivateSpeed");
                break;
            case eAchieveState.DefuseSuccess:
                SteamUserStats.SetAchievement("Success");
                break;
            default:
                break;
        }

        SteamUserStats.StoreStats();

        nowAchieveState = eAchieveState.Default;
    }

    public static Achievement Instance
    {
        get
        { return instance; }
    }
}
