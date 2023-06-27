using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ������ �����ϴ� ���ӸŴ���
/// </summary>

[System.Serializable]
public struct sKeyHpRange
{
    public eKeyLevel keylevel;
    public int minHp;
    public int maxHP;
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

    public Transform spawner;
    public BombSpawner spawner_Bomb;

    public GameObject keyprefabTest;
    public GameObject bombprefabTest;

    [Space(30f)]
    public float keyMoveSpeed; // key ���󰡴� ���ǵ�
    public float keyRotaSpeed; // key ȸ���ӵ�
    public float bombMoveSpeed;

    [Space(30f)]
    public int basketMAXHP = 120;
    public int basketHP = 120;
    [Header("��ư�����̴½ð�")]
    public float pushFlikerSpeed = 0.3f;
    float timer;
    [Header("�־����ð�")]
    public float nowRemainTime = 180;
    float remainTimeMax = 180;

    public float rankingScore;
    public float GetTime { get => timer; }
    public bool nowPlaying = false;
    [Header("Ű ü�� ����")]
    public sKeyHpRange[] keyHpRange = new sKeyHpRange[3];
    [Header("��ź ȸ����")]
    public int bombHealPoint = 1;
    [Header("Ű ȸ����")]
    public int healPoint = 1;
    [Header("Ű ������")]
    public int DamagePoint = 2;
    [Header("���ǵ�ӽ� �ӵ�")]
    public float speedMachineSpeed = 2;

    [Header("��ź Ÿ�̸�")]
    public int BombTimer = 5;
    [Header("��ź ������")]
    public int bombDamage = 30;

    [Header("���� ���̵�")]
    [Space(30f)]
    [SerializeField] eKeyLevel nowLevel;
    [Header("�븻�� �ٲ�� �ð�")]
    [SerializeField] int nomalTime;
    [Header("�ϵ�� �ٲ�� �ð�")]
    [SerializeField] int hardTime;

    [Header("N�ʸ��� ����")]
    [Space(30f)]
    [SerializeField] int createSecond;

    [Header("Easy ����Ȯ��")]
    [Space(30f)]
    [SerializeField] int probabilityEasy = 35;
    [Header("Nomal ����Ȯ��")]
    [SerializeField] int probabilityNomal = 50;
    [Header("Hard ����Ȯ��")]
    [SerializeField] int probabilityHard = 70;

    [Header("Bomb ����Ȯ��")]
    [Space(30f)]
    [SerializeField] int chanceBombEasy = 0;
    [SerializeField] int chanceBombNomal = 25;
    [SerializeField] int chanceBombHard = 35;

    [Header("��ȭ�� �︱Ȯ��")]
    [Space(30f)]
    [SerializeField] int chanceCallingEasy = 25;
    [SerializeField] int chanceCallingNomal = 45;
    [SerializeField] int chanceCallingHard = 65;

    [Header("����ġ ������ Ȯ��")]
    [SerializeField] int chanceSwitchEasy = 25;
    [SerializeField] int chanceSwitchNomal = 45;
    [SerializeField] int chanceSwitchHard = 65;

    [SerializeField] List<GameObject> lEndGameEffect;
    [SerializeField] float endEffectDelayTime = 0.5f;
    [SerializeField] TextMesh remainTimeText;
    public int retryCount;

    [Header("���� ���ϸ�")]
    [SerializeField] string dyingBombSound;

    [SerializeField] string dyingBombSound2;

    [SerializeField] string countSound;

    // ���︮��, �����, ��������ݱ�, ���̷�����, ü�±��, �� �ð�ٴ� ȸ�� ���� �־ �ѹ��� ȣ���Ų��.
    public delegate void deleAttacked();
    public deleAttacked attacked;

    public System.Action acCloseDoorWhenAlram; // Action�� ���ϰ��� ���°�                    /// �˶��︱�� ���� ���������� �ݰ��ϱ�����
    public System.Action acClockRotate; // �ð�ٴ�ȸ��

    public System.Action acCalling; // ��ȭ�� �︮�°� �޾ƿ�������
    public System.Action acElectroSwitch; //  ����ġ�������°� �޾ƿ�������
    public System.Action acAllStop; //  
    //public System.Action acFireAlram; // ����
    public System.Action acBomb; // ��ź ����
    //public System.Action acInitialize;
    public System.Action acFinalButton;
    public System.Action<eEndDoorEmission> acEndDoorEmission;
    System.Action acFailed;
    System.Action acSuccess;

    //System.Func<int, float, bool> fc; // ���ϰ� �ִ°� �������� ���ϰ� ���� �Ķ����

    private void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
        instance = this;

        keyHpRange[0].keylevel = eKeyLevel.EASY;
        keyHpRange[1].keylevel = eKeyLevel.NOMAL;
        keyHpRange[2].keylevel = eKeyLevel.HARD;

        retryCount = PlayerPrefs.GetInt("TryCount");

        countSound = "�ð��� �帣�� �Ҹ�";

        spawner = GameObject.Find("KeySpawner").transform;

        spawner_Bomb = GameObject.Find("BombSpawner").GetComponent<BombSpawner>();

        remainTimeText = GameObject.Find("RemainTime").GetComponent<TextMesh>();
    }
    private void Start()
    {
        nowPlaying = true;
        StartCoroutine(Timer());
        attacked += Damaged;
        //attacked = null; /// �׽�Ʈ������ ��� �Ͻ�������Ŵ;
        acFailed = UIManager.Instance.MissionFail;
        acSuccess = UIManager.Instance.DefuseSuccess;
    }


    //     public void GameManagerInitialize()
    //     {
    //         nowPlaying = true;
    //         StartCoroutine(Timer());
    //         basketHP = basketMAXHP;
    //         nowLevel = eKeyLevel.EASY;
    //         nowRemainTime = remainTimeMax;
    //         speedMachineSpeed = 1;
    // 
    //         acInitialize += GameManagerInitialize;
    //     }

    IEnumerator Timer()
    {
        int nowTime = 0;
        int pastTime = nowTime;


        while (true && nowPlaying)
        {
            timer += Time.deltaTime;
            nowRemainTime -= Time.deltaTime;
            nowTime = (int)timer;

            remainTimeText.text = nowRemainTime.ToString("F0");

            if (pastTime < nowTime)
            {
                /// 1�� ���� �Ͼ �ϵ�;
                basketHP -= 2;
                pastTime = nowTime;
                acClockRotate();
                int rd = ResetRandomInt();

                SoundMG.Instance.PlaySFX(countSound);

                switch (nowTime)
                {
                    case var value when value >= hardTime:
                        nowLevel = eKeyLevel.HARD;
                        break;
                    case var value when value >= nomalTime:
                        nowLevel = eKeyLevel.NOMAL;
                        break;
                    case var value when value < nomalTime: // �����ڵ�?
                        nowLevel = eKeyLevel.EASY;
                        break;
                }

                switch ((int)nowRemainTime)
                {
                    case var value when value == 0:
                        acEndDoorEmission(eEndDoorEmission.RIGHT);
                        break;
                    case var value when value == 60:
                        acEndDoorEmission(eEndDoorEmission.CENTER);
                        break;
                    case var value when value == 120:
                        acEndDoorEmission(eEndDoorEmission.LEFT);
                        break;
                }

                /// 3�ʸ��� ��÷�� ��;
                if (nowTime % createSecond == 0)
                {
                    switch (nowLevel)
                    {
                        case eKeyLevel.EASY:
                            if (rd < probabilityEasy) ObjectPool.Instance.CreateItem(eKeyLevel.EASY, spawner.GetChild(Random.Range(0, 6)));
                            DrawOccur(eKeyLevel.EASY);
                            break;

                        case eKeyLevel.NOMAL:
                            if (rd < probabilityNomal)
                            {
                                if (rd % 2 == 0) ObjectPool.Instance.CreateItem(eKeyLevel.EASY, spawner.GetChild(Random.Range(0, 6)));
                                else ObjectPool.Instance.CreateItem(eKeyLevel.NOMAL, spawner.GetChild(Random.Range(0, 6)));
                            }
                            DrawOccur(eKeyLevel.NOMAL);
                            break;

                        case eKeyLevel.HARD:
                            {
                                if (rd < probabilityHard)
                                {
                                    switch (rd % 3)
                                    {
                                        case 0:
                                            ObjectPool.Instance.CreateItem(eKeyLevel.EASY, spawner.GetChild(Random.Range(0, 6)));
                                            break;
                                        case 1:
                                            ObjectPool.Instance.CreateItem(eKeyLevel.NOMAL, spawner.GetChild(Random.Range(0, 6)));
                                            break;
                                        case 2:
                                            ObjectPool.Instance.CreateItem(eKeyLevel.HARD, spawner.GetChild(Random.Range(0, 6)));
                                            break;
                                    }
                                }
                                DrawOccur(eKeyLevel.HARD);
                                break;
                                //if (rd < probabilityHard) ObjectPool.Instance.CreateItem(eKeyLevel.HARD, spawner.GetChild(Random.Range(0, 6)));
                            }
                    }
                }
            }

            if (nowRemainTime <= 0) // �� ��������
            {
                nowRemainTime = 0;
                acFinalButton();
            }

            if (basketHP <= 0)  // �׾�����
            {
                nowPlaying = false;
                DyingEffect();
                //UIManager.Instance.MissionFail();
                StartCoroutine(WaitTime(acFailed, 1.5f));
            }
            yield return null;
        }
    }

    void DrawOccur(eKeyLevel _difficulty)
    {
        switch (_difficulty)
        {
            case eKeyLevel.EASY:
                if (ResetRandomInt() < chanceCallingEasy) acCalling();
                if (ResetRandomInt() < chanceSwitchEasy) acElectroSwitch();
                if (ResetRandomInt() < chanceBombEasy) acBomb();
                break;
            case eKeyLevel.NOMAL:
                if (ResetRandomInt() < chanceCallingNomal) acCalling();
                if (ResetRandomInt() < chanceSwitchNomal) acElectroSwitch();
                if (ResetRandomInt() < chanceBombNomal) acBomb();
                break;
            case eKeyLevel.HARD:
                if (ResetRandomInt() < chanceCallingHard) acCalling();
                if (ResetRandomInt() < chanceSwitchHard) acElectroSwitch();
                if (ResetRandomInt() < chanceBombHard) acBomb();
                break;
        }

    }
    public void Damaged() //���� ���ϸ� ȣ��ɰ�
    {
        //Debug.Log("Boom basket HP�� ��´�");
        basketHP -= DamagePoint * 2;
    }
    int ResetRandomInt()
    {
        int rd = (int)(Random.value * 99);
        return rd;
    }
    public void DyingEffect()
    {
        acAllStop();

        lEndGameEffect[0].SetActive(true);

        StartCoroutine(PlayEndEffect());
    }

    IEnumerator PlayEndEffect()
    {
        WaitForSeconds wfs = new WaitForSeconds(endEffectDelayTime);
        yield return wfs;
        lEndGameEffect[1].SetActive(true);
        SoundMG.Instance.PlaySFX(dyingBombSound);
        yield return wfs;
        lEndGameEffect[2].SetActive(true);
        SoundMG.Instance.PlaySFX(dyingBombSound);
        yield return wfs;
        lEndGameEffect[3].SetActive(true);
        SoundMG.Instance.PlaySFX(dyingBombSound2);

        StartCoroutine(SlowDownGame());
    }
    IEnumerator SlowDownGame()
    {
        WaitForSecondsRealtime wfsrt = new WaitForSecondsRealtime(0.2f);

        int count = 0;

        while (count < 10)
        {
            count++;
            yield return wfsrt;
        }

        Time.timeScale = 0;

        SoundMG.Instance.StopAllSound();
    }

    IEnumerator WaitTime(System.Action _ac, float _delayTime)
    {
        WaitForSeconds wfs = new WaitForSeconds(_delayTime);

        //Debug.Log("����ƮŸ�� ����");
        yield return wfs;

        _ac();
    }

    public void BombHeal() // ��ź �뿡 �־ ���Ž����� �� ���ϴ°�
    {
        basketHP += (bombHealPoint * 2);
    }
}


