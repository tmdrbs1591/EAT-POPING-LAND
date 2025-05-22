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

    public GameObject[] myTurnUI; 


    private void Awake()
    {
       if (photonView.IsMine)
        {
            TurnManager.instance.playerMoneyScript = this;

        }
    }
    void Start()
    {
        // 모든 UI 비활성화
        foreach (var ui in uiPositions)
        {
            ui.SetActive(false);
        }
        uiIndex = GetPlayerIndex(photonView.Owner);


        // 자신의 UI만 활성화
        uiPositions[uiIndex].SetActive(true);

        // 플레이어 이름 설정
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash["PlayerName"] = PhotonNetwork.LocalPlayer.NickName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        UpdateMoney(0);
    }
    void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.I))
                AddMoney(1000);
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
        int targetIndex = GetPlayerIndex(targetPlayer);

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
        // 자신의 UI만 활성화
        foreach (var ui in uiPositions)
        {
            ui.SetActive(false);
        }

        uiPositions[uiIndex].SetActive(true);  // 자신의 UI만 켬

        // 이름 및 돈 업데이트
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("PlayerName"))
        {
            nameTexts[uiIndex].text = (string)PhotonNetwork.LocalPlayer.CustomProperties["PlayerName"];
        }

        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Money"))
        {
            int playerMoney = (int)PhotonNetwork.LocalPlayer.CustomProperties["Money"];
            StopCoroutine(nameof(AnimateMoneyText));
            StartCoroutine(AnimateMoneyText(moneyTexts[uiIndex], playerMoney, PhotonNetwork.LocalPlayer.ActorNumber));
        }
    }

        private IEnumerator AnimateMoneyText(TextMeshProUGUI text, int targetMoney, int actorNumber)
    {
        string prefix = "";
        int currentMoney = ParseMoneyFromText(text.text);
        float duration = 0.5f;
        float timer = 0f;

        text.transform.DOKill(); // 기존 tween 제거
        text.transform.localScale = Vector3.one;

        // 애니메이션이 timeScale 무시하게 설정
        text.transform.DOScale(1.4f, 0.3f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true) // <- 여기가 중요
            .OnComplete(() =>
            {
                text.transform
                    .DOScale(1f, 0.15f)
                    .SetEase(Ease.InOutSine)
                    .SetDelay(0.2f)
                    .SetUpdate(true); // <- 이것도 중요
            });

        // Coroutine도 unscaledDeltaTime 사용
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime; // <- 핵심 변경
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

        string moneyStr = parts[1].Trim().Replace("만", "0000").Replace("만", "0000").Replace("", "");
        int result = 0;
        int.TryParse(moneyStr, out result);
        return result;
    }

    private string FormatKoreanMoney(int amount)
    {
        int man = amount / 10000;
        int remain = amount % 10000;

        if (man > 0 && remain > 0)
            return $"{man}만{remain}";
        else if (man > 0)
            return $"{man}만";
        else
            return $"{remain}";
    }

    private int GetPlayerIndex(Player player)
    {
        Player[] players = PhotonNetwork.PlayerList; // 현재 룸에 있는 플레이어 목록 (Join 순)
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].ActorNumber == player.ActorNumber)
                return i;
        }
        return -1; // 에러 처리
    }
public void SetTurnHighlight(bool isMyTurn)
{
    photonView.RPC("AnimateUIHighlightRPC", RpcTarget.AllBuffered, uiIndex, isMyTurn);

    if (photonView.IsMine)
    {
    photonView.RPC("MyTurnUIRPC", RpcTarget.AllBuffered,isMyTurn);
        }
    }
    [PunRPC]
    private void AnimateUIHighlightRPC(int index, bool isHighlight)
    {
        Transform uiTransform = uiPositions[index].transform;
        uiTransform.DOKill(); // 기존 tween 제거

        if (isHighlight)
        {
            uiTransform.DOScale(4.8f, 0.3f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
        }
        else
        {
            uiTransform.DOScale(4.4f, 0.3f)
                .SetEase(Ease.InOutSine)
                .SetUpdate(true);
         
        }
    }

    [PunRPC]
    private void MyTurnUIRPC(bool isMyTurn)
    {
        foreach (var ui in myTurnUI)
        {
            ui.SetActive(isMyTurn);
        }
    } 

}
