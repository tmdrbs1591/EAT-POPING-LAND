using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;

public class PlayerMoney : MonoBehaviourPunCallbacks
{
    public GameObject[] uiPositions; // 4���� UI ������Ʈ (����, �»��, ���ϴ�, ���ϴ�)
    public TextMeshProUGUI[] moneyTexts; // 4���� �� ǥ�� UI (�� UI ����)

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

        UpdateMoney(0); // �� �ʱ�ȭ
        UpdateAllUI();  // UI ������Ʈ
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            // I Ű�� ������ �� ����
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
        UpdateAllUI(); // �÷��̾ ������ UI ������Ʈ
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateAllUI(); // �÷��̾ ������ UI ������Ʈ
    }

    private void UpdateAllUI()
    {
        // ��� UI�� ����
        foreach (var ui in uiPositions)
        {
            ui.SetActive(false);
        }

        // ���� ������ �÷��̾ UI�� Ȱ��ȭ
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            int index = (player.ActorNumber - 1) % uiPositions.Length;
            uiPositions[index].SetActive(true);
        }
    }
}
