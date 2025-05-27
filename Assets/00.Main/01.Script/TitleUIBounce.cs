using System.Collections;
using UnityEngine;
using DG.Tweening;

public class TitleUIBounce : MonoBehaviour
{
    [SerializeField] private float startOffsetY = 100f;        // ���� ��ġ ������ (�Ʒ���)
    [SerializeField] private float moveDuration = 1f;          // ���� �������� �ð�
    [SerializeField] private float waitTime = 0.5f;            // ��� �ð�
    [SerializeField] private float floatScaleMultiplier = 1.1f; // �ణ Ŀ���� ȿ��
    [SerializeField] private float scaleDuration = 1f;         // ������ ��ȭ �ð�
    [SerializeField] private float scale = 0.9f;         

    private Vector3 originalPosition;
    private Vector3 originalScale;

    private void OnEnable()
    {
        originalPosition = transform.localPosition;
        originalScale = transform.localScale;

        // ���� ��ġ: �Ʒ������� �̵�
        transform.localPosition = originalPosition - new Vector3(0, startOffsetY, 0);
        transform.localScale = originalScale * scale; // �ణ �۰� ����

        Invoke(nameof(StartFloatUp), waitTime);
    }

    private void StartFloatUp()
    {
        Sequence floatSequence = DOTween.Sequence();

        // �ε巴�� ���� �̵� + õõ�� Ŀ����
        floatSequence.Append(transform.DOLocalMoveY(originalPosition.y, moveDuration).SetEase(Ease.OutSine));
        floatSequence.Join(transform.DOScale(originalScale * floatScaleMultiplier, scaleDuration).SetEase(Ease.OutSine));
        floatSequence.Append(transform.DOScale(originalScale, 0.5f).SetEase(Ease.OutQuad)); // ���� ũ��� ����
    }
}
