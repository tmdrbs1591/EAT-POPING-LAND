using UnityEngine;
using BackEnd; // �ڳ�SDK

public class BackendManager : MonoBehaviour
{

    private void Awake()
    {
        BackendSetup();
    }
    private void BackendSetup()
    {
        var bro = Backend.Initialize();

        if (bro.IsSuccess())
        {
            Debug.Log($"�ʱ�ȭ ����{bro}");
        }
        else
        {
            Debug.LogError("�ʱ�ȭ�ӽ���");
        }
    }
    
}
