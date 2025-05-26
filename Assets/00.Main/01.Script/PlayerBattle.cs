using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Spine.Unity;
using System.Linq;
using TMPro;
using DG.Tweening.Core.Easing;
using JetBrains.Annotations;

public enum AttackType
{
    Melee,
    Distance,

}
public enum StatType
{
    Power,
    HP,
    Speed
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerBattle : MonoBehaviourPun
{
    [Header("스텟")]
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public float curHp;
    [SerializeField] public float maxHp;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] public float attackPower = 1f;
    [Header("공격")]
    [SerializeField] private Vector3 attackBoxSize;
    [SerializeField] private Transform attackBoxPos;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint; // 총알 발사 위치
    [SerializeField] private float bulletSpeed = 20f;

    [Header("방패")]
    [SerializeField] GameObject shield;
    [SerializeField] bool isShielding;
    [SerializeField] private float shieldCooldown = 1f;
    private float nextShieldTime = 0f;

    [Header("대시")]
    [SerializeField] float dashForce = 30f;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashDrag = 5f;
    [SerializeField] ParticleSystem dashPtc;
    private bool isDashing = false;
    private Vector3 dashDir;
    [SerializeField] private float dashCooldown = 1f;
    private float nextDashTime = 0f;

    [Header("UI")]
    [SerializeField] GameObject charSprite;
    [SerializeField] public Slider hpSlider;
    [SerializeField] ParticleSystem slashPtc;
    [SerializeField] ParticleSystem slashPtc2;
    [SerializeField] ParticleSystem diePtc;
    [SerializeField] ParticleSystem damagePtc;
    [SerializeField] GameObject dieCanvas;
    [SerializeField] public TMP_Text hptext;
    [SerializeField] GameObject damageText;
    [SerializeField] GameObject shieldText;
    [SerializeField] ParticleSystem moveEffect;
    [SerializeField] public TMP_Text[] attackPowerUIText;
    [SerializeField] public TMP_Text[] HPUIText;
    [SerializeField] public TMP_Text[] MoveSpeedUIText;


    [Header("공격 쿨타임")]
    [SerializeField] private float attackCooldown = 1f;
    private float nextAttackTime = 0f;
    [SerializeField] private float jumpCooldown = 1f;
    private float nextJumpTime = 0f;

    [Header("스파인")] //스파인 애니메이션을 위한것
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset[] animClip;

    public Animator meleeWeaponAnim;

    public AttackType attackType;


    [SerializeField] private List<GameObject> bushFalseOb;
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
    private bool useFirstAttack = true; // Toggle용 변수
    private bool useFirstSlash = true;

    // public Transform dieBattleCameraPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        Init();
    }
    public void Init()
    {
        curHp = maxHp;
        hpSlider.value = curHp / maxHp;
        isDie = false;
        rb.isKinematic = false; // 물리 효과 아예 끔
        BattleManager.instance.isPlayerDown = false;

    }

    void Update()
    {
        if (BattleManager.instance.isPlayerDown)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            //   rb.isKinematic = true; // 물리 효과 아예 끔

            if (photonView.IsMine)
            {
                Color color = skeletonAnimation.Skeleton.GetColor();
                color.a = 1.0f;
                skeletonAnimation.Skeleton.SetColor(color);

            }
            else
            {
                foreach (var bo in bushFalseOb)
                {
                    bo.SetActive(true);
                }
            }
        }

        if (!photonView.IsMine || BattleManager.instance.isPlayerDown)
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
        if (!photonView.IsMine || BattleManager.instance.isPlayerDown)
            return;

        Die();
        ShieldOn();
        if (Input.GetMouseButton(0) && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;

            if (attackType == AttackType.Melee)
                Attack();
            else if (WeaponManager.instance.currentWeaponType == WeaponType.BubbleGun)
                BubbleGunFire();
            else if (WeaponManager.instance.currentWeaponType == WeaponType.BoomGun)
                BoomGunFire();
        }

        Vector3 moveDir = new Vector3(inputX, 0, inputZ);
        if (moveDir.magnitude < 0.1f)
        {
            rb.velocity = Vector3.zero;

            if (animState != AnimState.Attack)
            {
                photonView.RPC("SetAnimStateRPC", RpcTarget.All, (int)AnimState.Idle);

                if (moveEffect != null && moveEffect.isPlaying)
                    photonView.RPC(nameof(MoveEffectStopRPC), RpcTarget.All);
            }
        }
        else
        {
            moveDir.Normalize();
            rb.velocity = moveDir * moveSpeed;

            if (animState != AnimState.Attack)
            {
                photonView.RPC("SetAnimStateRPC", RpcTarget.All, (int)AnimState.Walk);

                if (moveEffect != null && !moveEffect.isPlaying)
                    photonView.RPC(nameof(MoveEffectStartRPC), RpcTarget.All);
            }
        }


        if (Input.GetKey(KeyCode.LeftShift) && Time.time >= nextDashTime)
        {
            nextDashTime = Time.time + dashCooldown;
            Dash();
        }

    }

    #region 무브
    [PunRPC]
    void MoveEffectStartRPC()
    {
        moveEffect.Play();
    }
    [PunRPC]
    void MoveEffectStopRPC()
    {
        moveEffect.Stop();
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


    #endregion
    #region 근거리 공격
    public void Attack()
    {
        StartCoroutine(AttackCor());
    }
    IEnumerator AttackCor()
    {
        photonView.RPC("WeaponAnimationRPC", RpcTarget.All);
        photonView.RPC("SlashPtcOnRPC", RpcTarget.All);
        StartCoroutine(BackToIdleAfterAttack(0.2f));
        photonView.RPC("SetAnimStateRPC", RpcTarget.All, (int)AnimState.Attack);

        yield return new WaitForSeconds(0.05f);
        Damage();
    }
    [PunRPC]
    void WeaponAnimationRPC()// 근접 애니메이션 동기화
    {
        // 애니메이션 트리거 번갈아 실행
        string triggerName = useFirstAttack ? "Attack1" : "Attack2";
        meleeWeaponAnim.SetTrigger(triggerName);

        // 다음 공격 때 바뀌도록 토글
        useFirstAttack = !useFirstAttack;
    }
    [PunRPC]
    public void SlashPtcOnRPC()
    {
        AudioManager.instance.PlaySound(transform.position, 12, Random.Range(1f, 1.2f), 1f);
        if (useFirstSlash)
        {
            slashPtc.Play();
        }
        else
        {
            slashPtc2.Play();
        }

        useFirstSlash = !useFirstSlash; // 다음 호출 때 번갈아
    }
    #endregion
    #region 원거리 공격
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
    #endregion
    #region 데미지관련
    [PunRPC]
    public void TakeDamage(float damage)
    {
        if (BattleManager.instance.isPlayerDown)
            return;
        if (!isShielding)
        {
            AudioManager.instance.PlaySound(transform.position, 6, Random.Range(1f, 1f), 1f);

            damagePtc.Play();
            CameraShake.instance.Shake(0.7f, 0.1f);
            curHp -= damage;
            hpSlider.value = curHp / maxHp;
            hptext.text = curHp.ToString() + "/" + maxHp.ToString();

            Vector3 randomOffset = new Vector3(
             Random.Range(-1f, 2f),   // X 범위
             Random.Range(2f, 3f),    // Y를 위쪽으로 띄운 범위
             -0.2f                      // Z 고정
         );

            var tmp = Instantiate(damageText, transform.position + randomOffset, transform.rotation).GetComponent<TMP_Text>();
            tmp.text = damage.ToString();
            Destroy(tmp, 2f);

        }
        else
        {
            AudioManager.instance.PlaySound(transform.position, 11, Random.Range(1f, 1f), 1f);
            Vector3 randomOffset = new Vector3(
          Random.Range(-1f, 2f),   // X 범위
          Random.Range(2f, 3f),    // Y를 위쪽으로 띄운 범위
          -0.2f                      // Z 고정
      );
            CameraShake.instance.Shake(0.7f, 0.1f);
            Destroy(Instantiate(shieldText, transform.position + randomOffset, transform.rotation), 2f);
        }
    }
    void Die()
    {
        if (isDie)
            return;
        if (curHp <= 0)
        {
            isDie = true;
            photonView.RPC(nameof(PlayerDownRPC), RpcTarget.All);
            photonView.RPC("DiePtcOnRPC", RpcTarget.All);
            photonView.RPC("TimeSlowRPC", RpcTarget.All);
            StartCoroutine(BattleLoseCor());


        }
    }


    [PunRPC]
    public void DiePtcOnRPC()
    {
        diePtc.Play();
        dieCanvas.SetActive(true);
        // BattleManager.instance.battleCamera.transform.DOMove(dieBattleCameraPos.position, 1f).SetEase(Ease.InOutSine);

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
        BattleManager.instance.photonView.RPC("ResetPosPlayerRPC", RpcTarget.All);

        Debug.Log("제ㅔㅁㄴㅇㅁㄴㅇㅁㄴㅇㅁㄴㅇㅁㄴㅇㅁㄴㅇㅁㄴㅇ");
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
                    otherPlayerScript.photonView.RPC("TakeDamage", RpcTarget.All, 10 * attackPower);
            }
        }
    }
    [PunRPC]
    public void BattleEndPanel()
    {

        BattleManager.instance.battleEndPanel.SetActive(false);
        BattleManager.instance.battleEndPanel.SetActive(true);

    }
    #endregion
    #region 버블건

    public int bulletCountPerShot = 5;
    public float spreadAngle = 30f;
    public float shotInterval = 0.1f;

    public void BubbleGunFire()
    {
        StartCoroutine(RandomOrderFanFire());
    }

    IEnumerator RandomOrderFanFire()
    {
        var battleCam = BattleManager.instance.battleCamera.GetComponent<Camera>();
        Ray ray = battleCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Vector3 targetPoint = hitInfo.point;
            Vector3 baseDirection = (targetPoint - firePoint.position).normalized;

            CameraShake.instance.Shake(0.5f, 0.1f);

            // 부채꼴 방향 미리 계산
            float angleStep = spreadAngle / (bulletCountPerShot - 1);
            float startAngle = -spreadAngle / 2f;
            List<Vector3> directions = new List<Vector3>();

            for (int i = 0; i < bulletCountPerShot; i++)
            {
                float angle = startAngle + angleStep * i;
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
                directions.Add(rotation * baseDirection);
            }

            // 순서를 랜덤하게 섞음
            directions = directions.OrderBy(x => Random.value).ToList();

            // 하나씩 시간차 발사
            foreach (var direction in directions)
            {
                FireSingleBullet(direction);
                yield return new WaitForSeconds(shotInterval);
            }
        }
    }

    void FireSingleBullet(Vector3 direction)
    {
        // 회전 유지: firePoint.rotation 사용
        GameObject bullet = PhotonNetwork.Instantiate(
            "BubbleBullet",
            firePoint.position,
            firePoint.rotation
        );
        AudioRPC(10);
        var bulletScript = bullet.GetComponent<PlayerBullet>();
        if (bulletScript != null)
        {
            bulletScript.shooterViewID = photonView.ViewID;
            bulletScript.damage *= attackPower;
        }

        Physics.IgnoreCollision(bullet.GetComponent<Collider>(), GetComponent<Collider>());

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = direction * bulletSpeed;
    }


    #endregion
    #region 폭탄총
    private void BoomGunFire()
    {
        var battleCam = BattleManager.instance.battleCamera.GetComponent<Camera>();
        Ray ray = battleCam.ScreenPointToRay(Input.mousePosition);

        CameraShake.instance.Shake(0.5f, 0.1f);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Vector3 targetPoint = hitInfo.point;

            object[] instantiationData = new object[] { targetPoint.x, targetPoint.y, targetPoint.z };

            GameObject bombObj = PhotonNetwork.Instantiate("BoomBullet", firePoint.position, firePoint.rotation, 0, instantiationData);
            var boomScript = bombObj.GetComponent<BoomBullet>();
            boomScript.damage *= attackPower;

            // 자기 자신과 충돌 무시
            Physics.IgnoreCollision(bombObj.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }

    #endregion
    #region 쉴드
    public void ShieldOn()
    {
        if (Input.GetMouseButton(1) && Time.time >= nextShieldTime)
        {
            nextShieldTime = Time.time + shieldCooldown;
            photonView.RPC(nameof(ShieldRPC), RpcTarget.All);
        }
    }

    [PunRPC]
    void ShieldRPC()
    {
        StartCoroutine(ShieldCor());

    }
    IEnumerator ShieldCor()
    {
        photonView.RPC(nameof(SetShieldState), RpcTarget.All, true);
        yield return new WaitForSeconds(3f);
        photonView.RPC(nameof(SetShieldState), RpcTarget.All, false);
    }
    [PunRPC]
    void SetShieldState(bool state)
    {
        isShielding = state;
        shield.gameObject.SetActive(state);
    }


    #endregion
    #region 대쉬
    public void Dash()
    {
        if (isDashing) return;

        dashDir = new Vector3(inputX, 0f, inputZ).normalized;
        if (dashDir == Vector3.zero)
            dashDir = transform.forward;

        photonView.RPC(nameof(DashRPC), RpcTarget.All);
        StartCoroutine(DashCoroutine());
    }
    [PunRPC]
    void DashRPC()
    {
        dashPtc.Play();
    }
    IEnumerator DashCoroutine()
    {
        isDashing = true;

        float originalDrag = rb.drag;
        rb.drag = dashDrag;

        float timer = 0f;
        while (timer < dashDuration)
        {
            rb.AddForce(dashDir * dashForce, ForceMode.Acceleration);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.drag = originalDrag;
        isDashing = false;
    }
    #endregion
    #region 강화
    public void UpgradeStat(StatType stat)
    {
        // 로컬에서 업그레이드 시작 (호스트가 호출한다고 가정)
        photonView.RPC("RPC_UpgradeStat", RpcTarget.All, (int)stat);
    }

    [PunRPC]
    public void RPC_UpgradeStat(int statValue)
    {
        StatType stat = (StatType)statValue;

        switch (stat)
        {
            case StatType.Power:
                attackPower += 0.2f;
                foreach (var Text in attackPowerUIText)
                {
                    Text.text = attackPower.ToString() + "X";
                }
                break;
            case StatType.HP:
                maxHp += 10;
                foreach (var Text in HPUIText)
                {
                    Text.text = maxHp.ToString();
                }
                break;
            case StatType.Speed:
                moveSpeed += 0.2f;
                foreach (var Text in MoveSpeedUIText)
                {
                    Text.text = moveSpeed.ToString();
                }
                break;
        }
    }
    #endregion

    [PunRPC]
    public void SetAnimStateRPC(int state)
    {
        animState = (AnimState)state;
        SetCurrentAnimation(animState);
    }

    [PunRPC]
    void PlayerDownRPC()
    {
        BattleManager.instance.isPlayerDown = true;

    }

    [PunRPC]
    public void DieCanvasFalseRPC()
    {
        dieCanvas.SetActive(false);
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
                AsncAnimation(animClip[(int)AnimState.Attack], true, 1.25f);
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

    public void AudioRPC(int index)
    {
        photonView.RPC("AudioRealRPC", RpcTarget.All, index);
    }

    [PunRPC]
    public void AudioRealRPC(int index)
    {
        AudioManager.instance.PlaySound(transform.position, index, Random.Range(1f, 1.2f), 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bush"))
        {
            if (photonView.IsMine)
            {
                Color color = skeletonAnimation.Skeleton.GetColor();
                color.a = 0.5f; 
                skeletonAnimation.Skeleton.SetColor(color);
            }
            else
            {
                foreach (var bo in bushFalseOb)
                {
                    bo.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bush"))
        {
            if (photonView.IsMine)
            {
                Color color = skeletonAnimation.Skeleton.GetColor();
                color.a = 1.0f; 
                skeletonAnimation.Skeleton.SetColor(color);

            }
            else
            {
                foreach (var bo in bushFalseOb)
                {
                    bo.SetActive(true);
                }
            }
        }
    }
}
