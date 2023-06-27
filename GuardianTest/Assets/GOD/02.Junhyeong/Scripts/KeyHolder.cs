using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// 키가 들어갈 수 있는 곳 버튼을 누르는 것도 여기서 확인한다.
public class KeyHolder : MonoBehaviour
{
    CKey holdingKey = null;
    [SerializeField] Mesh questionMarkMesh;

    Color oriColor;
    [SerializeField] Material rightMaterial;
    [SerializeField] Material wrongMaterial;
    Material tempMaterial;
    Collider[] holderCol;
    WaitForSeconds wfs; // 코루틴사용용

    [Header("사운드 파일명")]
    [SerializeField] string collectSound;
    [SerializeField] string wrongSound;
    [SerializeField] string holdKey;
    [SerializeField] string pushKey;


    int heal;
    int damage;

    System.Action PushRightColor => PushedRightButton;
    System.Action PushWrongColor => PushedWrongButton;

    private void Start()
    {
        wfs = new WaitForSeconds(GameManager.Instance.pushFlikerSpeed);
        heal = GameManager.Instance.healPoint * 2;
        damage = GameManager.Instance.healPoint * 2;
        holderCol = GetComponents<Collider>();
    }
    void ChangeToQuestionMark(Collider other)
    {
        other.GetComponent<MeshFilter>().mesh = questionMarkMesh;

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Key") && holdingKey == null)
        {
            Debug.Log("이그놀 호출");

            Physics.IgnoreCollision(collision.collider, holderCol[1]);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            if (holdingKey == null)
            {
                other.transform.position = transform.position;
                other.attachedRigidbody.velocity = Vector3.zero;
                other.attachedRigidbody.angularVelocity = Vector3.zero;

                holdingKey = other.GetComponent<CKey>();

                ChangeToQuestionMark(other);
                tempMaterial = other.GetComponent<MeshRenderer>().materials[1];
                oriColor = tempMaterial.color;

                SoundMG.Instance.PlaySFX(holdKey);
            }
        }
    }

    void PushedWrongButton() => tempMaterial.color = wrongMaterial.color;
    void PushedRightButton() => tempMaterial.color = rightMaterial.color;
    void BackOriButton() => tempMaterial.color = oriColor;
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CKey>() == holdingKey)
            holdingKey = null;
    }

    void HealSelf()
    {
        GameManager.Instance.basketHP += heal;
        if (GameManager.Instance.basketMAXHP <= GameManager.Instance.basketHP) GameManager.Instance.basketHP = GameManager.Instance.basketMAXHP;
        GameManager.Instance.acClockRotate();
    }

    void WrongInputDamage()
    {
        GameManager.Instance.basketHP -= damage;
        GameManager.Instance.acClockRotate();
    }

    private void Update()
    {
        if (Input.anyKeyDown && holdingKey != null)
        {
            foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
            {
                /// 올바른 키를 눌렀을 때
                if (Input.GetKey(vKey) && (holdingKey.GetKeyType == vKey.ToString() || holdingKey.GetKeyTypeKeyPad == vKey.ToString()))
                {
                    Debug.Log(vKey.ToString() + " 가 눌려짐");
                    holdingKey.AttackKey();
                    StartCoroutine(Fliker(PushRightColor));
                    HealSelf();
                    if (holdingKey.keyHp <= 0) holdingKey = null;

                    SoundMG.Instance.PlaySFX(collectSound);
                    SoundMG.Instance.PlaySFX(pushKey);
                    Achievement.Instance.AchieveCheck(eAchieveState.IntoHolder);
                    return;
                }
                /// 마우스 예외처리
                else if (Input.GetKey(vKey) && vKey == KeyCode.Mouse0)
                {
                    Debug.Log("좌클릭");
                    return;
                }
            }
            /// 잘못 눌렀을 때
            Debug.Log("잘못누르심");

            //SoundMG.Instance.AnotherPlay(wrongSound); // 테스트가 안되어 일단 내비둔다.
            SoundMG.Instance.PlaySFX(wrongSound);

            SoundMG.Instance.PlaySFX(pushKey);

            StartCoroutine(Fliker(PushWrongColor));
            WrongInputDamage();
        }
    }

    IEnumerator Fliker(System.Action _changeColor)
    {
        _changeColor();
        yield return wfs;
        BackOriButton();
    }
}
