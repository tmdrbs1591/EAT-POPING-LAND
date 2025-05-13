using UnityEngine;

public class GunRotate : MonoBehaviour
{
    void Update()
    {
        // ���콺 ��ġ�� ��ũ�� ��ǥ���� ��������
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;  // Z���� 0���� �����ؼ� 2D ��鿡���� ����ϵ���

        // ���� ��ġ���� ���콺������ ���� ���
        Vector3 direction = mousePos - transform.position;

        // ���� ���
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // ���� X, Y ȸ�� �� �����ϸ鼭 Z�� ȸ���� ����
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, angle);

        // 180�� �̻� ȸ���ϸ� ��������Ʈ �ø�
        if (angle > 90f || angle < -90f)
        {
            // ��������Ʈ ����
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            // ���� ���·� ����
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
