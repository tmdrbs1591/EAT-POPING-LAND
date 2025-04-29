using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerBullet : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject destroyPtc;

    public int shooterViewID = -1; // 총알 만든 사람의 ViewID
    public Collider collider;

    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Object"))
        {
            Instantiate(destroyPtc, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        if (other.CompareTag("Player"))
        {
            var target = other.GetComponent<PhotonView>();
            if (target != null && target.ViewID != shooterViewID)
            {
                if (photonView.IsMine)
                {
                    var otherPlayerScript = target.GetComponent<PlayerBattle>();
                    if (otherPlayerScript != null)
                        otherPlayerScript.photonView.RPC("TakeDamage", RpcTarget.All, 1f);
                         photonView.RPC("DestroyRPC", RpcTarget.All);
                }
            }
        }
    }

    [PunRPC]
    void DestroyRPC()
    {
        Destroy(gameObject);
    }

}
