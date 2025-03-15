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
            int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            Transform spawnPoint = SpawnPoint.instance.GetSpawnPoint(playerIndex);

            var player = PhotonNetwork.Instantiate("Player", spawnPoint.position, Quaternion.identity);
            var playerColorScript = player.GetComponent<PlayerColor>();

            PlayerColorType color = (PlayerColorType)playerIndex;

            if (playerColorScript.photonView.IsMine)
            {
                playerColorScript.playerColor = color;
                playerColorScript.photonView.RPC("SetColor", RpcTarget.AllBuffered, (int)color);
            }
        }
    }

}
