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
            // 현재 포커스된 인풋필드 확인
            var current = EventSystem.current.currentSelectedGameObject;

            // 다음 인풋필드 인덱스 계산
            currentIndex = (currentIndex + 1) % inputFields.Count;

            // 다음 인풋필드에 포커스 이동
            inputFields[currentIndex].Select();
        }
    }
}
