using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Slider�� ����ϱ� ���� ���ӽ����̽�
using TMPro; // TextMeshPro�� ����ϱ� ���� ���ӽ����̽�

public class TitleSceneLoad : MonoBehaviour
{
    public string SceneName; // �ε��� �� �̸�
    public TMP_Text loadingText; // TMP �ؽ�Ʈ ����
    public Slider loadingBar; // �ε� �� �����̴� ����
    public float messageInterval = 1.0f; // �޽����� ����Ǵ� ���� (�� �� ������)

    private string[] loadingMessages = new string[]
    {
        "���ҽ��� �ҷ����� ���Դϴ�...",
        "������ ����ȭ ��...",
        "�ֻ��� ��������..",
        "���� �Դ���..",
    };

    private int currentMessageIndex = 0;
    private float timeSinceLastMessage = 0f;
    private float loadingProgress = 0f; // �ε� ���� ����
    private bool loadingFinished = false; // �ε� �Ϸ� ����

    private float fakeLoadingSpeed = 0.2f; // �⺻ �ε� �ӵ� (�� ������)
    private float slowdownTime = 0f; // �ε��� �������� �ð� ���

    void Start()
    {
        loadingText.text = loadingMessages[currentMessageIndex]; // ù ��° �޽����� ����
        loadingBar.value = 0f; // �����̴� �ʱ�ȭ
    }

    void Update()
    {
        if (!loadingFinished) // �ε��� �Ϸ���� �ʾ��� ���� ����
        {
            // �޽����� ���� �ð����� ���������� ��ȯ
            timeSinceLastMessage += Time.deltaTime;
            if (timeSinceLastMessage >= messageInterval)
            {
                timeSinceLastMessage = 0f;
                currentMessageIndex = (currentMessageIndex + 1) % loadingMessages.Length; // �ε����� ��ȯ
                loadingText.text = loadingMessages[currentMessageIndex]; // �ؽ�Ʈ ������Ʈ
            }

            // �ε� ���� ���� ������Ʈ
            if (slowdownTime <= 0f)
            {
                // �ε� �ӵ� �������� ������ Ȥ�� ������ ��ȭ
                fakeLoadingSpeed = Random.Range(0.05f, 0.2f); // �� ���� �ε� �ӵ�
                slowdownTime = Random.Range(0.3f, 0.8f); // �ӵ��� ������ �ð� ���� (ª��)
            }
            else
            {
                slowdownTime -= Time.deltaTime;
            }

            // �ε� ���� ���� ������Ʈ (������ ����ǵ���)
            loadingProgress += fakeLoadingSpeed * Time.deltaTime;
            loadingBar.value = Mathf.Clamp01(loadingProgress); // 0~1 ������ �� ����

            // �ε��� �Ϸ�Ǹ� �����̴��� �ؽ�Ʈ�� ������Ʈ
            if (loadingProgress >= 1f)
            {
                loadingFinished = true;
                loadingBar.gameObject.SetActive(false); // �����̴� �����
                loadingText.text = "-�ƹ� Ű�� �����ּ���-"; // �ε� �Ϸ� �޽��� ǥ��
            }
        }

        // �ε��� �Ϸ�� �� Ű�� ������ �� �� �ε�
        if (loadingFinished && Input.anyKey)
        {
            LoadingManager.LoadScene(SceneName); // �� �ε�
        }
    }
}
