using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBin : MonoBehaviour
{
    [SerializeField] Transform buttonBinDoor;
    [SerializeField] Transform triggerPos;
    Collider colliderBox;

    [Header("사운드 파일명")]
    [SerializeField] string ButtonBinClose;
    [SerializeField] string ButtonBinOpen;

    private void Awake()
    {
        colliderBox = this.GetComponentInChildren<BoxCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            Debug.Log("Bin에들어온키는 " + other.name);

            other.transform.position = triggerPos.position;
            other.attachedRigidbody.angularVelocity = Vector3.zero;
            other.attachedRigidbody.velocity = Vector3.zero;


            SoundMG.Instance.PlaySFX(ButtonBinOpen);

            StartCoroutine(DoorDown(other.gameObject));

            Achievement.Instance.AchieveCheck(eAchieveState.DeleteKey);
        }

        Debug.Log("버튼쓰레기통에 들어옴");
        Debug.Log(other.name);
    }

    IEnumerator DoorDown(GameObject _targetGameObj)
    {
        WaitForFixedUpdate wffu = new WaitForFixedUpdate();
        WaitForSeconds wfs = new WaitForSeconds(0.01f);

        Vector3 oriPos = buttonBinDoor.position;

        float targetY = oriPos.y - 1f; // -1f 는 닫힐만큼의 간격임

        Vector3 targetPos = oriPos;

        while (targetY <= buttonBinDoor.position.y)
        {
            targetPos.y -= 0.05f;

            buttonBinDoor.position = targetPos;

            yield return wfs;
        }

        Destroy(_targetGameObj.gameObject);

        SoundMG.Instance.PlaySFX(ButtonBinClose);

        while (buttonBinDoor.position.y <= oriPos.y)
        {
            targetPos.y += 0.05f;

            buttonBinDoor.position = targetPos;

            yield return wfs;
        }

        buttonBinDoor.position = oriPos;
    }
}
