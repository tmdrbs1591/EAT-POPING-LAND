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

    public float moveDuration = 0.5f; // 이동 시간

    void Start()
    {
        upButton.onClick.AddListener(() => OnMoveButtonClicked("위"));
        downButton.onClick.AddListener(() => OnMoveButtonClicked("아래"));
        leftButton.onClick.AddListener(() => OnMoveButtonClicked("왼쪽"));
        rightButton.onClick.AddListener(() => OnMoveButtonClicked("오른쪽"));
    }

    void Update()
    {
        CastRay(Vector3.forward, "위");   // 위
        CastRay(Vector3.back, "아래");      // 아래
        CastRay(Vector3.left, "왼쪽");      // 왼쪽
        CastRay(Vector3.right, "오른쪽");     // 오른쪽
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
                targetPosition.y = transform.position.y; // Y값 고정
                directionPositions[directionName] = targetPosition;
                Debug.Log($"PointBox 감지! 방향: {directionName}");
            }
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + direction * rayDistance, Color.green);
            directionPositions.Remove(directionName); // 감지되지 않으면 제거
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