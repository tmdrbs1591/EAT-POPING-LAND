using UnityEngine;

public class GunRotate : MonoBehaviour
{
    void Update()
    {
        // 마우스 위치를 스크린 좌표에서 가져오기
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;  // Z값은 0으로 설정해서 2D 평면에서만 계산하도록

        // 총의 위치에서 마우스까지의 방향 계산
        Vector3 direction = mousePos - transform.position;

        // 각도 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 기존 X, Y 회전 값 유지하면서 Z축 회전만 적용
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, angle);

        // 180도 이상 회전하면 스프라이트 플립
        if (angle > 90f || angle < -90f)
        {
            // 스프라이트 반전
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            // 원래 상태로 복원
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
