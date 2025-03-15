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
        SceneManager.sceneLoaded -= OnSceneLoaded; // ���� ����
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "01.Ingame")
        {
            // ���� �濡�� �� �÷��̾� �ε��� ��������
            int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;

            // ���� ����Ʈ ��������
            Transform spawnPoint = SpawnPoint.instance.GetSpawnPoint(playerIndex);

            // ���� ��Ʈ��ũ�� �÷��̾� ����
            PhotonNetwork.Instantiate("Player", spawnPoint.position, Quaternion.identity);
        }
    }
}
