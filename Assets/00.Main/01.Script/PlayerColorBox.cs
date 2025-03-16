using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;

public class PlayerColorBox : MonoBehaviourPunCallbacks
{
    public PlayerColor playerColor; // 내 플레이어 색상 정보
    public GameObject battleRequestPanel; // 대결 신청 패널
    public TextMeshProUGUI battleRequestText; // 대결 신청 UI


    private void Start()
    {
        // 📌 내 플레이어 색상을 Photon CustomProperties에 저장
        Hashtable hash = new Hashtable();
        hash["PlayerColor"] = playerColor.playerColor.ToString(); // 🔹 문자열로 저장
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

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

            if (renderer != null && hold != null)
            {
                string currentHoldType = hold.holdType.ToString(); // 📌 현재 땅의 HoldType (문자열)
                string myColorType = playerColor.playerColor.ToString(); // 📌 내 PlayerColorType (문자열)

                // 📌 1️⃣ 내 타입과 땅 타입이 같으면 아무것도 안 함
                if (currentHoldType == myColorType)
                {
                    Debug.Log($"[내 땅] {currentHoldType} 땅입니다.");
                    return;
                }

                // 📌 2️⃣ 디폴트 땅이면 그냥 색 변경
                if (currentHoldType == HoldType.Default.ToString())
                {
                    Debug.Log("[디폴트 땅] 색 변경!");
                    renderer.material = playerColor.MaterialChange();
                    hold.holdType = playerColor.HoldChange(); // 내 타입으로 변경
                }
                else
                {
                    if (!photonView.IsMine) return; // 📌 내 플레이어만 처리

                    // 📌 3️⃣ 상대 플레이어 이름 찾기 (HoldType 기반)
                    string opponentName = FindPlayerByHoldType(currentHoldType);

                    Debug.Log($"[전투 신청] {opponentName} ({currentHoldType})의 땅에 도전!");

                    // 📌 4️⃣ UI에 상대 플레이어 이름 표시
                    ShowBattleRequestUI(opponentName, currentHoldType);
                }
            }
            else
            {
                Debug.LogWarning($"⚠️ {other.gameObject.name}: Renderer 또는 Hold가 없음!");
            }
        }
    }

    // 📌 상대 플레이어 찾기 (HoldType 기반으로 PlayerColor 매칭)
    private string FindPlayerByHoldType(string targetHoldType)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("PlayerColor") &&
                player.CustomProperties["PlayerColor"].ToString() == targetHoldType)
            {
                return player.NickName; // 📌 해당 HoldType을 가진 플레이어 이름 반환
            }
        }
        return "알 수 없음"; // 📌 해당 타입을 가진 플레이어가 없을 경우 기본값
    }

    // 📌 5️⃣ 대결 신청 UI 표시
    private void ShowBattleRequestUI(string opponentName, string opponentColor)
    {
        battleRequestPanel.SetActive(true);
        battleRequestText.text = $"\"{opponentName}\" 님의 땅 ({opponentColor})에 도전하시겠습니까?";
    }

}
