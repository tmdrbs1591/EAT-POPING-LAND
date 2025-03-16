using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;

public class PlayerMoney : MonoBehaviourPunCallbacks
{
    public GameObject[] uiPositions; // 4개의 UI 오브젝트 (우상단, 좌상단, 우하단, 좌하단)
    public TextMeshProUGUI[] moneyTexts; // 4개의 돈 표시 UI (각 UI 내부)

    private int money = 100; // 플레이어 초기 돈
    private int uiIndex; // 이 플레이어가 차지할 UI 위치

    void Start()
    {
        // 모든 UI를 꺼둠 (현재 접속한 플레이어만 보이도록)
        foreach (var ui in uiPositions)
        {
            ui.SetActive(false);
        }

        // 내 UI 위치 설정
        uiIndex = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % uiPositions.Length;
        uiPositions[uiIndex].SetActive(true);

        UpdateMoney(0); // 돈 초기화
        UpdateAllUI();  // UI 업데이트
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            // I 키를 누르면 돈 증가
            if (Input.GetKeyDown(KeyCode.I))
            {
                AddMoney(10);
            }
        }
    }

    public void AddMoney(int amount)
    {
        if (photonView.IsMine)
        {
            UpdateMoney(amount);
        }
    }

    private void UpdateMoney(int amount)
    {
        money += amount;

        Hashtable hash = new Hashtable();
        hash["Money"] = money;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("Money"))
        {
            int updatedMoney = (int)changedProps["Money"];
            int targetIndex = (targetPlayer.ActorNumber - 1) % moneyTexts.Length;

            moneyTexts[targetIndex].text = $"P{targetPlayer.ActorNumber}: {updatedMoney}G";
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateAllUI(); // 플레이어가 들어오면 UI 업데이트
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateAllUI(); // 플레이어가 나가면 UI 업데이트
    }

    private void UpdateAllUI()
    {
        // 모든 UI를 꺼둠
        foreach (var ui in uiPositions)
        {
            ui.SetActive(false);
        }

        // 현재 접속한 플레이어만 UI를 활성화
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            int index = (player.ActorNumber - 1) % uiPositions.Length;
            uiPositions[index].SetActive(true);
        }
    }
}
