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
        // �÷��̾� �ε����� �´� ���� ����Ʈ ��ȯ
        return spawnPoints[playerIndex % spawnPoints.Length];
    }
}
