using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 폭탄 넣는 통
/// </summary>

public class BombGarbageBin : MonoBehaviour
{
    [SerializeField] Animator animator_GarbageBin;

    [SerializeField] Transform holdPosition;

    //BombBinStateMachine bStateMahcine;
    [Header("사운드 파일명")]
    [SerializeField] string bombAndDispose;

    [Header("사운드 파일명")]
    [SerializeField] public string openDoor;
    [SerializeField] public string closeDoor;

    [Header("없애는 이팩트")]
    [SerializeField] GameObject effect;
    private void Start()
    {
        //bStateMahcine = ScriptableObject.CreateInstance<BombBinStateMachine>();
        GameManager.Instance.attacked += CloseDoor;
        GameManager.Instance.acCloseDoorWhenAlram = CloseDoorWhenArlam; // 알람울릴때 문이 열려있을시 닫게하기위해
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
        Debug.Log("닫혀라문");

        animator_GarbageBin.SetBool("isOpenNow", false);
    }

    private void OnMouseDown()
    {
        Debug.Log("마클");

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
