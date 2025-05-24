using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class PoisonZoneManager : MonoBehaviour
{
    public GameObject poisonCloudPrefab;

    [Header("������ 4�� Transform")]
    public Transform topLeft;
    public Transform topRight;
    public Transform bottomLeft;
    public Transform bottomRight;

    public int maxLayers = 10; // �� �� ��
    public float interval = 5f;

    private int currentLayer = 0;
    private Coroutine spawnCoroutine;

    private List<GameObject> spawnedClouds = new List<GameObject>();

    // �Ա��� ���� (�� ���� �� �� ��������)
    public int stepsPerSide = 10;

    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            StartPoison();
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            StopPoison();
        }
    }
    public void StartPoison()
    {
        if (spawnCoroutine == null)
            spawnCoroutine = StartCoroutine(SpawnPoisonRoutine());
    }

    public void StopPoison()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        foreach (var cloud in spawnedClouds)
            if (cloud != null)
                ObjectPool.ReturnToPool("Poison", cloud);

        spawnedClouds.Clear();
        currentLayer = 0;
    }

    private IEnumerator SpawnPoisonRoutine()
    {
        while (currentLayer < maxLayers)
        {
            SpawnPoisonLayer(currentLayer);
            currentLayer++;
            yield return new WaitForSeconds(interval);
        }
    }

    void SpawnPoisonLayer(int layer)
    {
        if (layer >= maxLayers)
            return;

        float t = layer / (float)maxLayers;

        // �밢�� �������� �����ؼ� ���� ������ 4�� ���
        Vector3 newTopLeft = Vector3.Lerp(topLeft.position, bottomRight.position, t);
        Vector3 newTopRight = Vector3.Lerp(topRight.position, bottomLeft.position, t);
        Vector3 newBottomLeft = Vector3.Lerp(bottomLeft.position, topRight.position, t);
        Vector3 newBottomRight = Vector3.Lerp(bottomRight.position, topLeft.position, t);

        // �� �� ���� �Ա��� ����
        SpawnLine(newTopLeft, newTopRight);
        SpawnLine(newTopRight, newBottomRight);
        SpawnLine(newBottomRight, newBottomLeft);
        SpawnLine(newBottomLeft, newTopLeft);
    }

    void SpawnLine(Vector3 start, Vector3 end)
    {
        for (int i = 0; i <= stepsPerSide; i++)
        {
            float t = i / (float)stepsPerSide;
            Vector3 pos = Vector3.Lerp(start, end, t);
            SpawnPoisonAt(pos);
        }
    }

    void SpawnPoisonAt(Vector3 pos)
    {
        Vector3 spawnPos = pos + Vector3.up * 0.5f;
        GameObject cloud = ObjectPool.SpawnFromPool("Poison", spawnPos, Quaternion.identity);
        spawnedClouds.Add(cloud);
    }
}
