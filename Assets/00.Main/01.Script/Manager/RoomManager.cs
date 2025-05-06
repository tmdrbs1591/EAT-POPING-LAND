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
        SceneManager.sceneLoaded -= OnSceneLoaded; // ���� ����
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "01.Ingame")
        {

            //   PhotonNetwork.SendRate = 100; // �⺻���� 10
            //  PhotonNetwork.SerializationRate = 100; // �⺻���� 10

            // RoomManager.cs �� StartGame ����, �÷��̾� ���� ������ ����
            Player[] players = PhotonNetwork.PlayerList; // �̰� �ڵ� ���ĵ� (���� ����)

            // ���� �ε����� ���� ������ ���
            int playerIndex = System.Array.IndexOf(players, PhotonNetwork.LocalPlayer);

            // ���� ����Ʈ ��������
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
