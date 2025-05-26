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

        // 타겟 플레이어 찾기
        PhotonView targetPhotonView = PhotonView.Find(targetViewID);
        if (targetPhotonView == null)
        {
            Debug.LogError("타겟 PhotonView를 찾을 수 없습니다.");
            return;
        }

        GameObject playerGO = targetPhotonView.gameObject;
        Transform playerPos = playerGO.transform;

        // 이동 경로 설정
        Vector3 midPos = playerPos.position + Vector3.up * 3f;
        Vector3 endPos = holdPosPosition +Vector3.up * 1f; ;
        Vector3[] path = new[] { startPos, midPos, endPos };

        // 부드러운 곡선 경로 이동
        Tween moveTween = ufoTr.DOPath(path, flightDuration, PathType.CatmullRom)
            .SetEase(Ease.InOutSine);

        // 회전은 너무 과격하지 않게 시간 늘려서 부드럽게
        Tween rotateTween = ufoTr.DORotate(new Vector3(0, 360, 0), flightDuration * 2f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear);

        // 중간 위치 도달 시 플레이어 비활성화
        moveTween.OnWaypointChange(index =>
        {
            if (index == 1)
            {
                playerGO.SetActive(false);
                AudioManager.instance.PlaySound(transform.position, 9, Random.Range(1f, 1f), 1f);
            }
        });

        // 최종 도착 시 플레이어 위치 이동, 활성화, UFO 상승
        moveTween.OnComplete(() =>
        {
            playerGO.transform.position = endPos;
            playerGO.SetActive(true);

            if (targetPhotonView.IsMine)
            {
                var playerScript = targetPhotonView.GetComponent<PlayerControl>();
                playerScript.ColorChange();
            }

            AudioManager.instance.PlaySound(transform.position, 9, Random.Range(1f, 1f), 1f);

            // UFO 상승 연출
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
