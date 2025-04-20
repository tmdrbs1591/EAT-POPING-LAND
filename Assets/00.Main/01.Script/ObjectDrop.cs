using System.Collections;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;

public class ObjectDrop : MonoBehaviour
{
    public float dropDelay = 2f;         // 언제 떨어질지 대기 시간
    public float dropHeight = 5f;        // 위에서 얼마나 떨어질지

    private Vector3 originalPosition;    // 원래 위치

    PhotonTransformView photonTransformview;
    Rigidbody rigid;
    void Start()
    {
        photonTransformview = GetComponent<PhotonTransformView>();
        rigid = GetComponent<Rigidbody>();
        originalPosition = transform.position;                     // 현재 위치 저장
        transform.position += Vector3.up * dropHeight;             // 위로 올려서 숨기기

        StartCoroutine(PTCor());
        StartCoroutine(DropAfterDelay());
    }

    IEnumerator DropAfterDelay()
    {
        yield return new WaitForSeconds(dropDelay);                // 대기 시간 기다리기
        transform.DOMove(originalPosition, 0.5f).SetEase(Ease.OutBounce);  // 부드럽게 떨어뜨리기
    }

    IEnumerator PTCor()
    {
        if (photonTransformview != null)
        {
            photonTransformview.enabled = false;
            yield return new WaitForSeconds(7f);
            photonTransformview.enabled = true;
        }

        if (rigid != null)
        {
            rigid.constraints |= RigidbodyConstraints.FreezePositionY;
            yield return new WaitForSeconds(7f);
            rigid.constraints &= ~RigidbodyConstraints.FreezePositionY;
        }
    }
}
