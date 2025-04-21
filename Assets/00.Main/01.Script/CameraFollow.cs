using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    public float minDistance = 10f;
    public float maxDistance = 30f;
    public float zoomLimiter = 20f;

    public Vector3 offset;
    public float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;

    void FixedUpdate()
    {
        if (player1 == null || player2 == null)
            return;

        Move();
    }

    void Move()
    {
        // �߰� ����
        Vector3 centerPoint = (player1.position + player2.position) / 2f;

        // �� �÷��̾� ���� �Ÿ�
        float distance = (player1.position - player2.position).magnitude;

        // �Ÿ� ������� �� �Ÿ� ���
        float desiredDistance = Mathf.Lerp(minDistance, maxDistance, distance / zoomLimiter);

        // ī�޶� �ڷ� ������ ���� (ī�޶��� �ٶ󺸴� ���� ����)
        Vector3 zoomDirection = -transform.forward * desiredDistance;

        // ���� ��ġ ���
        Vector3 targetPosition = centerPoint + zoomDirection + offset;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
