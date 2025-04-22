using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;

public class PlayerMoney : MonoBehaviourPunCallbacks
{
    public GameObject[] uiPositions; // 4개의 UI 오브젝트 (우상단, 좌상단, 우하단, 좌하단)
    public TextMeshProUGUI[] moneyTexts; // 4개의 돈 표시 UI (각 UI 내부)
    public TextMeshProUGUI[] nameTexts; // 4개의 플레이어 이름 UI

    public int money = 100; // 플레이어 초기 돈
    public int uiIndex; // 이 플레이어가 차지할 UI 위치

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

        // 닉네임 설정
        Hashtable hash = new Hashtable();
        hash["PlayerName"] = PhotonNetwork.LocalPlayer.NickName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        UpdateMoney(0); // 돈 초기화
        UpdateAllUI();  // UI 업데이트
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.I)) AddMoney(10);
        }
    }

    public void AddMoney(int amount)
    {
            UpdateMoney(amount);
    }

    private void UpdateMoney(int amount)
    {
        photonView.RPC("UpdateMoneyRPC", RpcTarget.AllBuffered, amount);
           
    }
    [PunRPC]
    private void UpdateMoneyRPC(int amount)
    {
        money += amount;

        // RPC 내부에서 바로 CustomProperties 업데이트
        if (photonView.IsMine) // 내 클라이언트에서만 SetCustomProperties
        {
            Hashtable hash = new Hashtable();
            hash["Money"] = money;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        int targetIndex = (targetPlayer.ActorNumber - 1) % moneyTexts.Length;

        if (changedProps.ContainsKey("Money"))
        {
            int updatedMoney = (int)changedProps["Money"];
            moneyTexts[targetIndex].text = $"P{targetPlayer.ActorNumber}: {updatedMoney}G";
        }

        if (changedProps.ContainsKey("PlayerName"))
        {
            string playerName = (string)changedProps["PlayerName"];
            nameTexts[targetIndex].text = playerName;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateAllUI();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateAllUI();
    }

    private void UpdateAllUI()
    {
        foreach (var ui in uiPositions)
        {
            ui.SetActive(false);
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            int index = (player.ActorNumber - 1) % uiPositions.Length;
            uiPositions[index].SetActive(true);

            // 닉네임 UI 반영
            if (player.CustomProperties.ContainsKey("PlayerName"))
            {
                nameTexts[index].text = (string)player.CustomProperties["PlayerName"];
            }

            // 돈 UI 반영
            if (player.CustomProperties.ContainsKey("Money"))
            {
                int playerMoney = (int)player.CustomProperties["Money"];
                moneyTexts[index].text = $"P{player.ActorNumber}: {playerMoney}G";
            }
        }
    }

 

}
