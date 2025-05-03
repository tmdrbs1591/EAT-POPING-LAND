using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TitleUIBounce : MonoBehaviour
{
    [SerializeField] private float startOffsetY = 100f; // 밑으로 내려갈 거리
    [SerializeField] private float moveDuration = 0.5f; // 튀어오르는 시간
    [SerializeField] private float waitTime = 1f; // 대기 시간
    [SerializeField] private Vector3 punchScale = new Vector3(1.2f, 1.2f, 1.2f); // 튈 때 스케일
    [SerializeField] private float scaleDuration = 0.3f; // 스케일 변화 시간

    private Vector3 originalPosition;
    private Vector3 originalScale;



    private void OnEnable()
    {
        originalPosition = transform.localPosition;
        originalScale = transform.localScale;

        transform.localPosition = originalPosition - new Vector3(0, startOffsetY, 0); // 밑에 대기
        Invoke(nameof(StartBounce), waitTime); // waitTime 기다렸다 튀어오름
    }
    private void StartBounce()
    {
        Sequence bounceSequence = DOTween.Sequence();

        bounceSequence.Append(transform.DOLocalMoveY(originalPosition.y, moveDuration).SetEase(Ease.OutQuad));
        bounceSequence.Join(transform.DOScale(punchScale, scaleDuration).SetEase(Ease.OutBack))
                       .Append(transform.DOScale(originalScale, 0.2f)); // 스케일 복구
    }
}
