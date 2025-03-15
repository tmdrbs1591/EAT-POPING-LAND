using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public static SpawnPoint instance;

    public Transform[] spawnPoints;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Transform GetSpawnPoint(int playerIndex)
    {
        // 플레이어 인덱스에 맞는 스폰 포인트 반환
        return spawnPoints[playerIndex % spawnPoints.Length];
    }
}
