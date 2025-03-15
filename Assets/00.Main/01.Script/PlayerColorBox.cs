using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColorBox : MonoBehaviour
{
    public PlayerColor playerColor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PointBox"))
        {
            Renderer renderer = other.GetComponent<Renderer>();

            if (renderer == null)
            {
                renderer = other.GetComponentInChildren<Renderer>();
            }

            if (renderer != null)
            {
                renderer.material = playerColor.MaterialChange();
            }
            else
            {
                Debug.LogWarning($"{other.gameObject.name} 오브젝트에 Renderer가 없음!");
            }
        }
    }
}
