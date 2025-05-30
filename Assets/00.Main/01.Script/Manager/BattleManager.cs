using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;
using Spine.Unity;

public class BattleManager : MonoBehaviourPun
{
    public static BattleManager instance;

    public GameObject battleCamera;
    public GameObject battleScreen;

    public Transform firstBattlePos;
    public Transform secondBattlePos;

    private string challengerName; // 배틀 건 사람
    private string defenderName;   // 배틀 당한 사람


    private Dictionary<int, Vector3> playerOriginalPositions = new Dictionary<int, Vector3>(); // 플레이어 원래 위치 저장

    [SerializeField] TMP_Text firstPlayerNameText;
    [SerializeField] TMP_Text secondPlayerNameText;
    [SerializeField] GameObject battlePanel;
    [SerializeField] public GameObject winnerPanel;
    [SerializeField] TMP_Text winnerNameText;
    [SerializeField] public GameObject battleEndPanel;
    [SerializeField] public GameObject holdUpgradePanel;// 땅 업그레이드 패널


    private string opponentName = "";

    private int player1ID;
    private int player2ID;
    private int winnerActorID = -1; // 승자 ID 저장용

    public bool isBattle; // 배틀중인지

    public bool isPlayerDown;//경기 플레이어가 죽었는지

    public Vector3 startPos;

    public int holdPrice;

    [SerializeField] private SkeletonGraphic player1SkeletonGraphic;
    [SerializeField] private SkeletonGraphic player2SkeletonGraphic;

    [Header("Skin 설정")]
    [SerializeField] private string redSkin = "skin3";
    [SerializeField] private string blueSkin = "skin2";
    [SerializeField] private string yellowSkin = "skin4";
    [SerializeField] private string blackSkin = "skin1";

    [Header("애니메이션 설정")]
    [SerializeField] private AnimationReferenceAsset redAnim;
    [SerializeField] private AnimationReferenceAsset blueAnim;
    [SerializeField] private AnimationReferenceAsset yellowAnim;
    [SerializeField] private AnimationReferenceAsset blackAnim;

    private void Awake()
    {
        instance = this;
        startPos = battleCamera.transform.position;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    BattleWin();
        //}
        if (Input.GetKeyDown(KeyCode.L))
        {
        }
    }

    public void SetBattleInfo(string opponent)
    {
        opponentName = opponent;
    }

    public void BattleStart()
    {
        if (string.IsNullOrEmpty(opponentName))
            return;


        challengerName = PhotonNetwork.NickName;
        defenderName = opponentName;


        // 시스템 메시지 출력
        SystemMessaageManager.instance.MessageTextStart($"{challengerName}님이 {defenderName}님에게 배틀을 신청했습니다!");

        photonView.RPC("RPC_BattleStart", RpcTarget.All, challengerName, defenderName);
    }



    [PunRPC]
    private void RPC_BattleStart(string attacker, string defender)
    {
        isBattle = true;
        SongManager.instance.SongChange(3);


        battleCamera.transform.position = startPos;
        battleScreen.SetActive(true);
        battleCamera.SetActive(true);
        battlePanel.SetActive(false);
        battlePanel.SetActive(true);

        firstPlayerNameText.text = attacker;
        secondPlayerNameText.text = defender;

        // 내부적으로도 공격자와 방어자 저장
        challengerName = attacker;
        defenderName = defender;

        SavePlayerPositions(attacker, defender);
        MovePlayerToBattlePos(attacker, firstBattlePos.position);
        MovePlayerToBattlePos(defender, secondBattlePos.position);

        player1ID = PhotonNetwork.CurrentRoom.Players.Values.First(p => p.NickName == attacker).ActorNumber;
        player2ID = PhotonNetwork.CurrentRoom.Players.Values.First(p => p.NickName == defender).ActorNumber;

        // player1ID에 해당하는 플레이어 처리
        if (PhotonNetwork.CurrentRoom.Players.TryGetValue(player1ID, out Player player1))
        {
            if (player1.CustomProperties.TryGetValue("CharacterType", out object charTypeObj))
            {
                int charTypeInt = (int)charTypeObj;
                CharacterType characterType = (CharacterType)charTypeInt;
                Debug.Log($"[{player1.NickName}] 캐릭터 타입: {characterType}");

                // SkeletonGraphic이 비어있지 않다면 적용
                if (player1SkeletonGraphic != null && player1SkeletonGraphic.Skeleton != null)
                {
                    string skinName = GetSkinName(characterType);
                    player1SkeletonGraphic.Skeleton.SetSkin(skinName);
                    player1SkeletonGraphic.Skeleton.SetSlotsToSetupPose();
                    player1SkeletonGraphic.AnimationState.Apply(player1SkeletonGraphic.Skeleton);

                    AnimationReferenceAsset anim = GetAnimation(characterType);
                    if (anim != null)
                    {
                        player1SkeletonGraphic.AnimationState.SetAnimation(0, anim, true);
                    }
                }
            }
        }

        // player2ID에 해당하는 플레이어 처리
        if (PhotonNetwork.CurrentRoom.Players.TryGetValue(player2ID, out Player player2))
        {
            if (player2.CustomProperties.TryGetValue("CharacterType", out object charTypeObj2)) // 여기 수정
            {
                int charTypeInt = (int)charTypeObj2;
                CharacterType characterType = (CharacterType)charTypeInt;
                Debug.Log($"[{player2.NickName}] 캐릭터 타입: {characterType}");

                // SkeletonGraphic이 비어있지 않다면 적용
                if (player2SkeletonGraphic != null && player2SkeletonGraphic.Skeleton != null)
                {
                    string skinName = GetSkinName(characterType);
                    player2SkeletonGraphic.Skeleton.SetSkin(skinName);
                    player2SkeletonGraphic.Skeleton.SetSlotsToSetupPose();
                    player2SkeletonGraphic.AnimationState.Apply(player2SkeletonGraphic.Skeleton);

                    AnimationReferenceAsset anim = GetAnimation(characterType);
                    if (anim != null)
                    {
                        player2SkeletonGraphic.AnimationState.SetAnimation(0, anim, true);
                    }
                }
            }
        }

        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player"); // 배틀 카메라에 플레이어 위치들 넣기
        foreach (GameObject go in allPlayers)
        {
            var view = go.GetComponent<PhotonView>();
            if (view != null && view.OwnerActorNr == player1ID)
            {
                var battleCameraScript = battleCamera.GetComponent<CameraFollow>();
                battleCameraScript.player1 = go.transform;
                break;
            }
        }

        foreach (GameObject go in allPlayers)
        {
            var view = go.GetComponent<PhotonView>();
            if (view != null && view.OwnerActorNr == player2ID)
            {
                var battleCameraScript = battleCamera.GetComponent<CameraFollow>();
                battleCameraScript.player2 = go.transform;
                break;
            }
        }
       // StartCoroutine(DicePanelCloseUICor());


    }

    IEnumerator DicePanelCloseUICor()
    {
        yield return new WaitForSeconds(0.2f);
        TurnManager.instance.diceUI.SetActive(false);
        TurnManager.instance.otherDiceUI.SetActive(false);

    }

    private void SavePlayerPositions(string player1, string player2)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in players)
        {
            PhotonView pv = player.GetComponent<PhotonView>();
            if (pv != null)
            {
                int playerID = pv.Owner.ActorNumber;
                playerOriginalPositions[playerID] = player.transform.position; // 원래 위치 저장
            }
        }
    }

    private void MovePlayerToBattlePos(string playerName, Vector3 position)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in players)
        {
            PhotonView pv = player.GetComponent<PhotonView>();
            if (pv != null && pv.Owner.NickName == playerName)
            {
                pv.RPC("RPC_SetBattlePosition", RpcTarget.All, position);
                pv.RPC("RPC_SetRotation", RpcTarget.All, 40f, 0f, 0f);
                pv.RPC("RPC_SetUIPosition", RpcTarget.All, -0.3f, 4f, 2.8f);
                break;
            }
        }


    }

    //public void BattleWin()
    //{
    //    int winnerID = PhotonNetwork.LocalPlayer.ActorNumber;
    //    photonView.RPC("RPC_BattleResult", RpcTarget.All, winnerID);
    //}
    public void BattleLose()
    {
        int myID = PhotonNetwork.LocalPlayer.ActorNumber;
        int opponentID = (myID == player1ID) ? player2ID : player1ID;

        Player winner = PhotonNetwork.CurrentRoom.GetPlayer(opponentID);
        Player loser = PhotonNetwork.LocalPlayer;

        string winnerName = winner.NickName;
        string loserName = loser.NickName;
        // 승자와 패자에게만 승패 판단용
        photonView.RPC("RPC_BattleResult", winner, opponentID);
        photonView.RPC("RPC_BattleResult", loser, opponentID);

        // 모든 클라이언트에게 승자 이름 보여주기
        photonView.RPC("RPC_ShowWinner", RpcTarget.All, winnerName);

        if (winnerName == defenderName) // 배틀 당한 애가 이겼을 때
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in players)
            {
                PhotonView pv = player.GetComponent<PhotonView>();

                if (pv == null) continue;

                // 이긴 사람에게 돈 100원
                if (pv.Owner.ActorNumber == winner.ActorNumber)
                {
                    PlayerMoney money = player.GetComponent<PlayerMoney>();
                    if (money != null)
                    {
                        Debug.Log("이긴 사람 돈 추가");
                        StartCoroutine(AddMoney(money, holdPrice));
                    }
                }

                // 진 사람에게도 돈 50원
                else if (pv.Owner.ActorNumber == loser.ActorNumber)
                {
                    PlayerMoney money = player.GetComponent<PlayerMoney>();
                    if (money != null)
                    {
                        Debug.Log("진 사람 돈 빼기");
                        StartCoroutine(AddMoney(money, -holdPrice));
                    }
                }
            }
        }

        SystemMessaageManager.instance.MessageTextStart(GetBattleResultMessage(winnerName));
    }
    IEnumerator AddMoney(PlayerMoney money , int holdPrice)
    {
        yield return new WaitForSeconds(2f);
        money.AddMoney(holdPrice);
    }
    private string GetBattleResultMessage(string winnerName)
    {
        if (winnerName == challengerName) // 배틀 건 애가 이겼을때
        {
            return $"{challengerName}님이 {defenderName}님의 땅을 빼앗았습니다!";
        }
        else if (winnerName == defenderName) // 배틀당한애가 이겼을때
        {
            return $"{defenderName}님이 땅을 지켜 {challengerName}님의 캔디코인을 빼앗았습니다!";
        }
        else
        {
            return $"{winnerName}님이 승리하였습니다!";
        }
    }


    [PunRPC]
    private void RPC_BattleResult(int winnerID)
    {
        winnerActorID = winnerID; // 저장해둠

        if (PhotonNetwork.LocalPlayer.ActorNumber == winnerID)
        {
            Debug.Log("승리!");
        }
        else
        {
            Debug.Log("패배!");
        }
    }

    [PunRPC]
    public void RPC_ShowWinner(string winnerName)
    {
        winnerPanel.SetActive(true);
        winnerNameText.text = $"{winnerName} WIN";
    }

    [PunRPC]
    public void ResetPosPlayerRPC() //플레이어 위치 다시 제자리로
    {
        ResetPlayerPositions();
    }

    [PunRPC]
    private void RPC_BattlePanelFalse()
    {
        battlePanel.SetActive(false);
        battleScreen.SetActive(false);
        isBattle = false;
    }
    private void ResetPlayerPositions()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // 🏆 승자 닉네임 가져오기
        string winnerName = PhotonNetwork.CurrentRoom.GetPlayer(winnerActorID).NickName;

        foreach (var player in players)
        {
            PhotonView pv = player.GetComponent<PhotonView>();
            if (pv != null)
            {
                int playerID = pv.Owner.ActorNumber;

                if (playerOriginalPositions.ContainsKey(playerID))
                {
                    Vector3 originalPos = playerOriginalPositions[playerID];
                    pv.RPC("RPC_SetRePosition", RpcTarget.All, originalPos);
                    pv.RPC("RPC_SetRotation", RpcTarget.All, 31.7f, 42.8f, 0f);
                    pv.RPC("RPC_SetUIPosition", RpcTarget.All, 0.56f, 4.4f, 0.53f);
                }
                else
                {
                    Debug.Log("해당 플레이어 ID의 위치 정보 없음!");
                }

                if (winnerName == defenderName)
                {
                    Debug.Log("배틀에서 디펜더가 승리. 위치 리셋 후 계속 진행.");
                    continue;
                }

                else if (winnerName == challengerName) // 배틀 당한 애가 이겼을 때
                {
                    if (playerID == winnerActorID)
                    {
                        PlayerControl control = player.GetComponent<PlayerControl>();
                        if (control != null)
                        {
                            if (photonView.IsMine)
                            {
                                control.WinColorChange();
                                Debug.Log("<color=red>Red</color>컬러 체인지");
                            }
                        
                        }
                    }
                }
            }
        }

        playerOriginalPositions.Clear();
    }
    private string GetSkinName(CharacterType type)
    {
        switch (type)
        {
            case CharacterType.Red: return redSkin;
            case CharacterType.Blue: return blueSkin;
            case CharacterType.Yellow: return yellowSkin;
            case CharacterType.Black: return blackSkin;
            default: return redSkin;
        }
    }

    private AnimationReferenceAsset GetAnimation(CharacterType type)
    {
        switch (type)
        {
            case CharacterType.Red: return redAnim;
            case CharacterType.Blue: return blueAnim;
            case CharacterType.Yellow: return yellowAnim;
            case CharacterType.Black: return blackAnim;
            default: return redAnim;
        }
    }

}
