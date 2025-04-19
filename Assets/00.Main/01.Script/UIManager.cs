using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private Stack<GameObject> uiStack = new Stack<GameObject>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseTopUI();
        }
    }

    // UI ������Ʈ�� ���ÿ� �߰��ϸ鼭 Ȱ��ȭ
    public void OpenUI(GameObject uiObject)
    {
        uiObject.SetActive(true);
        uiStack.Push(uiObject);
    }

    // ������ �ֻ�� UI�� ��Ȱ��ȭ�ϰ� ����
    public void CloseTopUI()
    {
        if (uiStack.Count > 0)
        {
            GameObject topUI = uiStack.Pop();
            topUI.SetActive(false);
        }
    }

    // ��ü �ݱ� (���û���)
    public void CloseAllUI()
    {
        while (uiStack.Count > 0)
        {
            GameObject ui = uiStack.Pop();
            ui.SetActive(false);
        }
    }
}
