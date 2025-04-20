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
    [SerializeField] private float rayLength = 3f; // 인스펙터에서 조절 가능하게



    public void CheckPointBelow()
    {
        Vector3 center = transform.position + Vector3.down * (rayLength / 2f);
        Vector3 halfExtents = new Vector3(0.5f, rayLength / 2f, 0.5f); // 너비, 높이 반절
        Collider[] hits = Physics.OverlapBox(center, halfExtents);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("PointBox"))
            {
                Hold hold = hit.GetComponent<Hold>();
                if (hold != null)
                {
                    if (hold.holdType == PlayerColorType.Default)
                    {
                        int materialIndex = playerColorScript.GetMaterialIndex(); // 미리 지정된 인덱스
                        int holdTypeInt = (int)playerColorScript.HoldChange();

                        PhotonView holdView = hold.GetComponent<PhotonView>();
                        if (holdView != null)
                        {
                            holdView.RPC("HoldColorChange", RpcTarget.AllBuffered, materialIndex, holdTypeInt);
                        }

                    }
                    if (hold.holdType != playerColorScript.playerColor && !playerControl.isWin)
                    {
                        if (photonView.IsMine)
                        {
                            string playerName = FindPlayerNameByColor(hold.holdType);
                            Debug.Log($"[PlayerColorBox] 전투 상대: {playerName}");
                            BattleManager.instance.SetBattleInfo(playerName);
                            battleUI.SetActive(true);
                            battleUIText.text = $"{playerName} ({hold.holdType})님에게 전투를 신청하시겠습니까?";
                        }
                    }
                    else if (hold.holdType != playerColorScript.playerColor && playerControl.isWin)
                    {
                        int materialIndex = playerColorScript.GetMaterialIndex(); // 미리 지정된 인덱스
                        int holdTypeInt = (int)playerColorScript.HoldChange();

                        PhotonView holdView = hold.GetComponent<PhotonView>();
                        if (holdView != null)
                        {
                            holdView.RPC("HoldColorChange", RpcTarget.AllBuffered, materialIndex, holdTypeInt);
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

                    return; // 첫 번째 맞은 거 처리하고 종료
                }
            }
        }

        Debug.Log("아래에 PointBox 없음");
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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position + Vector3.down * (rayLength / 2f);
        Vector3 size = new Vector3(1f, rayLength, 1f);
        Gizmos.DrawWireCube(center, size);
    }

}
