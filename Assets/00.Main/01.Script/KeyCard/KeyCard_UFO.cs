using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;


public class KeyCard_UFO : MonoBehaviourPunCallbacks, KeyCardEvents
{
    [SerializeField] List<GameObject> holdMoveButtons;

    [SerializeField] GameObject panel;
    [SerializeField] GameObject UFO;


    [SerializeField] Camera mainCam;
    float originalCamSize;
    float zoomOutSize; // 확대될 크기
    private void Awake()
    {
        originalCamSize = mainCam.orthographicSize;
        zoomOutSize = originalCamSize + 1f; // 확대될 크기
    }

    public void EventStart()
    {
        UIOpen();
    }
    public void EventEnd()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            UIOpen();
        }
        else if (Input.GetKeyUp(KeyCode.T))
        {
            UIclose();
        }
    }
    public void UIOpen()
    {
        mainCam.DOOrthoSize(zoomOutSize, 0.5f);
        panel.SetActive(true);
        foreach (GameObject img in holdMoveButtons)
        {
            img.SetActive(true);
        }

    }
    public void UIclose()
    {
        mainCam.DOOrthoSize(originalCamSize, 0.5f);
        panel.SetActive(false);
        foreach (GameObject img in holdMoveButtons)
        {
            img.SetActive(false);
        }
    }

    [SerializeField] float flightDuration = 2f;   // 비행 시간 조정

    [PunRPC]
    public void UFOMove(int targetViewID, Vector3 holdPosPosition, Quaternion holdPosRotation)
    {

        Transform ufoTr = UFO.transform;

        // 시작 위치
        Vector3 startPos = new Vector3(0, 20, -25);
        ufoTr.position = startPos;

        // 타겟 플레이어 GameObject
        PhotonView targetPhotonView = PhotonView.Find(targetViewID);
        if (targetPhotonView == null)
        {
            Debug.LogError("타겟 PhotonView를 찾을 수 없습니다.");
            return;
        }

        GameObject playerGO = targetPhotonView.gameObject;
        Transform playerPos = playerGO.transform;

        // 중간 위치 (플레이어 위), 도착 위치
        Vector3 midPos = playerPos.position + Vector3.up * 3f;
        Vector3 endPos = holdPosPosition;
        Vector3[] path = new[] { midPos, endPos };

        // 이동 트윈
        Tween moveTween = ufoTr.DOPath(path, flightDuration, PathType.CatmullRom)
            .SetEase(Ease.InOutSine);

        Tween rotateTween = ufoTr.DORotate(new Vector3(0, 360, 0), flightDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear);

        // 중간 웨이포인트에 도달했을 때 플레이어 비활성화
        moveTween.OnWaypointChange(index =>
        {
            if (index == 1)
            {
                playerGO.SetActive(false);
            }
        });

        // 최종 도착했을 때 위치 이동 및 재활성화 + UFO 위로 이동
        moveTween.OnComplete(() =>
        {
            playerGO.transform.position = endPos;
            playerGO.SetActive(true);

                var playerScript = targetPhotonView.GetComponent<PlayerControl>();
                playerScript.ColorChange();
         
            ufoTr.DOMoveY(40f, 1f).SetEase(Ease.InOutSine);
        });
    }

    public void CallUFOMove(int targetViewID, Transform holdPos)
    {
        // 위치 및 회전 정보를 Vector3와 Quaternion으로 전달
        Vector3 holdPosPosition = holdPos.position;
        Quaternion holdPosRotation = holdPos.rotation;
        SystemMessaageManager.instance.MessageTextStart("날아간다~ 슝슝슝~~");

        photonView.RPC("UFOMove", RpcTarget.All, targetViewID, holdPosPosition, holdPosRotation);
    }


}
