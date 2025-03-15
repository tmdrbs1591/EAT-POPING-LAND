using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded; // 구독 해제
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "01.Ingame")
        {
            // 현재 방에서 내 플레이어 인덱스 가져오기
            int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;

            // 스폰 포인트 가져오기
            Transform spawnPoint = SpawnPoint.instance.GetSpawnPoint(playerIndex);

            // 포톤 네트워크로 플레이어 생성
            PhotonNetwork.Instantiate("Player", spawnPoint.position, Quaternion.identity);
        }
    }
}
