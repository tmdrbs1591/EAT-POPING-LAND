using System.Collections;
using UnityEngine;
using DG.Tweening;

public class TitleUIBounce : MonoBehaviour
{
    [SerializeField] private float startOffsetY = 100f;        // 시작 위치 오프셋 (아래로)
    [SerializeField] private float moveDuration = 1f;          // 위로 떠오르는 시간
    [SerializeField] private float waitTime = 0.5f;            // 대기 시간
    [SerializeField] private float floatScaleMultiplier = 1.1f; // 약간 커지는 효과
    [SerializeField] private float scaleDuration = 1f;         // 스케일 변화 시간
    [SerializeField] private float scale = 0.9f;         

    private Vector3 originalPosition;
    private Vector3 originalScale;

    private void OnEnable()
    {
        originalPosition = transform.localPosition;
        originalScale = transform.localScale;

        // 시작 위치: 아래쪽으로 이동
        transform.localPosition = originalPosition - new Vector3(0, startOffsetY, 0);
        transform.localScale = originalScale * scale; // 약간 작게 시작

        Invoke(nameof(StartFloatUp), waitTime);
    }

    private void StartFloatUp()
    {
        Sequence floatSequence = DOTween.Sequence();

        // 부드럽게 위로 이동 + 천천히 커지기
        floatSequence.Append(transform.DOLocalMoveY(originalPosition.y, moveDuration).SetEase(Ease.OutSine));
        floatSequence.Join(transform.DOScale(originalScale * floatScaleMultiplier, scaleDuration).SetEase(Ease.OutSine));
        floatSequence.Append(transform.DOScale(originalScale, 0.5f).SetEase(Ease.OutQuad)); // 원래 크기로 복귀
    }
}
