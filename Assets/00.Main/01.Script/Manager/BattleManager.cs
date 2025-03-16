using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class BattleManager : MonoBehaviourPun
{
    public static BattleManager instance;

    [SerializeField] TMP_Text firstPlayerNameText;  // 내 이름
    [SerializeField] TMP_Text secondPlayerNameText; // 상대 이름
    [SerializeField] GameObject battlePanel;        // 배틀 UI 패널

    private string opponentName = "";

    private void Awake()
    {
        instance = this;
    }

    public void SetBattleInfo(string opponent)
    {
        opponentName = opponent;
    }

    public void BattleStart()
    {
        if (string.IsNullOrEmpty(opponentName))
            return;

        photonView.RPC("RPC_BattleStart", RpcTarget.All, PhotonNetwork.NickName, opponentName);
    }

    [PunRPC]
    private void RPC_BattleStart(string player1, string player2)
    {
        battlePanel.SetActive(true);
        firstPlayerNameText.text = player1;
        secondPlayerNameText.text = player2;
    }
}
