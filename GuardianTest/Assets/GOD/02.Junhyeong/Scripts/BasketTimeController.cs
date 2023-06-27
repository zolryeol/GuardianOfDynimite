using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 폭탄통의 시계바늘을 관리한다.
/// </summary>
public class BasketTimeController : MonoBehaviour
{
    [SerializeField] Transform clockHand;
    [SerializeField] float rotaDegree; // 체력이 1떨어질때 회전할 눈금의 각도;
    int preHp;

    void Start()
    {
        GameManager.Instance.acClockRotate += ClockCoroutine;
        GameManager.Instance.attacked += ClockCoroutine;

        rotaDegree = 360 / GameManager.Instance.basketHP;
        preHp = GameManager.Instance.basketHP;
    }

    void ClockCoroutine()
    {
        if (GameManager.Instance.basketHP <= 0) GameManager.Instance.basketHP = 0;

        StartCoroutine(CorChangeClockHand());
    }

    IEnumerator CorChangeClockHand() // 현재체력을확인해서 시계바늘을 움직임
    {
        WaitForEndOfFrame wfef = new WaitForEndOfFrame();

        int nowHP = GameManager.Instance.basketHP;

        var temp = clockHand.rotation; // 월드 로테이션가져와서

        if (preHp < nowHP) //회복되었다면
        {
            for (int i = preHp; i < nowHP; ++i)
            {
                temp = Quaternion.Euler(new Vector3(temp.eulerAngles.x, temp.eulerAngles.y, (preHp++) * rotaDegree));

                clockHand.localRotation = temp;
                yield return wfef;
            }
        }
        else  // HP가 줄었다면
        {
            for (int i = preHp; nowHP <= i; --i)
            {
                var val = preHp - i;
                temp = Quaternion.Euler(new Vector3(temp.eulerAngles.x, temp.eulerAngles.y, (preHp - val) * rotaDegree));

                clockHand.localRotation = temp;
                yield return wfef;
            }
        }
        preHp = nowHP;
    }
    void ChangeClockHand()
    {
        int hpGap = GameManager.Instance.basketMAXHP - GameManager.Instance.basketHP;

        var temp = clockHand.rotation; // 월드 로테이션가져와서

        temp = Quaternion.Euler(new Vector3(temp.eulerAngles.x, temp.eulerAngles.y, -hpGap * rotaDegree));

        Debug.Log(temp.eulerAngles);

        clockHand.localRotation = temp;
    }

    // Update is called once per frame
    //     void Update()
    //     {
    //         if (Input.GetKey(KeyCode.C))
    //         {
    //             GameManager.Instance.basketHP--;
    //             Debug.Log("체력이 줄었다. 현재체력 = " + GameManager.Instance.basketHP);
    //             //ChangeClockHand();
    //         }
    // 
    //         if (Input.GetKey(KeyCode.Z))
    //         {
    //             GameManager.Instance.basketHP++;
    //             Debug.Log("체력이 늘었다. 현재체력 = " + GameManager.Instance.basketHP);
    //             //ChangeClockHand();
    //         }
    // 
    //         if (Input.GetKeyDown(KeyCode.X))
    //         {
    //             StartCoroutine(CorChangeClockHand());
    //         }
    //     }
}
