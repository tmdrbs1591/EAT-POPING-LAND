using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using DG.Tweening;

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
    [SerializeField] public Slider hpSlider;
    [SerializeField] ParticleSystem slashPtc;
    [SerializeField] ParticleSystem diePtc;
    [SerializeField] GameObject dieCanvas;

    [Header("공격 쿨타임")]
    [SerializeField] private float attackCooldown = 1f;
    private float nextAttackTime = 0f;

    private Rigidbody rb;
    private float inputX;
    private bool isFacingRight = true;
    private bool isDie;
        
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Init();
    }
    public void Init()
    {
        curHp = maxHp;
        hpSlider.value = curHp / maxHp;
        isDie = false;
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

        Die();

        if (Input.GetMouseButton(0) && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            Damage();
            photonView.RPC("SlashPtcOnRPC", RpcTarget.All);
        }

        Vector3 move = new Vector3(inputX, 0, 0) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }

    void Die()
    {
        if (isDie)
            return;
        if (curHp <= 0)
        {
            isDie = true;
            photonView.RPC("DiePtcOnRPC", RpcTarget.All);
            photonView.RPC("TimeSlowRPC", RpcTarget.All);
            StartCoroutine(BattleLoseCor());
        }
    }

    [PunRPC]
    void TimeSlowRPC()
    {
        StartCoroutine(TimeSlowCor());
    }
    IEnumerator TimeSlowCor()
    {
        CameraShake.instance.Shake(0.4f, 0.4f);

        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(5f);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }


    IEnumerator BattleLoseCor()
    {
        BattleManager.instance.BattleLose();
        yield return new WaitForSecondsRealtime(5f);
        BattleManager.instance.photonView.RPC("RPC_BattlePanelFalse", RpcTarget.All);
        BattleManager.instance.photonView.RPC("ResetPosPlayerRPC",RpcTarget.All);


    }
    private void Flip()
    {
        photonView.RPC("FlipRPC", RpcTarget.All);
    }
[PunRPC]
private void FlipRPC()
{
    isFacingRight = !isFacingRight;

    // 바라보는 방향에 따라 정확한 회전값 설정
    if (isFacingRight)
        charSprite.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    else
        charSprite.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

    // 공격 박스 위치 반전
    Vector3 attackPos = attackBoxPos.localPosition;
    attackPos.x *= -1;
    attackBoxPos.localPosition = attackPos;
}


    [PunRPC]
    public void TakeDamage(float damage)
    {
        CameraShake.instance.Shake(0.3f, 0.1f);
        curHp -= damage;
        hpSlider.value = curHp/maxHp;
        
    }
    [PunRPC]
    public void SlashPtcOnRPC()
    {
        slashPtc.Play();
    }
    [PunRPC]
    public void DiePtcOnRPC()
    {
        diePtc.Play();
        dieCanvas.SetActive(true);
    }

    [PunRPC]
    public void DieCanvasFalseRPC()
    {
        dieCanvas.SetActive(false);
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
