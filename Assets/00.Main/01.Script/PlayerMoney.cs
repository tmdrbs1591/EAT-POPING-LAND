using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;
using System.Collections;
using DG.Tweening;

public class PlayerMoney : MonoBehaviourPunCallbacks
{
    public GameObject[] uiPositions; // UI 위치들 (우상단, 좌상단 등)
    public TextMeshProUGUI[] moneyTexts; // 돈 텍스트
    public TextMeshProUGUI[] nameTexts;  // 이름 텍스트

    public int money = 100; // 초기 돈
    public int uiIndex;     // UI 인덱스

    void Start()
    {
        foreach (var ui in uiPositions)
        {
            ui.SetActive(false);
        }

        uiIndex = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % uiPositions.Length;
        uiPositions[uiIndex].SetActive(true);

        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash["PlayerName"] = PhotonNetwork.LocalPlayer.NickName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        UpdateMoney(0);
        UpdateAllUI();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.I))
                AddMoney(10);
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

        if (photonView.IsMine)
        {
            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
            hash["Money"] = money;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        int targetIndex = (targetPlayer.ActorNumber - 1) % moneyTexts.Length;

        if (changedProps.ContainsKey("Money"))
        {
            int updatedMoney = (int)changedProps["Money"];
            StopCoroutine(nameof(AnimateMoneyText));
            StartCoroutine(AnimateMoneyText(moneyTexts[targetIndex], updatedMoney, targetPlayer.ActorNumber));
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

            if (player.CustomProperties.ContainsKey("PlayerName"))
            {
                nameTexts[index].text = (string)player.CustomProperties["PlayerName"];
            }

            if (player.CustomProperties.ContainsKey("Money"))
            {
                int playerMoney = (int)player.CustomProperties["Money"];
                StopCoroutine(nameof(AnimateMoneyText));
                StartCoroutine(AnimateMoneyText(moneyTexts[index], playerMoney, player.ActorNumber));
            }
        }
    }

    private IEnumerator AnimateMoneyText(TextMeshProUGUI text, int targetMoney, int actorNumber)
    {
        string prefix = "돈 : ";
        int currentMoney = ParseMoneyFromText(text.text);
        float duration = 0.5f;
        float timer = 0f;

        text.transform.DOKill(); // 기존 tween 제거
        text.transform.localScale = Vector3.one;

        text.transform.DOScale(1.4f, 0.3f).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                text.transform
                    .DOScale(1f, 0.15f)
                    .SetEase(Ease.InOutSine)
                    .SetDelay(0.2f); // 여기가 '커진 상태 유지 시간'
            });

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            int displayMoney = Mathf.RoundToInt(Mathf.Lerp(currentMoney, targetMoney, t));
            text.text = prefix + FormatKoreanMoney(displayMoney);
            yield return null;
        }

        text.text = prefix + FormatKoreanMoney(targetMoney);
    }

    private int ParseMoneyFromText(string text)
    {
        // 예: "P2: 1만2000원"에서 숫자만 추출
        string[] parts = text.Split(':');
        if (parts.Length < 2) return 0;

        string moneyStr = parts[1].Trim().Replace("만원", "0000").Replace("만", "0000").Replace("원", "");
        int result = 0;
        int.TryParse(moneyStr, out result);
        return result;
    }

    private string FormatKoreanMoney(int amount)
    {
        int man = amount / 10000;
        int remain = amount % 10000;

        if (man > 0 && remain > 0)
            return $"{man}만{remain}원";
        else if (man > 0)
            return $"{man}만원";
        else
            return $"{remain}원";
    }
}
