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

        // Ÿ�� �÷��̾� ã��
        PhotonView targetPhotonView = PhotonView.Find(targetViewID);
        if (targetPhotonView == null)
        {
            Debug.LogError("Ÿ�� PhotonView�� ã�� �� �����ϴ�.");
            return;
        }

        GameObject playerGO = targetPhotonView.gameObject;
        Transform playerPos = playerGO.transform;

        // �̵� ��� ����
        Vector3 midPos = playerPos.position + Vector3.up * 3f;
        Vector3 endPos = holdPosPosition +Vector3.up * 1f; ;
        Vector3[] path = new[] { startPos, midPos, endPos };

        // �ε巯�� � ��� �̵�
        Tween moveTween = ufoTr.DOPath(path, flightDuration, PathType.CatmullRom)
            .SetEase(Ease.InOutSine);

        // ȸ���� �ʹ� �������� �ʰ� �ð� �÷��� �ε巴��
        Tween rotateTween = ufoTr.DORotate(new Vector3(0, 360, 0), flightDuration * 2f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear);

        // �߰� ��ġ ���� �� �÷��̾� ��Ȱ��ȭ
        moveTween.OnWaypointChange(index =>
        {
            if (index == 1)
            {
                playerGO.SetActive(false);
                AudioManager.instance.PlaySound(transform.position, 9, Random.Range(1f, 1f), 1f);
            }
        });

        // ���� ���� �� �÷��̾� ��ġ �̵�, Ȱ��ȭ, UFO ���
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

            // UFO ��� ����
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
