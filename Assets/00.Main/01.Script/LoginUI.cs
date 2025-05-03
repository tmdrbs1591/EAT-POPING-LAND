using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class LoginUI : MonoBehaviour
{
    public List<TMP_InputField> inputFields;
    private int currentIndex = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // ���� ��Ŀ���� ��ǲ�ʵ� Ȯ��
            var current = EventSystem.current.currentSelectedGameObject;

            // ���� ��ǲ�ʵ� �ε��� ���
            currentIndex = (currentIndex + 1) % inputFields.Count;

            // ���� ��ǲ�ʵ忡 ��Ŀ�� �̵�
            inputFields[currentIndex].Select();
        }
    }
}
