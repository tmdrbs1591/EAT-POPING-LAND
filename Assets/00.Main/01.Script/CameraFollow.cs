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
        Vector3 zoomDir = new Vector3(0, 0, -1).normalized; // 고정된 방향으로 줌 (z-방향)

        Vector3 targetPosition = centerPoint + zoomDir * targetDistance + offset;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        transform.LookAt(centerPoint); // 항상 중심 바라보게
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
