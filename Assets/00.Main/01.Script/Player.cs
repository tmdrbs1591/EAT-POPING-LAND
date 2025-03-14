using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public float rayDistance = 5f;
    private Dictionary<string, Vector3> directionPositions = new Dictionary<string, Vector3>();

    public Button upButton;
    public Button downButton;
    public Button leftButton;
    public Button rightButton;

    public float moveDuration = 0.5f; // �̵� �ð�

    void Start()
    {
        upButton.onClick.AddListener(() => OnMoveButtonClicked("��"));
        downButton.onClick.AddListener(() => OnMoveButtonClicked("�Ʒ�"));
        leftButton.onClick.AddListener(() => OnMoveButtonClicked("����"));
        rightButton.onClick.AddListener(() => OnMoveButtonClicked("������"));
    }

    void Update()
    {
        CastRay(Vector3.forward, "��");   // ��
        CastRay(Vector3.back, "�Ʒ�");      // �Ʒ�
        CastRay(Vector3.left, "����");      // ����
        CastRay(Vector3.right, "������");     // ������
    }

    void CastRay(Vector3 direction, string directionName)
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, rayDistance))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);

            if (hit.collider.CompareTag("PointBox"))
            {
                Vector3 targetPosition = hit.collider.bounds.center;
                targetPosition.y = transform.position.y; // Y�� ����
                directionPositions[directionName] = targetPosition;
                Debug.Log($"PointBox ����! ����: {directionName}");
            }
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + direction * rayDistance, Color.green);
            directionPositions.Remove(directionName); // �������� ������ ����
        }
    }

    void OnMoveButtonClicked(string direction)
    {
        if (directionPositions.ContainsKey(direction))
        {
            MovePlayerToTarget(directionPositions[direction]);
        }
    }

    void MovePlayerToTarget(Vector3 targetPosition)
    {
        transform.DOMove(targetPosition, moveDuration).SetEase(Ease.InOutQuad);
    }
}