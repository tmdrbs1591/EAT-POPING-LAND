using UnityEngine;
using Photon.Pun;

public class BoomBullet : MonoBehaviourPun
{
    public Vector3 targetPoint;
    public float speed = 10f;
    public float explosionRadius = 3f;
    public float damage = 50f;
    public GameObject explosionEffect;

    private void Update()
    {
        Vector3 dir = (targetPoint - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPoint) < 0.5f)
        {
            Explode();
        }
    }

    void Explode()
    {
        photonView.RPC(nameof(ExplodeRPC), RpcTarget.All);
    }

    [PunRPC]
    void ExplodeRPC()
    {
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // 범위 내 적 탐색
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                var pv = hit.GetComponent<PhotonView>();
                if (pv != null && pv.ViewID != photonView.ViewID)
                {
                    pv.RPC("TakeDamage", RpcTarget.All, damage);
                }
            }
        }

        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Object"))
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    }
