using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ź���� �ð�ٴ��� �����Ѵ�.
/// </summary>
public class BasketTimeController : MonoBehaviour
{
    [SerializeField] Transform clockHand;
    [SerializeField] float rotaDegree; // ü���� 1�������� ȸ���� ������ ����;
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

    IEnumerator CorChangeClockHand() // ����ü����Ȯ���ؼ� �ð�ٴ��� ������
    {
        WaitForEndOfFrame wfef = new WaitForEndOfFrame();

        int nowHP = GameManager.Instance.basketHP;

        var temp = clockHand.rotation; // ���� �����̼ǰ����ͼ�

        if (preHp < nowHP) //ȸ���Ǿ��ٸ�
        {
            for (int i = preHp; i < nowHP; ++i)
            {
                temp = Quaternion.Euler(new Vector3(temp.eulerAngles.x, temp.eulerAngles.y, (preHp++) * rotaDegree));

                clockHand.localRotation = temp;
                yield return wfef;
            }
        }
        else  // HP�� �پ��ٸ�
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

        var temp = clockHand.rotation; // ���� �����̼ǰ����ͼ�

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
    //             Debug.Log("ü���� �پ���. ����ü�� = " + GameManager.Instance.basketHP);
    //             //ChangeClockHand();
    //         }
    // 
    //         if (Input.GetKey(KeyCode.Z))
    //         {
    //             GameManager.Instance.basketHP++;
    //             Debug.Log("ü���� �þ���. ����ü�� = " + GameManager.Instance.basketHP);
    //             //ChangeClockHand();
    //         }
    // 
    //         if (Input.GetKeyDown(KeyCode.X))
    //         {
    //             StartCoroutine(CorChangeClockHand());
    //         }
    //     }
}
