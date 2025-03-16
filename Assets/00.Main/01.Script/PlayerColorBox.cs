using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColorBox : MonoBehaviourPunCallbacks
{
    public PlayerColor playerColorScript;

    public GameObject battleUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PointBox"))
        {
            Renderer renderer = other.GetComponent<Renderer>();
            Hold hold = other.GetComponent<Hold>();

            if (renderer == null)
            {
                renderer = other.GetComponentInChildren<Renderer>();
            }

            if (renderer != null)
            {
                if (hold.holdType == PlayerColorType.Default)
                {
                    renderer.material = playerColorScript.MaterialChange();
                    hold.holdType = playerColorScript.HoldChange();
                }

                if(hold.holdType != playerColorScript.playerColor)
                {
                    if (photonView.IsMine)
                    {
                        battleUI.SetActive(true);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"{other.gameObject.name} 오브젝트에 Renderer가 없음!");
            }
        }
    }
}