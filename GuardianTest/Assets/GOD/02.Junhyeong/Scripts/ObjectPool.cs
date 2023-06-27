using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ǯ�̶�������� ��ü������ ������ ������ ���� �ʱ⿡ 1�� �̻��ʿ��ҽÿ��� ���� �� ���Ž�Ű��� �Ѵ�.
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
    [Header("���� ���ϸ�")]
    [SerializeField] public string keySpawnSound;
    [SerializeField] public string keySpawnSound2;
    [SerializeField] public string bombSpawnSound;



    void LoadPrefab()
    {
        dArrPrefab[(int)eKeyLevel.EASY] = Resources.LoadAll<GameObject>("Key_Resources/Prefabs/A.Easy Key");
        dArrPrefab[(int)eKeyLevel.NOMAL] = Resources.LoadAll<GameObject>("Key_Resources/Prefabs/B.Normal Key");
        dArrPrefab[(int)eKeyLevel.HARD] = Resources.LoadAll<GameObject>("Key_Resources/Prefabs/C.Hard Key");

        // ��ź�� ���Ż�ϰ� 3�� ������ ���ڴ�;
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
        if (_KeyLevel != eKeyLevel.BOMB) // Bomb�� ���� ���� ó�����ش�
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

        ///�ߺ��ȵǵ��� �ϴ� ���
        //         if(item.activeSelf) //
        //         {
        //             var anotherItem = PopItem(_KeyLevel, _Pos);
        //             return anotherItem;
        //         }

        /// �̹� ������϶� ���θ�����ִ� ���
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