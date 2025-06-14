﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
using static PlayerBattle;

public class PlayerControl : MonoBehaviourPunCallbacks
{
    [Header("Stat")]
    public float moveDuration = 0.5f;
    public float jumpHeight = 1f;
    public float rayDistance = 5f;
    private Dictionary<string, Vector3> directionPositions = new Dictionary<string, Vector3>();
    [SerializeField] GameObject playerColorBox;
    [SerializeField] public PlayerColorBox playerColorBoxScript;


    [Header("UI")]
    public Button upButton;
    public Button downButton;
    public Button leftButton;
    public Button rightButton;
    public GameObject uiCanvas;
    [SerializeField] TMP_Text nickNameText;
    [SerializeField] TMP_Text diceCountText;


    [SerializeField] GameObject playerSprite;
    [SerializeField] GameObject playerNickUI;


    private List<string> currentValidDirections = new List<string>(); // 현재 감지된 방향 리스트
    private string lastDirection = ""; // 마지막 이동한 방향
    private string currentDirection = "";

    public int remainingMoveCount;
    public bool isMove;
    public bool isWin;

    private Dictionary<string, string> oppositeDirection = new Dictionary<string, string>()
    {
        { "위", "아래" }, { "아래", "위" }, { "왼쪽", "오른쪽" }, { "오른쪽", "왼쪽" }
    };

    public PlayerBattle playerbattleScript;

    private void Awake()
    {
        playerbattleScript = GetComponent<PlayerBattle>();
        if (photonView.IsMine)
        {
            GameManager.instance.playerScript = this;
        }
        foreach (var Text in playerbattleScript.attackPowerUIText)
        {
            Text.text = playerbattleScript.attackPower.ToString() + "X";
        }
        foreach (var Text in playerbattleScript.HPUIText)
        {
            Text.text = playerbattleScript.maxHp.ToString();
        }
        foreach (var Text in playerbattleScript.MoveSpeedUIText)
        {
            Text.text = playerbattleScript.moveSpeed.ToString();
        }

    }
    void Start()
    {
        if (photonView.IsMine)
        {
            nickNameText.text = PhotonNetwork.NickName;
            nickNameText.color = Color.green;
        }
        else
        {
            nickNameText.text = photonView.Owner.NickName;
            nickNameText.color = Color.white;
        }

        // 버튼 리스너 설정
        upButton.onClick.AddListener(() => OnDirectionButtonClicked("위"));
        downButton.onClick.AddListener(() => OnDirectionButtonClicked("아래"));
        leftButton.onClick.AddListener(() => OnDirectionButtonClicked("왼쪽"));
        rightButton.onClick.AddListener(() => OnDirectionButtonClicked("오른쪽"));
    }

    void Update()
    {
        CastRay(Vector3.forward, "위");
        CastRay(Vector3.back, "아래");
        CastRay(Vector3.left, "왼쪽");
        CastRay(Vector3.right, "오른쪽");

        if (!photonView.IsMine)
            return;

        if (Input.GetKeyDown(KeyCode.Space) &&
           TurnManager.instance.IsMyTurn() &&
       !isMove &&
       !TurnManager.instance.isCountingDown && !BattleManager.instance.isBattle)
        {
            StartCoroutine(DiceResultCor());
            uiCanvas.SetActive(true);
            UpdateButtonVisibility();
        }
    }

    IEnumerator DiceResultCor()
    {
        yield return new WaitForSeconds(0.1f);
        photonView.RPC(nameof(DiceResultRPC), RpcTarget.All);
        yield return new WaitForSeconds(1f);
        photonView.RPC(nameof(DiceCountTextOn), RpcTarget.All);

    }
    [PunRPC]
    void DiceResultRPC()
    {
        remainingMoveCount = DiceManager.instance.diceResult;
    }
    void CastRay(Vector3 direction, string directionName)
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, rayDistance))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);

            if (hit.collider.CompareTag("PointBox"))
            {
                Vector3 targetPosition = hit.collider.bounds.center;
                targetPosition.y = transform.position.y;
                directionPositions[directionName] = targetPosition;

                if (!currentValidDirections.Contains(directionName))
                {
                    currentValidDirections.Add(directionName);
                }
            }
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + direction * rayDistance, Color.green);
            directionPositions.Remove(directionName);
            currentValidDirections.Remove(directionName);
        }
    }

    public void OnDirectionButtonClicked(string dir)
    {
        if (isMove) return;

        currentDirection = dir;
        StartCoroutine(MoveStep());
    }

    IEnumerator MoveStep()
    {
        isMove = true;
        uiCanvas.SetActive(false);
        photonView.RPC("TurnUICloseRPC", RpcTarget.All);
      


        while (remainingMoveCount > 0)
        {
            // 현재 방향이 유효하지 않으면 중단하고 UI 켜기
            if (!currentValidDirections.Contains(currentDirection))
            {
                Debug.Log("더 이상 갈 수 없음. 방향 재선택 필요");
                break;
            }

            photonView.RPC("OnMoveButtonClicked", RpcTarget.All, currentDirection);
            photonView.RPC(nameof(DiceCountDownRPC), RpcTarget.All);
            photonView.RPC(nameof(DiceTextUpdateRPC), RpcTarget.All);

            yield return new WaitForSeconds(0.2f);
            playerColorBoxScript.HoldDown();

        }

        isMove = false;

        if (remainingMoveCount <= 0)
        {
            ColorChange();
            photonView.RPC(nameof(DiceCountTextOff), RpcTarget.All);
            Debug.Log("턴 종료");
        }
        else
        {
            uiCanvas.SetActive(true);
            UpdateButtonVisibility();
        }
    }
    [PunRPC]
    void DiceCountDownRPC()
    {
        remainingMoveCount--;
    }
    [PunRPC]
    void DiceTextUpdateRPC()
    {
        diceCountText.text = remainingMoveCount.ToString();
    }
    [PunRPC]
    void DiceCountTextOn()
    {
        diceCountText.gameObject.SetActive(true);
        photonView.RPC(nameof(DiceTextUpdateRPC), RpcTarget.All);

    }
    [PunRPC]
    void DiceCountTextOff()
    {
        diceCountText.gameObject.SetActive(false);
    }
    [PunRPC]
    void TurnUICloseRPC()
    {
        TurnManager.instance.otherDiceUI.SetActive(false);
        Debug.Log("diceui닫기");
    }



    void UpdateButtonVisibility()
    {
        upButton.gameObject.SetActive(currentValidDirections.Contains("위"));
        downButton.gameObject.SetActive(currentValidDirections.Contains("아래"));
        leftButton.gameObject.SetActive(currentValidDirections.Contains("왼쪽"));
        rightButton.gameObject.SetActive(currentValidDirections.Contains("오른쪽"));
    }

    [PunRPC]
    void OnMoveButtonClicked(string direction)
    {
        if (directionPositions.ContainsKey(direction))
        {
            MovePlayerToTarget(directionPositions[direction]);
            AudioManager.instance.PlaySound(transform.position, 1, Random.Range(1f, 1.1f), 1);
        }
    }

    void MovePlayerToTarget(Vector3 targetPosition)
    {
        transform.DOJump(targetPosition, jumpHeight, 1, moveDuration).SetEase(Ease.InOutQuad);
    }






    public void ColorChange()
    {

        playerColorBoxScript.CheckPointBelow();
        Debug.Log("컬체 들아엄00;");
    }
    
    public void WinColorChange()
    {
        isWin = true;
        playerColorBoxScript.CheckPointBelow();
        isWin = false;
    }


    public void BattleStart()
    {
        BattleManager.instance.BattleStart();
        Debug.Log("배틀 시작");
    }

    [PunRPC]
    public void RPC_SetBattlePosition(Vector3 newPosition) //배틀 위치 이동
    {
        transform.position = newPosition;
        playerbattleScript.Init();
        playerbattleScript.enabled = true;
        playerbattleScript.hpSlider.gameObject.SetActive(true);

       playerbattleScript.hptext.text = playerbattleScript.curHp.ToString() + "/" + playerbattleScript.maxHp.ToString();

        PoisonZoneManager.instance.photonView.RPC("RPC_StartPoison", RpcTarget.AllBuffered);// 자기장 시작

        Debug.Log("배틀위치 이동 ~ ");

    }
    [PunRPC]
    public void RPC_SetRePosition(Vector3 newPosition) //원래 위치 이동
    {
        transform.position = newPosition;
        playerbattleScript.enabled = false;
        playerbattleScript.photonView.RPC("DieCanvasFalseRPC", RpcTarget.All);
        playerbattleScript.hpSlider.gameObject.SetActive(false);
        BattleManager.instance.winnerPanel.SetActive(false);
        photonView.RPC("SetAnimStateRPC", RpcTarget.All, (int)AnimState.Idle);
        Debug.Log("위치 이동 ~ ");

        BattleManager.instance.isPlayerDown = false;

        PoisonZoneManager.instance.photonView.RPC("RPC_StopPoison", RpcTarget.AllBuffered);// 자기장 중지
    }


    [PunRPC]
    public void RPC_SetRotation(float x, float y, float z)
    {
        playerSprite.transform.rotation = Quaternion.Euler(x, y, z);
        playerNickUI.transform.rotation = Quaternion.Euler(x, y, z);
    }
    [PunRPC]
    public void RPC_SetUIPosition(float x, float y, float z)
    {
        playerNickUI.GetComponent<RectTransform>().localPosition = new Vector3(x, y, z);
    }



}
