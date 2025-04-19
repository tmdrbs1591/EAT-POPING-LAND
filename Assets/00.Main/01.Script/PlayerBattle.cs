using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBattle : MonoBehaviourPun
{
    [Header("����")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float curHp;
    [SerializeField] private float maxHp;

    [Header("����")]
    [SerializeField] private Vector3 attackBoxSize;
    [SerializeField] private Transform attackBoxPos;

    [Header("UI")]
    [SerializeField] GameObject charSprite;
    [SerializeField] Slider hpSlider;
    [SerializeField] ParticleSystem slasgPtc;

    [Header("���� ��Ÿ��")]
    [SerializeField] private float attackCooldown = 1f;
    private float nextAttackTime = 0f;

    private Rigidbody rb;
    private float inputX;
    private bool isFacingRight = true;
        
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Init();
    }
    public void Init()
    {
        curHp = maxHp;
        hpSlider.value = curHp / maxHp;
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;

        inputX = Input.GetAxis("Horizontal");


        // ���� ��ȯ ó��
        if (inputX > 0 && isFacingRight)
            Flip();
        else if (inputX < 0 && !isFacingRight)
            Flip();

    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;

        Die();

        if (Input.GetMouseButton(0) && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            Damage();
            photonView.RPC("PtcOnRPC", RpcTarget.All);
        }

        Vector3 move = new Vector3(inputX, 0, 0) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }

    void Die()
    {
        if(curHp <= 0)
        {
           BattleManager.instance.BattleLose();
        }
    }
    private void Flip()
    {
        photonView.RPC("FlipRPC", RpcTarget.All);
    }
[PunRPC]
private void FlipRPC()
{
    isFacingRight = !isFacingRight;

    // �ٶ󺸴� ���⿡ ���� ��Ȯ�� ȸ���� ����
    if (isFacingRight)
        charSprite.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    else
        charSprite.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

    // ���� �ڽ� ��ġ ����
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
