using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public class MenuCameraMove : MonoBehaviour
{
    [SerializeField] private Transform charSelectCameraPos; // ���õ� ��ġ
    private Vector3 originalPosition; // ���� ��ġ
    private Quaternion originalRotation; // ���� ȸ�� ��

    [SerializeField] private GameObject charSelectBtn;
    [SerializeField] private GameObject charSelectPanel;

    void Start()
    {
        // ���� ��ġ�� ȸ�� ���� ����
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            MoveToOriginalPos();
        }
    }

    // ī�޶� ���õ� ��ġ�� �ڿ������� �̵���Ű�� �Լ�
    public void MoveToSelectPos()
    {
        AudioManager.instance.PlaySound(transform.position, 4, Random.Range(1f, 1f), 1f);
        // DOTween�� �̿��ؼ� ī�޶� �̵�
        transform.DOMove(charSelectCameraPos.position, 0.5f).SetEase(Ease.InOutQuad);
        transform.DORotate(charSelectCameraPos.rotation.eulerAngles, 1f).SetEase(Ease.InOutQuad);
        charSelectBtn.SetActive(false);
        charSelectPanel.SetActive(true);

    }

    // ���� ��ġ�� ī�޶� �ڿ������� �̵���Ű�� �Լ�
    public void MoveToOriginalPos()
    {
        // DOTween�� �̿��ؼ� ī�޶� �̵�
        transform.DOMove(originalPosition, 0.5f).SetEase(Ease.InOutQuad);
        transform.DORotate(originalRotation.eulerAngles, 1f).SetEase(Ease.InOutQuad);
        charSelectBtn.SetActive(true);
        charSelectPanel.SetActive(false);

    }
}
