using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : MonoBehaviour
{
    private Dictionary<GameObject, float> damageTimers = new Dictionary<GameObject, float>();
    public float damageInterval = 1f; // 1초 간격
    public float damageAmount = 20f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!damageTimers.ContainsKey(other.gameObject))
            {
                damageTimers[other.gameObject] = 0f;
            }

            damageTimers[other.gameObject] += Time.deltaTime;

            if (damageTimers[other.gameObject] >= damageInterval)
            {
                var playerBattle = other.GetComponent<PlayerBattle>();
                if (playerBattle != null && playerBattle.photonView.IsMine)
                {
                    playerBattle.photonView.RPC("TakeDamage", RpcTarget.All, damageAmount);
                }

                damageTimers[other.gameObject] = 0f; // 타이머 초기화
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (damageTimers.ContainsKey(other.gameObject))
        {
            damageTimers.Remove(other.gameObject); // 영역 벗어나면 제거
        }
    }
}
