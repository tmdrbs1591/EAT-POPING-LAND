using System.Collections;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;

public class ObjectDrop : MonoBehaviour
{
    public float dropDelay = 2f;         // ���� �������� ��� �ð�
    public float dropHeight = 5f;        // ������ �󸶳� ��������

    private Vector3 originalPosition;    // ���� ��ġ

    PhotonTransformView photonTransformview;
    Rigidbody rigid;
    void Start()
    {
        photonTransformview = GetComponent<PhotonTransformView>();
        rigid = GetComponent<Rigidbody>();
        originalPosition = transform.position;                     // ���� ��ġ ����
        transform.position += Vector3.up * dropHeight;             // ���� �÷��� �����

        StartCoroutine(PTCor());
        StartCoroutine(DropAfterDelay());
    }

    IEnumerator DropAfterDelay()
    {
        yield return new WaitForSeconds(dropDelay);                // ��� �ð� ��ٸ���
        transform.DOMove(originalPosition, 0.5f).SetEase(Ease.OutBounce);  // �ε巴�� ����߸���
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
