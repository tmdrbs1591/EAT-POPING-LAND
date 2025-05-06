using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.IO;
using Photon.Realtime;

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

            //   PhotonNetwork.SendRate = 100; // 기본값은 10
            //  PhotonNetwork.SerializationRate = 100; // 기본값은 10

            // RoomManager.cs 안 StartGame 이후, 플레이어 스폰 시점에 실행
            Player[] players = PhotonNetwork.PlayerList; // 이건 자동 정렬됨 (입장 순서)

            // 나의 인덱스를 입장 순서로 계산
            int playerIndex = System.Array.IndexOf(players, PhotonNetwork.LocalPlayer);

            // 스폰 포인트 가져오기
            Transform spawnPoint = SpawnPoint.instance.GetSpawnPoint(playerIndex);

            var player = PhotonNetwork.Instantiate(CharacterManager.instance.characterType.ToString()+"Player", spawnPoint.position, Quaternion.identity);
            var playerColorScript = player.GetComponent<PlayerColor>();

            ColorType color = (ColorType)playerIndex;

            if (playerColorScript.photonView.IsMine)
            {
                playerColorScript.playerColor = color;
                playerColorScript.photonView.RPC("SetColor", RpcTarget.AllBuffered, (int)color);
            }
        }
    }

}
