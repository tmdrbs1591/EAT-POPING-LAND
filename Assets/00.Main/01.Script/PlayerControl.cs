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

    private List<string> currentValidDirections = new List<string>(); // ���� ������ ���� ����Ʈ

    private string lastDirection = ""; // ������ �̵��� ����

    private Dictionary<string, string> oppositeDirection = new Dictionary<string, string>()
    {
        { "��", "�Ʒ�" },
        { "�Ʒ�", "��" },
        { "����", "������" },
        { "������", "����" }
    };

    void Start()
    {
        upButton.onClick.AddListener(() => StartCoroutine(MoveCor("��")));
        downButton.onClick.AddListener(() => StartCoroutine(MoveCor("�Ʒ�")));
        leftButton.onClick.AddListener(() => StartCoroutine(MoveCor("����")));
        rightButton.onClick.AddListener(() => StartCoroutine(MoveCor("������")));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            uiCanvas.SetActive(true);
        }

        CastRay(Vector3.forward, "��");
        CastRay(Vector3.back, "�Ʒ�");
        CastRay(Vector3.left, "����");
        CastRay(Vector3.right, "������");
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

            // ���� ���õ� ������ �������� ������, �ٸ� ���� �� ���� ����
            if (!currentValidDirections.Contains(direction))
            {
                List<string> validDirections = new List<string>(currentValidDirections);

                // ���� ������ �ݴ� ���� ����
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
                    Debug.Log("������ ���� ����! �̵� �ߴ�");
                    yield break;
                }
            }

            OnMoveButtonClicked(selectedDirection);
            lastDirection = selectedDirection; // ������ �̵� ���� ���

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