using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ź �ִ� ��
/// </summary>

public class BombGarbageBin : MonoBehaviour
{
    [SerializeField] Animator animator_GarbageBin;

    [SerializeField] Transform holdPosition;

    //BombBinStateMachine bStateMahcine;
    [Header("���� ���ϸ�")]
    [SerializeField] string bombAndDispose;

    [Header("���� ���ϸ�")]
    [SerializeField] public string openDoor;
    [SerializeField] public string closeDoor;

    [Header("���ִ� ����Ʈ")]
    [SerializeField] GameObject effect;
    private void Start()
    {
        //bStateMahcine = ScriptableObject.CreateInstance<BombBinStateMachine>();
        GameManager.Instance.attacked += CloseDoor;
        GameManager.Instance.acCloseDoorWhenAlram = CloseDoorWhenArlam; // �˶��︱�� ���� ���������� �ݰ��ϱ�����
    }

    void OpenDoor()
    {
        animator_GarbageBin.SetBool("isOpenNow", true);
        SoundMG.Instance.PlaySFX(openDoor);
    }
    void CloseDoorWhenArlam()
    {
        CloseDoor();
    }

    void CloseDoor()
    {
        Debug.Log("������");

        animator_GarbageBin.SetBool("isOpenNow", false);
    }

    private void OnMouseDown()
    {
        Debug.Log("��Ŭ");

        if (animator_GarbageBin.GetBool("isOpenNow")) CloseDoor();
        else OpenDoor();
    }

    private void OnTriggerStay(Collider other)
    {
        var otherParent = other.transform.parent;

        if (other.CompareTag("Key")) { }
        else if (otherParent.CompareTag("Bomb"))
        {
            other.gameObject.layer = default;
            otherParent.transform.position = holdPosition.position;
        }
        if (other.transform.CompareTag("Bomb") && animator_GarbageBin.GetBool("DestroyBomb"))
        {
            Instantiate(effect, other.transform.position, other.transform.rotation);

            other.gameObject.layer = 6;
            Destroy(other.transform.root.gameObject);
            SoundMG.Instance.PlaySFX(bombAndDispose);
            //var p = other.transform.parent;
            //p.gameObject.SetActive(false);

            GameManager.Instance.BombHeal();
        }
    }

}
