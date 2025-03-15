using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Photon.Realtime;

public class PlayerControl : MonoBehaviour
{
    [Header("Stat")]
    public float moveDuration = 0.5f;
    public float jumpHeight = 1f;
    public float rayDistance = 5f;
    private Dictionary<string, Vector3> directionPositions = new Dictionary<string, Vector3>();

    [Header("UI")]
    public Button upButton;
    public Button downButton;
    public Button leftButton;
    public Button rightButton;
    public GameObject uiCanvas;

    private List<string> currentValidDirections = new List<string>(); // 현재 감지된 방향 리스트

    private string lastDirection = ""; // 마지막 이동한 방향

    private Dictionary<string, string> oppositeDirection = new Dictionary<string, string>()
    {
        { "위", "아래" },
        { "아래", "위" },
        { "왼쪽", "오른쪽" },
        { "오른쪽", "왼쪽" }
    };

    void Start()
    {
        upButton.onClick.AddListener(() => StartCoroutine(MoveCor("위")));
        downButton.onClick.AddListener(() => StartCoroutine(MoveCor("아래")));
        leftButton.onClick.AddListener(() => StartCoroutine(MoveCor("왼쪽")));
        rightButton.onClick.AddListener(() => StartCoroutine(MoveCor("오른쪽")));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            uiCanvas.SetActive(true);
        }

        CastRay(Vector3.forward, "위");
        CastRay(Vector3.back, "아래");
        CastRay(Vector3.left, "왼쪽");
        CastRay(Vector3.right, "오른쪽");
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
                targetPosition.y = transform.position.y;
                directionPositions[directionName] = targetPosition;

                if (!currentValidDirections.Contains(directionName))
                {
                    currentValidDirections.Add(directionName);
                }
            }
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + direction * rayDistance, Color.green);
            directionPositions.Remove(directionName);
            currentValidDirections.Remove(directionName);
        }
    }

    IEnumerator MoveCor(string direction)
    {
        uiCanvas.SetActive(false);

        for (int i = 0; i < DiceManager.instance.diceResult; i++)
        {
            string selectedDirection = direction;

            // 현재 선택된 방향이 감지되지 않으면, 다른 방향 중 랜덤 선택
            if (!currentValidDirections.Contains(direction))
            {
                List<string> validDirections = new List<string>(currentValidDirections);

                // 이전 방향의 반대 방향 제거
                if (!string.IsNullOrEmpty(lastDirection) && validDirections.Contains(oppositeDirection[lastDirection]))
                {
                    validDirections.Remove(oppositeDirection[lastDirection]);
                }

                if (validDirections.Count > 0)
                {
                    selectedDirection = validDirections[Random.Range(0, validDirections.Count)];
                }
                else
                {
                    Debug.Log("감지된 방향 없음! 이동 중단");
                    yield break;
                }
            }

            OnMoveButtonClicked(selectedDirection);
            lastDirection = selectedDirection; // 마지막 이동 방향 기록

            yield return new WaitForSeconds(0.6f);
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
        transform.DOJump(targetPosition, jumpHeight, 1, moveDuration).SetEase(Ease.InOutQuad);
    }
}