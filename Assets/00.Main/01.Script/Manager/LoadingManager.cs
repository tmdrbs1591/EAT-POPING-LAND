using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    static string nextScene;// ���� ���� �̸��� �����ϴ� ����
    [SerializeField]
    Image progressBar;// �ε� ���� ��Ȳ�� ǥ���� UI �̹���

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;// ���� �� �̸� ����
        SceneManager.LoadScene("LoadingScene");// ���� �� �̸� ����
    }

    void Start()
    {
        StartCoroutine(LoadSceneProgress()); // �� �ε� ���� ��Ȳ�� �����ϴ� �ڷ�ƾ ����
    }

    IEnumerator LoadSceneProgress()    // �� �ε� ���� ��Ȳ�� �����ϴ� �ڷ�ƾ
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene); // �񵿱� ������� ���� �ε��ϴ� AsyncOperation ��ü ����
        op.allowSceneActivation = false;// �� Ȱ��ȭ�� ����

        float timer = 0f; // Ÿ�̸� ���� �ʱ�ȭ
        float fakeLoadTime = 2f; // �ε� �ӵ��� �����ϱ� ���� ����

        while (!op.isDone)// �� �ε尡 �Ϸ�� ������ �ݺ�
        {
            yield return null; // ���� �����ӱ��� ���

            if (op.progress < 0.9f)// �� �ε� ���൵�� 90% �̸��� ���
            {
                progressBar.fillAmount = op.progress;// ���� ��Ȳ�� UI�� �ݿ�
            }
            else// �� �ε� ���൵�� 90% �̻��� ���
            {
                timer += Time.unscaledDeltaTime / fakeLoadTime; // Ÿ�̸� ���� 
                progressBar.fillAmount = Mathf.Lerp(0f, 1f, timer);// ���� ��Ȳ�� �ε巴�� UI�� �ݿ�

                if (progressBar.fillAmount >= 1f)// ���� ��Ȳ�� 100% �̻��� ���
                {
                    op.allowSceneActivation = true; // �� Ȱ��ȭ ���
                    yield break; // �ڷ�ƾ ����
                }
            }
        }
    }
}
