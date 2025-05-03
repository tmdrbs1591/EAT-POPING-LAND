using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class BackendManager : MonoBehaviour
{
    private void Awake()
    {
        BackendSetup();
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
