using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 키가 가져야할 것들
/// </summary>
/// 



public class CKey : MonoBehaviour
{
    // Start is called before the first frame update

    bool goLeft; // 왼쪽으로 출발할지 오른쪽으로 출발할지

    Rigidbody rigidBody;

    bool isPicking = false;

    [SerializeField] float keySpeed; // 움직일때 속도 시작하자마자 매니저에서 받아온다.
    [SerializeField] float keyRota;

    [Header("키속성")]
    public int keyHp;
    [SerializeField] string keyType;
    [SerializeField] string keyTypeKeyPad = null;
    [Header("난이도")]
    [SerializeField] eKeyLevel keyLevel;

    [Header("이팩트")]
    [SerializeField] GameObject keyEffect;

    string collisionKeySound;

    public eKeyLevel GetLevel { get => keyLevel; }

    public string GetKeyType => keyType;
    public string GetKeyTypeKeyPad => keyTypeKeyPad;

    public bool Picking
    {
        get
        {
            return isPicking;
        }
        set
        {
            isPicking = value;
        }
    }

    private void Awake()
    {
        if (TryGetComponent(out Rigidbody rb))
        {
            rigidBody = rb;
            this.gameObject.GetComponent<MeshCollider>().convex = true;
            rigidBody.useGravity = false;
            rigidBody.angularDrag = 0;
        }
        else
        {
            rigidBody = this.gameObject.AddComponent<Rigidbody>();
            this.gameObject.GetComponent<MeshCollider>().convex = true;
            rigidBody.useGravity = false;
            rigidBody.angularDrag = 0;
        }

        keyEffect = Resources.Load<GameObject>("Effect/Key_Crash");
        collisionKeySound = "키 부딪혔을때";
    }
    void Start()
    {
        transform.tag = "Key";
        gameObject.layer = 6;

        KeyInit();

        AssignHP();
    }

    private void OnDisable()
    {
        transform.position = new Vector3(0, 15, 0);
    }

    private void OnEnable()
    {
        KeyInit();
        AssignHP();
    }

    public void KeyInit()
    {
        keySpeed = GameManager.Instance.keyMoveSpeed;
        keyRota = GameManager.Instance.keyRotaSpeed;

        if (transform.position.x == 0) Debug.LogError("시작이 0이라고?");
        if (0 < this.transform.position.x) goLeft = true;
        else goLeft = false;

        //rigidBody.velocity = Vector3.zero;

        if (goLeft)
        {
            rigidBody.AddForce(Vector3.left * keySpeed, ForceMode.VelocityChange);
            rigidBody.AddTorque(Vector3.up * keyRota, ForceMode.Impulse);
        }
        else
        {
            rigidBody.AddForce(Vector3.right * keySpeed, ForceMode.VelocityChange);
            rigidBody.AddTorque(Vector3.down * keyRota, ForceMode.Impulse);
        }

        AssignHP();
    }

    void AssignHP()
    {
        switch (keyLevel)
        {
            case eKeyLevel.EASY:
                keyHp = Random.Range(GameManager.Instance.keyHpRange[(int)eKeyLevel.EASY].minHp, GameManager.Instance.keyHpRange[(int)eKeyLevel.EASY].maxHP + 1);
                break;
            case eKeyLevel.NOMAL:
                keyHp = Random.Range(GameManager.Instance.keyHpRange[(int)eKeyLevel.NOMAL].minHp, GameManager.Instance.keyHpRange[(int)eKeyLevel.NOMAL].maxHP + 1);
                break;
            case eKeyLevel.HARD:
                keyHp = Random.Range(GameManager.Instance.keyHpRange[(int)eKeyLevel.HARD].minHp, GameManager.Instance.keyHpRange[(int)eKeyLevel.HARD].maxHP + 1);
                break;
            case eKeyLevel.BOMB:
                break;
            default:
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Key")) { SoundMG.Instance.PlaySFX(collisionKeySound); }
        else if ((collision.transform.CompareTag("Bomb")) || (collision.transform.CompareTag("Walls"))) { SoundMG.Instance.PlaySFX(collisionKeySound); }
        else
        {
            if (collision.transform.CompareTag("TrashBin") || collision.transform.CompareTag("DefuseLine")) return;

            Debug.Log("키가 " + collision.transform.name + "에 부딪힘");
            SoundMG.Instance.PlaySFX(collisionKeySound);
            if (GameManager.Instance.attacked != null) GameManager.Instance.attacked();
            InstantiateParticle();

            Destroy(gameObject);
        }

    }

    public void AttackKey() // 버튼을 눌러서 체력이 빠진다.
    {
        this.keyHp--;

        Debug.Log("KeyHp -1 / 현재 KeyHp = " + keyHp);

        //GameManager.Instance.Heal();

        if (keyHp <= 0) DeathOfKey();
    }

    public void DeathOfKey() // 키의 체력이 0이되면 죽는다
    {
        Destroy(gameObject);
    }

    void InstantiateParticle()
    {
        Instantiate(keyEffect, transform.position, Quaternion.identity);
    }
}
