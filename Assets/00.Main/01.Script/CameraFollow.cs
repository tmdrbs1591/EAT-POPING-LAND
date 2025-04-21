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
        // 중간 지점
        Vector3 centerPoint = (player1.position + player2.position) / 2f;

        // 두 플레이어 사이 거리
        float distance = (player1.position - player2.position).magnitude;

        // 거리 기반으로 줌 거리 계산
        float desiredDistance = Mathf.Lerp(minDistance, maxDistance, distance / zoomLimiter);

        // 카메라가 뒤로 빠지는 방향 (카메라의 바라보는 방향 기준)
        Vector3 zoomDirection = -transform.forward * desiredDistance;

        // 최종 위치 계산
        Vector3 targetPosition = centerPoint + zoomDirection + offset;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
