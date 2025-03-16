using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerColorBox : MonoBehaviourPunCallbacks
{
    public PlayerColor playerColorScript;
    public PlayerControl playerControl;

    public GameObject battleUI;
    public TMP_Text battleUIText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PointBox"))
        {
            Renderer renderer = other.GetComponent<Renderer>();
            Hold hold = other.GetComponent<Hold>();

            if (renderer == null)
            {
                renderer = other.GetComponentInChildren<Renderer>();
            }

            if (renderer != null)
            {
                if (hold.holdType == PlayerColorType.Default)
                {
                    renderer.material = playerColorScript.MaterialChange();
                    hold.holdType = playerColorScript.HoldChange();
                }

                if (hold.holdType != playerColorScript.playerColor)
                {
                    if (photonView.IsMine)
                    {
                        string playerName = FindPlayerNameByColor(hold.holdType);
                        battleUI.SetActive(true);
                        battleUIText.text = $"{playerName} 님에게 전투를 신청하시겠습니까?";
                    }
                }
                else
                {
                    if (photonView.IsMine)
                    {
                        TurnManager.instance.EndTurn();
                        playerControl.isMove = false;
                    }
                }
            }
            else
            {
                Debug.LogWarning($"{other.gameObject.name} 오브젝트에 Renderer가 없음!");
            }
        }
    }

    private string FindPlayerNameByColor(PlayerColorType colorType)
    {
        PlayerColor[] allPlayers = FindObjectsOfType<PlayerColor>(); // 모든 PlayerColor 스크립트 찾기

        foreach (var playerColor in allPlayers)
        {
            if (playerColor.playerColor == colorType) // 색이 같은 플레이어 찾기
            {
                return playerColor.GetComponent<PhotonView>().Owner.NickName; // PhotonView에서 닉네임 가져오기
            }
        }
        return "알 수 없음";
    }

}
