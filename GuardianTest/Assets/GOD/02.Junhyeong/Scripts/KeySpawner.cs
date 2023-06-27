using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 타이머를 받아와서 일정시간마다 키 소환한다.
/// </summary>

public class KeySpawner : MonoBehaviour
{
    Transform[] keySpawnPos = new Transform[6];
    void Start()
    {
        for (int i = 0; i < keySpawnPos.Length; ++i)
        {
            keySpawnPos[i] = transform.GetChild(i).transform;
        }
    }

    // Update is called once per frame
//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.Keypad1))
//         {
//             ObjectPool.Instance.PopItem(eKeyLevel.EASY, keySpawnPos[Random.Range(0, 2)]);
//         }
// 
//         if (Input.GetKeyDown(KeyCode.Keypad2))
//         {
//             ObjectPool.Instance.PopItem(eKeyLevel.NOMAL, keySpawnPos[Random.Range(0, 4)]);
//         }
// 
//         if (Input.GetKeyDown(KeyCode.Keypad3))
//         {
//             ObjectPool.Instance.PopItem(eKeyLevel.HARD, keySpawnPos[Random.Range(0, 6)]);
//         }
//     }
}
