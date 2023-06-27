using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임의 전반을 관리하는 게임매니저
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
    public float keyMoveSpeed; // key 날라가는 스피드
    public float keyRotaSpeed; // key 회전속도
    public float bombMoveSpeed;

    [Space(30f)]
    public int basketMAXHP = 120;
    public int basketHP = 120;
    [Header("버튼깜빡이는시간")]
    public float pushFlikerSpeed = 0.3f;
    float timer;
    [Header("주어진시간")]
    public float nowRemainTime = 180;
    float remainTimeMax = 180;

    public float rankingScore;
    public float GetTime { get => timer; }
    public bool nowPlaying = false;
    [Header("키 체력 범위")]
    public sKeyHpRange[] keyHpRange = new sKeyHpRange[3];
    [Header("폭탄 회복량")]
    public int bombHealPoint = 1;
    [Header("키 회복량")]
    public int healPoint = 1;
    [Header("키 데미지")]
    public int DamagePoint = 2;
    [Header("스피드머신 속도")]
    public float speedMachineSpeed = 2;

    [Header("폭탄 타이머")]
    public int BombTimer = 5;
    [Header("폭탄 데미지")]
    public int bombDamage = 30;

    [Header("현재 난이도")]
    [Space(30f)]
    [SerializeField] eKeyLevel nowLevel;
    [Header("노말로 바뀌는 시간")]
    [SerializeField] int nomalTime;
    [Header("하드로 바뀌는 시간")]
    [SerializeField] int hardTime;

    [Header("N초마다 생성")]
    [Space(30f)]
    [SerializeField] int createSecond;

    [Header("Easy 생성확률")]
    [Space(30f)]
    [SerializeField] int probabilityEasy = 35;
    [Header("Nomal 생성확률")]
    [SerializeField] int probabilityNomal = 50;
    [Header("Hard 생성확률")]
    [SerializeField] int probabilityHard = 70;

    [Header("Bomb 생성확률")]
    [Space(30f)]
    [SerializeField] int chanceBombEasy = 0;
    [SerializeField] int chanceBombNomal = 25;
    [SerializeField] int chanceBombHard = 35;

    [Header("전화기 울릴확률")]
    [Space(30f)]
    [SerializeField] int chanceCallingEasy = 25;
    [SerializeField] int chanceCallingNomal = 45;
    [SerializeField] int chanceCallingHard = 65;

    [Header("스위치 내려갈 확률")]
    [SerializeField] int chanceSwitchEasy = 25;
    [SerializeField] int chanceSwitchNomal = 45;
    [SerializeField] int chanceSwitchHard = 65;

    [SerializeField] List<GameObject> lEndGameEffect;
    [SerializeField] float endEffectDelayTime = 0.5f;
    [SerializeField] TextMesh remainTimeText;
    public int retryCount;

    [Header("사운드 파일명")]
    [SerializeField] string dyingBombSound;

    [SerializeField] string dyingBombSound2;

    [SerializeField] string countSound;

    // 벨울리기, 점등레버, 쓰레기통닫기, 사이렌램프, 체력깍기, 통 시계바늘 회전 등을 넣어서 한번에 호출시킨다.
    public delegate void deleAttacked();
    public deleAttacked attacked;

    public System.Action acCloseDoorWhenAlram; // Action은 리턴값이 없는것                    /// 알람울릴때 문이 열려있을시 닫게하기위해
    public System.Action acClockRotate; // 시계바늘회전

    public System.Action acCalling; // 전화기 울리는것 받아오기위해
    public System.Action acElectroSwitch; //  스위치내려가는것 받아오기위해
    public System.Action acAllStop; //  
    //public System.Action acFireAlram; // 보류
    public System.Action acBomb; // 폭탄 생성
    //public System.Action acInitialize;
    public System.Action acFinalButton;
    public System.Action<eEndDoorEmission> acEndDoorEmission;
    System.Action acFailed;
    System.Action acSuccess;

    //System.Func<int, float, bool> fc; // 리턴값 있는것 마지막이 리턴값 앞은 파라미터

    private void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
        instance = this;

        keyHpRange[0].keylevel = eKeyLevel.EASY;
        keyHpRange[1].keylevel = eKeyLevel.NOMAL;
        keyHpRange[2].keylevel = eKeyLevel.HARD;

        retryCount = PlayerPrefs.GetInt("TryCount");

        countSound = "시간초 흐르는 소리";

        spawner = GameObject.Find("KeySpawner").transform;

        spawner_Bomb = GameObject.Find("BombSpawner").GetComponent<BombSpawner>();

        remainTimeText = GameObject.Find("RemainTime").GetComponent<TextMesh>();
    }
    private void Start()
    {
        nowPlaying = true;
        StartCoroutine(Timer());
        attacked += Damaged;
        //attacked = null; /// 테스트용으로 잠시 일시정지시킴;
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
                /// 1초 마다 일어날 일들;
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
                    case var value when value < nomalTime: // 안전코드?
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

                /// 3초마다 추첨할 일;
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

            if (nowRemainTime <= 0) // 다 버텼을때
            {
                nowRemainTime = 0;
                acFinalButton();
            }

            if (basketHP <= 0)  // 죽었을때
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
    public void Damaged() //공격 당하면 호출될것
    {
        //Debug.Log("Boom basket HP를 깍는다");
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

        //Debug.Log("웨이트타임 들어옴");
        yield return wfs;

        _ac();
    }

    public void BombHeal() // 폭탄 통에 넣어서 제거시켰을 때 힐하는것
    {
        basketHP += (bombHealPoint * 2);
    }
}


