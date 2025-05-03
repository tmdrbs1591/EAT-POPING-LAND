using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class BackendManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        BackendSetup();
    }
    private void Update()
    {
        if (Backend.IsInitialized) {
            Backend.AsyncPoll();
        }
    }
    private void BackendSetup()
    {
        var bro = Backend.Initialize(true);

        if (bro.IsSuccess())
        {
            Debug.Log($"�ʱ�ȭ ���� : {bro}");

        }
        else
        {
            Debug.Log($"�ʱ�ȭ ���� : {bro}" );
        }
    }
}
