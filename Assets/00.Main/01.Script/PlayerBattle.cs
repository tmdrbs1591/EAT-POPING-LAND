using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBattle : MonoBehaviourPun
{
    [Header("스텟")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float curHp;
    [SerializeField] private float maxHp;

    [Header("공격")]
    [SerializeField] private Vector3 attackBoxSize;
    [SerializeField] private Transform attackBoxPos;

    [Header("UI")]
    [SerializeField] GameObject charSprite;
    [SerializeField] Slider hpSlider;
    [SerializeField] ParticleSystem slasgPtc;

    private Rigidbody rb;
    private float inputX;
    private bool isFacingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        curHp = maxHp;
        hpSlider.value = curHp / maxHp;
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;

        inputX = Input.GetAxis("Horizontal");

        // 방향 전환 처리
        if (inputX > 0 && isFacingRight)
            Flip();
        else if (inputX < 0 && !isFacingRight)
            Flip();

    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;

        if (Input.GetKeyDown(KeyCode.X))
        {
            Damage();
            photonView.RPC("PtcOnRPC", RpcTarget.All);
        }


        Vector3 move = new Vector3(inputX, 0, 0) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }


    private void Flip()
    {
        photonView.RPC("FlipRPC", RpcTarget.All);
    }
    [PunRPC]
    private void FlipRPC()
    {
        isFacingRight = !isFacingRight;

        // 스프라이트 Y축으로 180도 회전
        charSprite.transform.Rotate(0f, 180f, 0f);

        // 공격 박스 위치 반전
        Vector3 attackPos = attackBoxPos.localPosition;
        attackPos.x *= -1;
        attackBoxPos.localPosition = attackPos;
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        curHp -= damage;
        hpSlider.value = curHp/maxHp;
    }
    [PunRPC]
    public void PtcOnRPC()
    {
        slasgPtc.Play();
    }
    private void Damage()
    {
        Collider[] colliders = Physics.OverlapBox(attackBoxPos.position, attackBoxSize / 2f);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                var otherPlayerScript = collider.GetComponent<PlayerBattle>();
                if (otherPlayerScript != null)
                    otherPlayerScript.photonView.RPC("TakeDamage", RpcTarget.All, 1f);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (attackBoxPos == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackBoxPos.position, attackBoxSize);
    }
}
