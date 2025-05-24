using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PoisonZoneManager : MonoBehaviourPunCallbacks
{
    public static PoisonZoneManager instance;
    public GameObject poisonCloudPrefab;

    [Header("꼭짓점 4개 Transform")]
    public Transform topLeft;
    public Transform topRight;
    public Transform bottomLeft;
    public Transform bottomRight;

    public int maxLayers = 10;
    public float interval = 5f;
    public int stepsPerSide = 10;

    private int currentLayer = 0;
    private Coroutine spawnCoroutine;

    
    private List<GameObject> spawnedClouds = new List<GameObject>();


    private void Awake()
    {
        instance = this;    
    }
    private void Update()
    {
   

        if (Input.GetKeyDown(KeyCode.Y))
        {
            photonView.RPC("RPC_StartPoison", RpcTarget.AllBuffered);
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            photonView.RPC("RPC_StopPoison", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
   public void RPC_StartPoison()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (spawnCoroutine == null)
            spawnCoroutine = StartCoroutine(SpawnPoisonRoutine());
    }

    [PunRPC]
    public void RPC_StopPoison()
    {
        if (!PhotonNetwork.IsMasterClient) return;
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

        Vector3 newTopLeft = Vector3.Lerp(topLeft.position, bottomRight.position, t);
        Vector3 newTopRight = Vector3.Lerp(topRight.position, bottomLeft.position, t);
        Vector3 newBottomLeft = Vector3.Lerp(bottomLeft.position, topRight.position, t);
        Vector3 newBottomRight = Vector3.Lerp(bottomRight.position, topLeft.position, t);

        // steps를 점점 줄이기 (ex: 10, 9, 8, ..., 1)
        int currentSteps = Mathf.Max(1, Mathf.FloorToInt(stepsPerSide - layer * 1.5f));

        SpawnLine(newTopLeft, newTopRight, currentSteps);
        SpawnLine(newTopRight, newBottomRight, currentSteps);
        SpawnLine(newBottomRight, newBottomLeft, currentSteps);
        SpawnLine(newBottomLeft, newTopLeft, currentSteps);
    }

    void SpawnLine(Vector3 start, Vector3 end, int steps)
    {
        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
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
