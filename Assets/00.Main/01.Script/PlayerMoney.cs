using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;

public class PlayerMoney : MonoBehaviourPunCallbacks
{
    public GameObject[] uiPositions; // 4���� UI ������Ʈ (����, �»��, ���ϴ�, ���ϴ�)
    public TextMeshProUGUI[] moneyTexts; // 4���� �� ǥ�� UI (�� UI ����)
    public TextMeshProUGUI[] nameTexts; // 4���� �÷��̾� �̸� UI

    private int money = 100; // �÷��̾� �ʱ� ��
    private int uiIndex; // �� �÷��̾ ������ UI ��ġ

    void Start()
    {
        // ��� UI�� ���� (���� ������ �÷��̾ ���̵���)
        foreach (var ui in uiPositions)
        {
            ui.SetActive(false);
        }

        // �� UI ��ġ ����
        uiIndex = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % uiPositions.Length;
        uiPositions[uiIndex].SetActive(true);

        // �г��� ����
        Hashtable hash = new Hashtable();
        hash["PlayerName"] = PhotonNetwork.LocalPlayer.NickName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        UpdateMoney(0); // �� �ʱ�ȭ
        UpdateAllUI();  // UI ������Ʈ
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.I)) AddMoney(10);
            if (Input.GetKeyDown(KeyCode.Alpha1)) StealMoneyFromPlayer(0, 10);
            if (Input.GetKeyDown(KeyCode.Alpha2)) StealMoneyFromPlayer(1, 10);
            if (Input.GetKeyDown(KeyCode.Alpha3)) StealMoneyFromPlayer(2, 10);
            if (Input.GetKeyDown(KeyCode.Alpha4)) StealMoneyFromPlayer(3, 10);
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

            // �г��� UI �ݿ�
            if (player.CustomProperties.ContainsKey("PlayerName"))
            {
                nameTexts[index].text = (string)player.CustomProperties["PlayerName"];
            }

            // �� UI �ݿ�
            if (player.CustomProperties.ContainsKey("Money"))
            {
                int playerMoney = (int)player.CustomProperties["Money"];
                moneyTexts[index].text = $"P{player.ActorNumber}: {playerMoney}G";
            }
        }
    }

    public void StealMoneyFromPlayer(int playerIndex, int amount)
    {
        if (!photonView.IsMine) return;

        Player[] players = PhotonNetwork.PlayerList;

        if (playerIndex < 0 || playerIndex >= players.Length) return;

        Player targetPlayer = players[playerIndex];

        if (targetPlayer.CustomProperties.ContainsKey("Money"))
        {
            int targetMoney = (int)targetPlayer.CustomProperties["Money"];

            if (targetMoney >= amount)
            {
                UpdateMoney(amount);

                Hashtable hash = new Hashtable();
                hash["Money"] = targetMoney - amount;
                targetPlayer.SetCustomProperties(hash);

                // ��� UI �ݿ�
                UpdateAllUI();
            }
        }
    }
}
