using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public class MenuCameraMove : MonoBehaviour
{
    [SerializeField] private Transform charSelectCameraPos; // 선택된 위치
    private Vector3 originalPosition; // 원래 위치
    private Quaternion originalRotation; // 원래 회전 값

    [SerializeField] private GameObject charSelectBtn;
    [SerializeField] private GameObject charSelectPanel;

    void Start()
    {
        // 원래 위치와 회전 값을 저장
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            MoveToOriginalPos();
        }
    }

    // 카메라를 선택된 위치로 자연스럽게 이동시키는 함수
    public void MoveToSelectPos()
    {
        AudioManager.instance.PlaySound(transform.position, 4, Random.Range(1f, 1f), 1f);
        // DOTween을 이용해서 카메라 이동
        transform.DOMove(charSelectCameraPos.position, 0.5f).SetEase(Ease.InOutQuad);
        transform.DORotate(charSelectCameraPos.rotation.eulerAngles, 1f).SetEase(Ease.InOutQuad);
        charSelectBtn.SetActive(false);
        charSelectPanel.SetActive(true);

    }

    // 원래 위치로 카메라를 자연스럽게 이동시키는 함수
    public void MoveToOriginalPos()
    {
        // DOTween을 이용해서 카메라 이동
        transform.DOMove(originalPosition, 0.5f).SetEase(Ease.InOutQuad);
        transform.DORotate(originalRotation.eulerAngles, 1f).SetEase(Ease.InOutQuad);
        charSelectBtn.SetActive(true);
        charSelectPanel.SetActive(false);

    }
}
