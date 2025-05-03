using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TitleUIBounce : MonoBehaviour
{
    [SerializeField] private float startOffsetY = 100f; // ������ ������ �Ÿ�
    [SerializeField] private float moveDuration = 0.5f; // Ƣ������� �ð�
    [SerializeField] private float waitTime = 1f; // ��� �ð�
    [SerializeField] private Vector3 punchScale = new Vector3(1.2f, 1.2f, 1.2f); // ƥ �� ������
    [SerializeField] private float scaleDuration = 0.3f; // ������ ��ȭ �ð�

    private Vector3 originalPosition;
    private Vector3 originalScale;



    private void OnEnable()
    {
        originalPosition = transform.localPosition;
        originalScale = transform.localScale;

        transform.localPosition = originalPosition - new Vector3(0, startOffsetY, 0); // �ؿ� ���
        Invoke(nameof(StartBounce), waitTime); // waitTime ��ٷȴ� Ƣ�����
    }
    private void StartBounce()
    {
        Sequence bounceSequence = DOTween.Sequence();

        bounceSequence.Append(transform.DOLocalMoveY(originalPosition.y, moveDuration).SetEase(Ease.OutQuad));
        bounceSequence.Join(transform.DOScale(punchScale, scaleDuration).SetEase(Ease.OutBack))
                       .Append(transform.DOScale(originalScale, 0.2f)); // ������ ����
    }
}
