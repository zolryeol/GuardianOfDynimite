using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 풀이라고했지만 전체적으로 나오는 개수가 많지 않기에 1개 이상필요할시에는 생성 및 제거시키기로 한다.
/// </summary>

public class ObjectPool : MonoBehaviour
{
    private static ObjectPool instance = null;

    public static ObjectPool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ObjectPool>();
                if (instance == null) instance = new ObjectPool();
            }
            return instance;
        }
    }

    GameObject[][] dArrPrefab = new GameObject[4][];
    GameObject[][] dArrInstance = new GameObject[4][];

    Dictionary<eKeyLevel, GameObject[]> dKeyPool = new Dictionary<eKeyLevel, GameObject[]>();

    public Transform poolParent;

    [SerializeField] GameObject keySpawnEffect;
    [Header("사운드 파일명")]
    [SerializeField] public string keySpawnSound;
    [SerializeField] public string keySpawnSound2;
    [SerializeField] public string bombSpawnSound;



    void LoadPrefab()
    {
        dArrPrefab[(int)eKeyLevel.EASY] = Resources.LoadAll<GameObject>("Key_Resources/Prefabs/A.Easy Key");
        dArrPrefab[(int)eKeyLevel.NOMAL] = Resources.LoadAll<GameObject>("Key_Resources/Prefabs/B.Normal Key");
        dArrPrefab[(int)eKeyLevel.HARD] = Resources.LoadAll<GameObject>("Key_Resources/Prefabs/C.Hard Key");

        // 폭탄은 브루탈하게 3개 정도만 쓰겠다;
        dArrPrefab[(int)eKeyLevel.BOMB] = new GameObject[3];
        dArrPrefab[(int)eKeyLevel.BOMB][0] = Resources.Load<GameObject>("Bomb");
        dArrPrefab[(int)eKeyLevel.BOMB][1] = dArrPrefab[(int)eKeyLevel.BOMB][0];
        dArrPrefab[(int)eKeyLevel.BOMB][2] = dArrPrefab[(int)eKeyLevel.BOMB][0];

    }

    void InstanceAddDictionary()
    {
        for (int i = 0; i < dArrPrefab.Length; ++i)
        {
            dArrInstance[i] = new GameObject[dArrPrefab[i].Length];

            for (int j = 0; j < dArrPrefab[i].Length; ++j)
            {
                var item = Instantiate<GameObject>(dArrPrefab[i][j], poolParent);
                dArrInstance[i][j] = item;
                //item.SetActive(false);
            }
            dKeyPool.Add((eKeyLevel)i, dArrInstance[i]);
        }
    }

    private void Awake()
    {
        poolParent = new GameObject("PoolParent").transform;
        poolParent.position = new Vector3(2, 200, 200);

        LoadPrefab();

        //InstanceAddDictionary();
    }

    public GameObject CreateItem(eKeyLevel _KeyLevel, Transform _Pos)
    {
        var item = Instantiate(dArrPrefab[(int)_KeyLevel][Random.Range(0, dArrPrefab[(int)_KeyLevel].Length)], _Pos.position, Quaternion.identity);
        Instantiate(keySpawnEffect, item.transform);
        /*var item = dKeyPool[_KeyLevel].GetValue(Random.Range(0, dArrInstance[(int)_KeyLevel].Length)) as GameObject;*/
        if (_KeyLevel != eKeyLevel.BOMB) // Bomb는 따로 사운드 처리해준다
        {
            SoundMG.Instance.PlaySFX(keySpawnSound);
            SoundMG.Instance.PlaySFX(keySpawnSound2);
        }
        else { SoundMG.Instance.PlaySFX(bombSpawnSound); }

        return item;
    }
    public GameObject PopItem(eKeyLevel _KeyLevel, Transform _Pos)
    {
        var item = dKeyPool[_KeyLevel].GetValue(Random.Range(0, dArrInstance[(int)_KeyLevel].Length)) as GameObject;

        ///중복안되도록 하는 기능
        //         if(item.activeSelf) //
        //         {
        //             var anotherItem = PopItem(_KeyLevel, _Pos);
        //             return anotherItem;
        //         }

        /// 이미 사용중일때 새로만들어주는 기능
        if (item.activeSelf)
        {
            var anotherItem = Instantiate(item);
            anotherItem.transform.position = _Pos.position;
            return anotherItem;
        }

        item.transform.position = _Pos.position;
        item.SetActive(true);

        return item;
    }
}