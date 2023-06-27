using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  동그란 폭탄 튀어나오는 스포너. 3방향이다
/// </summary>
public class BombSpawner : MonoBehaviour
{
    [SerializeField] Transform[] spawner;
    [SerializeField] Transform targetPos;

    private void Start()
    {
        GameManager.Instance.acBomb += SpawnBomb;
    }

    public void SpawnBomb()
    {
        var bomb = ObjectPool.Instance.CreateItem(eKeyLevel.BOMB, spawner[Random.Range(0, transform.childCount - 1)]);

        Vector3 targetDir = targetPos.position - bomb.transform.position;

        bomb.GetComponent<Rigidbody>().AddForce(targetDir * GameManager.Instance.bombMoveSpeed, ForceMode.Acceleration);
    }
}
