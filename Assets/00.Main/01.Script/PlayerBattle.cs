using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Spine.Unity;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBattle : MonoBehaviourPun
{
    [Header("스텟")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float curHp;
    [SerializeField] private float maxHp;
    [SerializeField] private float jumpForce = 7f;
    [Header("공격")]
    [SerializeField] private Vector3 attackBoxSize;
    [SerializeField] private Transform attackBoxPos;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint; // 총알 발사 위치
    [SerializeField] private float bulletSpeed = 20f;

    [Header("UI")]
    [SerializeField] GameObject charSprite;
    [SerializeField] public Slider hpSlider;
    [SerializeField] ParticleSystem slashPtc;
    [SerializeField] ParticleSystem diePtc;
    [SerializeField] ParticleSystem damagePtc;
    [SerializeField] GameObject dieCanvas;

    [Header("공격 쿨타임")]
    [SerializeField] private float attackCooldown = 1f;
    private float nextAttackTime = 0f;
    [SerializeField] private float jumpCooldown = 1f;
    private float nextJumpTime = 0f;

    [Header("스파인")] //스파인 애니메이션을 위한것
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset[] animClip;

    public Animator meleeWeaponAnim;

    public enum AnimState
    {
        Idle,
        Walk,
        Attack
    }
    public AnimState animState; // 현재 애니메이션 처리가 무엇인가에 대한 변수
    private string currentAnimation; //현재 어떤 애니메이션이 재생되고 있는가에 대한 변수

    private Rigidbody rb;
    private float inputX;
    private float inputZ; // z축 입력

    private bool isFacingRight = true;
    private bool isDie;

    // public Transform dieBattleCameraPos;

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
        inputZ = Input.GetAxis("Vertical"); // z축 입력 추가

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

            Attack();
            //   Fire();
        }



        //if (Input.GetKey(KeyCode.Space) && Time.time >= nextJumpTime)
        //{
        //    nextJumpTime = Time.time + jumpCooldown;
        //    Jump();
        //}

        Vector3 moveDir = new Vector3(inputX, 0, inputZ);
        if (moveDir.magnitude < 0.1f) // 거의 0에 가까우면
        {
            rb.velocity = Vector3.zero;
            if (animState != AnimState.Attack)
                photonView.RPC("SetAnimStateRPC", RpcTarget.All, (int)AnimState.Idle);
        }
        else
        {
            moveDir.Normalize();
            rb.velocity = moveDir * moveSpeed;
            if (animState != AnimState.Attack)
                photonView.RPC("SetAnimStateRPC", RpcTarget.All, (int)AnimState.Walk);
        }


    }

    public void Attack()
    {
        StartCoroutine(AttackCor());
    }
    IEnumerator  AttackCor()
    {
        meleeWeaponAnim.SetTrigger("Attack");
        photonView.RPC("SlashPtcOnRPC", RpcTarget.All);
        StartCoroutine(BackToIdleAfterAttack(0.2f));
        photonView.RPC("SetAnimStateRPC", RpcTarget.All, (int)AnimState.Attack);
        yield return new WaitForSeconds(0.2f);
        Damage();
    }

    [PunRPC]
    void SetAnimStateRPC(int state)
    {
        animState = (AnimState)state;
        SetCurrentAnimation(animState);
    }
    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Y속도 초기화
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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
        yield return new WaitForSecondsRealtime(5.5f);
        photonView.RPC("BattleEndPanel", RpcTarget.All);
        yield return new WaitForSecondsRealtime(0.5f);
        BattleManager.instance.photonView.RPC("RPC_BattlePanelFalse", RpcTarget.All);
        BattleManager.instance.photonView.RPC("ResetPosPlayerRPC",RpcTarget.All);


    }

    [PunRPC]
    public void BattleEndPanel() {

        BattleManager.instance.battleEndPanel.SetActive(false);
        BattleManager.instance.battleEndPanel.SetActive(true);
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
        charSprite.transform.rotation = Quaternion.Euler(40f, 0f, 0f);
    else
        charSprite.transform.rotation = Quaternion.Euler(-40f, 180f, 0f);

    // 공격 박스 위치 반전
    Vector3 attackPos = attackBoxPos.localPosition;
    attackPos.x *= -1;
    attackBoxPos.localPosition = attackPos;
}


    [PunRPC]
    public void TakeDamage(float damage)
    {

        AudioManager.instance.PlaySound(transform.position, 6, Random.Range(1f, 1f), 1f);

        damagePtc.Play();
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
        // BattleManager.instance.battleCamera.transform.DOMove(dieBattleCameraPos.position, 1f).SetEase(Ease.InOutSine);

    }

    [PunRPC]
    public void DieCanvasFalseRPC()
    {
        dieCanvas.SetActive(false);
    }
    private void Fire()
    {
        var battleCam = BattleManager.instance.battleCamera.GetComponent<Camera>();
        // 1. 마우스 스크린 위치에서 Ray 쏘기
        Ray ray = battleCam.ScreenPointToRay(Input.mousePosition);

        CameraShake.instance.Shake(0.5f, 0.1f);

        // 2. Ray를 쏴서 어디를 향할지 결정
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            // 목표 지점
            Vector3 targetPoint = hitInfo.point;

            // 3. 방향 계산
            Vector3 direction = (targetPoint - firePoint.position).normalized;
            GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, transform.rotation);

            var bulletScript = bullet.GetComponent<PlayerBullet>();
            if (bulletScript != null)
            {
                bulletScript.shooterViewID = photonView.ViewID;
            }

            // 자기 자신과 충돌 무시
            Physics.IgnoreCollision(bullet.GetComponent<Collider>(), GetComponent<Collider>());

            // 5. 총알에 힘 주기
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = direction * bulletSpeed;


        }
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
    private void AsncAnimation(AnimationReferenceAsset animClip, bool loop, float timeScale)
    {
        if (animClip.name.Equals(currentAnimation)) // 동일한 애니메이션을 재생하려 한다면 아래코드구문 실행 X 
            return;

        skeletonAnimation.state.SetAnimation(0, animClip, loop).TimeScale = timeScale; // 해당 애니메이션으로 변경한다
        skeletonAnimation.loop = loop;
        skeletonAnimation.timeScale = timeScale;


        currentAnimation = animClip.name; // 현재 재생되고 있는 애니메이션 값을 변경 
    }

    private void SetCurrentAnimation(AnimState _state)
    {
        switch (_state)
        {
            case AnimState.Idle:
                AsncAnimation(animClip[(int)AnimState.Idle], true, 1f);
                break;
            case AnimState.Walk:
                AsncAnimation(animClip[(int)AnimState.Walk], true, 1.4f);
                break;
            case AnimState.Attack:
                AsncAnimation(animClip[(int)AnimState.Attack], true, 1.4f);
                break;
        }
    }
    IEnumerator BackToIdleAfterAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (animState == AnimState.Attack) // 여전히 공격 중이면 Idle로
            photonView.RPC("SetAnimStateRPC", RpcTarget.All, (int)AnimState.Idle);
    }

  
    private void OnDrawGizmos()
    {
        if (attackBoxPos == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackBoxPos.position, attackBoxSize);
    }
}
