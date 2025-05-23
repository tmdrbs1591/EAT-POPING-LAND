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

    void LateUpdate()
    {
        if (player1 == null || player2 == null) return;
        Move();
    }

    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        float distance = GetGreatestDistance();

        float targetDistance = Mathf.Lerp(minDistance, maxDistance, distance / zoomLimiter);

        // ī�޶��� �ٶ󺸴� ���� ���� �������� ��
        Vector3 zoomDir = -transform.forward;

        Vector3 targetPosition = centerPoint + zoomDir * targetDistance + offset;

        // Y�� ���� �Ǵ� �����ϰ� �ʹٸ� �Ʒ� �ڵ� ��� (����)
        // targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        transform.LookAt(centerPoint);
    }

    Vector3 GetCenterPoint()
    {
        return (player1.position + player2.position) / 2f;
    }

    float GetGreatestDistance()
    {
        return Vector3.Distance(player1.position, player2.position);
    }
}
