using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerColorBox : MonoBehaviourPunCallbacks
{
    public PlayerColor playerColorScript;
    public PlayerControl playerControl;
    public PlayerMoney playerMoneyScript;

    public GameObject battleUI;
    public GameObject moneyEffect;
    public TMP_Text battleUIText;
    public TMP_Text priceText;
    [SerializeField] private float rayLength = 3f; // 인스펙터에서 조절 가능하게



    public void CheckPointBelow()
    {
        Vector3 center = transform.position + Vector3.down * (rayLength / 2f);
        Vector3 halfExtents = new Vector3(0.5f, rayLength / 2f, 0.5f); // 너비, 높이 반절
        Collider[] hits = Physics.OverlapBox(center, halfExtents);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("PointBox")) continue;

            Hold hold = hit.GetComponent<Hold>();
            if (hold == null) continue;

            PhotonView holdView = hold.GetComponent<PhotonView>();

            switch (hold.holdType)
            {
                case ColorType.Key:
                    KeyCardManager.instance.KeyCardPanelOpen();
                    playerControl.isMove = false;
                    return;

                case ColorType.Prison:
                    Debug.Log("Prison");
                    TurnManager.instance.isPrison = true;
                    TurnManager.instance.prisonStartUI.SetActive(false);
                    TurnManager.instance.prisonStartUI.SetActive(true);
                    EndTurn();
                    return;

                case ColorType.Shop:
                    Debug.Log("Shop");
                    WeaponManager.instance.ShopPanelOpen();
                    return;

                case ColorType.Money:
                    Debug.Log("Money");
                    if (photonView.IsMine)
                    {
                        playerMoneyScript.AddMoney(300);
                        string playerNickName = photonView.Owner.NickName;
                        SystemMessaageManager.instance.MessageTextStart($"{playerNickName}님이 300캔디코인을 획득했습니다!");
                    }
                    Instantiate(moneyEffect, transform.position, Quaternion.identity);
                    photonView.RPC("AudioRPC", RpcTarget.AllBuffered, 8);
                    EndTurn();
                    return;

                case ColorType.Default:
                    if (holdView != null)
                    {
                        int materialIndex = playerColorScript.GetMaterialIndex();
                        int holdTypeInt = (int)playerColorScript.HoldChange();
                        holdView.RPC("HoldColorChange", RpcTarget.AllBuffered, materialIndex, holdTypeInt);
                        holdView.RPC("HoldPriceUp", RpcTarget.AllBuffered, 100, PhotonNetwork.NickName);
                    }
                    EndTurn();
                    return;

                default:
                    // 내 땅일 때
                    if (hold.holdType == playerColorScript.playerColor)
                    {
                        if (holdView != null)
                        {
                            holdView.RPC("HoldPriceUp", RpcTarget.AllBuffered, 100, PhotonNetwork.NickName);

                        }
                        EndTurn();
                        return;
                    }

                    // 다른 사람 땅일 때
                    if (hold.holdType != playerColorScript.playerColor)
                    {
                        if (playerControl.isWin)
                        {
                            if (holdView != null)
                            {
                                int materialIndex = playerColorScript.GetMaterialIndex();
                                int holdTypeInt = (int)playerColorScript.HoldChange();
                                holdView.RPC("HoldColorChange", RpcTarget.AllBuffered, materialIndex, holdTypeInt);
                                holdView.RPC("HoldPriceUp", RpcTarget.AllBuffered, 100, photonView.Owner.NickName);
                            }
                        }
                        else
                        {
                            if (photonView.IsMine)
                            {
                                string playerName = FindPlayerNameByColor(hold.holdType);
                                Debug.Log($"[PlayerColorBox] 전투 상대: {playerName}");
                                BattleManager.instance.SetBattleInfo(playerName);
                                battleUI.SetActive(true);
                                battleUIText.text = $"{playerName} ({hold.holdType})님에게 전투를 신청하시겠습니까?";
                                priceText.text = "땅값 : " + FormatKoreanCurrency(hold.holdPrice);
                                BattleManager.instance.holdPrice = hold.holdPrice;
                            }
                            // 여기선 EndTurn() 호출 안 함 (전투 신청해야 하니까)
                        }
                        return;
                    }

                    break;
            }
        }


        Debug.Log("아래에 PointBox 없음");
    }
    private string FormatKoreanCurrency(int money)
    {
        if (money < 10000)
        {
            return money.ToString("N0") + "원";
        }

        int man = money / 10000;
        int rest = money % 10000;

        if (rest == 0)
        {
            return $"{man}만원";
        }
        else
        {
            return $"{man}만{rest.ToString("N0")}원";
        }
    }



    public void EndTurn()
    {
        if (photonView.IsMine)
        {
            TurnManager.instance.EndTurn();
            playerControl.isMove = false;
        }
    }
    private string FindPlayerNameByColor(ColorType colorType)
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

    [PunRPC]
    public void AudioRPC(int index)
    {
        AudioManager.instance.PlaySound(transform.position, index, Random.Range(1f, 1f), 1f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position + Vector3.down * (rayLength / 2f);
        Vector3 size = new Vector3(1f, rayLength, 1f);
        Gizmos.DrawWireCube(center, size);
    }

}
