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
    float zoomOutSize; // Ȯ��� ũ��
    private void Awake()
    {
        originalCamSize = mainCam.orthographicSize;
        zoomOutSize = originalCamSize + 1f; // Ȯ��� ũ��
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

    [SerializeField] float flightDuration = 2f;   // ���� �ð� ����

    [PunRPC]
    public void UFOMove(int targetViewID, Vector3 holdPosPosition, Quaternion holdPosRotation)
    {

        Transform ufoTr = UFO.transform;

        // ���� ��ġ
        Vector3 startPos = new Vector3(0, 20, -25);
        ufoTr.position = startPos;

        // Ÿ�� �÷��̾� GameObject
        PhotonView targetPhotonView = PhotonView.Find(targetViewID);
        if (targetPhotonView == null)
        {
            Debug.LogError("Ÿ�� PhotonView�� ã�� �� �����ϴ�.");
            return;
        }

        GameObject playerGO = targetPhotonView.gameObject;
        Transform playerPos = playerGO.transform;

        // �߰� ��ġ (�÷��̾� ��), ���� ��ġ
        Vector3 midPos = playerPos.position + Vector3.up * 3f;
        Vector3 endPos = holdPosPosition;
        Vector3[] path = new[] { midPos, endPos };

        // �̵� Ʈ��
        Tween moveTween = ufoTr.DOPath(path, flightDuration, PathType.CatmullRom)
            .SetEase(Ease.InOutSine);

        Tween rotateTween = ufoTr.DORotate(new Vector3(0, 360, 0), flightDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear);

        // �߰� ��������Ʈ�� �������� �� �÷��̾� ��Ȱ��ȭ
        moveTween.OnWaypointChange(index =>
        {
            if (index == 1)
            {
                playerGO.SetActive(false);
            }
        });

        // ���� �������� �� ��ġ �̵� �� ��Ȱ��ȭ + UFO ���� �̵�
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
        // ��ġ �� ȸ�� ������ Vector3�� Quaternion���� ����
        Vector3 holdPosPosition = holdPos.position;
        Quaternion holdPosRotation = holdPos.rotation;
        SystemMessaageManager.instance.MessageTextStart("���ư���~ ������~~");

        photonView.RPC("UFOMove", RpcTarget.All, targetViewID, holdPosPosition, holdPosRotation);
    }


}
