using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public enum PlayerStatus
    {
        Idle,
        Run,
        WallRun,
        Jump,
        Fall,
        Punch,
        Kick,
        Slide,
        Dash,
        Damage,
        KnockBack,
        Dead
    }

    public PlayerStatus status = PlayerStatus.Idle;

    private PlayerStatus previousStatus;

    [Header("Speed")]
    public float speed = 0f;
    public float maxSpeed = 30f;
    public float acceleration = 5f;
    public float deceleration = 8f;
    public float brakePower = 35f;
    public float turnSpeedThreshold = 15f;

    [Header("Speed Level")]
    public float lowSpeed = 10f;
    public float midSpeed = 20f;
    public float highSpeed = 30f;

    [Header("Move")]
    public float moveSpeed = 10f;

    [Header("Jump")]
    public float jumpPower = 16f;
    public bool isGrounded = true;

    [Header("Attack")]
    public float baseAttack = 10f;
    public float attackPower;
    public float attackRate = 0.5f;
    public bool isAttack = false;

    [Header("Status")]
    public float maxHp = 100f;
    public float playerHp;

    [HideInInspector]
    public Rigidbody rb;

    public Animator animator;
    private PlayerMove playerMove;

    // 着地処理中か
    private bool isLanding = false;
    public bool IsLanding => isLanding;

    [SerializeField] private GameObject gameOverUI;

    // 死亡エフェクト
    [SerializeField] private GameObject deathEffect;

    [SerializeField] private float fallGravity = 2.5f;
    private Vector3 StartPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.AddForce(
                Physics.gravity * (fallGravity - 1f),
                ForceMode.Acceleration
            );
        }
    }

    void Start()
    {
       StartPos=new Vector3(50f, 0f, 8f);

        playerHp = maxHp;
        attackPower = baseAttack;

        previousStatus = status;

        // AnimatorがInspectorで設定されていない場合の保険
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        // playermoveのスクリプトの取得
        playerMove = GetComponent<PlayerMove>();
    }

    void Update()
    {
        attackPower = baseAttack + speed * attackRate;

        Fall();
    }

    public void ChangeStatus(PlayerStatus newStatus)
    {
        // 同じ状態なら無視
        if (status == newStatus) return;

        // 死亡中は何も上書きできない
        if (status == PlayerStatus.Dead) return;

        // ダメージ中も優先
        if (status == PlayerStatus.Damage &&
            newStatus != PlayerStatus.Dead)
            return;

        //// 攻撃中はRunで上書き禁止
        //if ((status == PlayerStatus.Slide ||
        //     status == PlayerStatus.Punch ||
        //     status == PlayerStatus.Kick) &&
        //     newStatus == PlayerStatus.Run)
        //    return;

        // Jump / Fall中はRun禁止
        if ((status == PlayerStatus.Jump ||
             status == PlayerStatus.Fall) &&
             newStatus == PlayerStatus.Run)
            return;

        // Slide中はJump以外禁止
        if (status == PlayerStatus.Slide)
        {
            if (newStatus != PlayerStatus.Jump &&
                newStatus != PlayerStatus.Run)
                return;
        }

        // ノックバック中は通常状態に変更しない
        if (status == PlayerStatus.KnockBack &&
            newStatus != PlayerStatus.Run &&
            newStatus != PlayerStatus.Idle &&
            newStatus != PlayerStatus.Dead)
            return;

        Debug.Log(
            "状態変更 : " +
            status +
            " → " +
            newStatus
        );

        previousStatus = status;
        status = newStatus;

        UpdateAnimation();
    }

    public void TakeDamage(float damage = 1f)
    {
        // すでに死亡していたら何もしない
        if (status == PlayerStatus.Dead)
            return;

        playerHp -= damage;

        if (playerHp <= 0)
        {
            playerHp = 0;
            Debug.Log("死亡");

            // 死亡エフェクト
            if (deathEffect != null)
            {
                Instantiate(
                    deathEffect,
                    transform.position,
                    Quaternion.identity
                );
            }

            ChangeStatus(PlayerStatus.Dead);

            if (gameOverUI!=null)
            {
                gameOverUI.SetActive(true);
            }
        }
    }

    // 回復処理
    public void Heal(float heal)
    {
        playerHp += heal;
        Debug.Log("回復");
        if (playerHp >= 100)
        {
            playerHp = 100;
            Debug.Log("FullHP");
        }
    }

    // スピードの現在の段階
    public bool IsLowSpeed()
    {
        return speed <= lowSpeed;
    }
    public bool IsMidSpeed()
    {
        return speed >= midSpeed;
    }

    public bool IsHighSpeed()
    {
        return speed >= highSpeed;
    }

    //========================================
    // アニメーション
    //========================================

    private void UpdateAnimation()
    {
        if (animator == null)
            return;

        switch (status)
        {
            case PlayerStatus.Idle:
                animator.Play("idle");
                break;

            case PlayerStatus.Run:
                animator.Play("run");
                break;

            case PlayerStatus.WallRun:
                animator.Play("run");
                break;

            case PlayerStatus.Jump:
                // ジャンプ開始
                animator.Play("JumpStart");
                break;

            case PlayerStatus.Fall:
                animator.Play("Fall");
                break;

            case PlayerStatus.Slide:
                animator.Play("slide");
                break;

            case PlayerStatus.Damage:
                animator.Play("damage");
                break;

            case PlayerStatus.KnockBack:
                animator.Play("knockBack");
                break;

            case PlayerStatus.Dead:
                animator.Play("dead");

                break;
        }
    }

    //========================================
    // 着地
    //========================================

    public void Land()
    {
        // すでに着地処理中なら何もしない
        if (isLanding)
            return;

        // Jump / Fall / KnockBack以外での通常の接触なら何もしない
        if (status != PlayerStatus.Jump &&
            status != PlayerStatus.Fall &&
            status != PlayerStatus.KnockBack)
            return;

        isGrounded = true;
        isLanding = true;

        // 着地時のスピード管理
        if(playerMove != null)
        {
            playerMove.LandingSpeed();
        }

        // 着地アニメーション
        if (animator != null)
        {
            animator.Play("JumpEnd");
        }

        StartCoroutine(LandAnimationCoroutine());
    }

    private IEnumerator LandAnimationCoroutine()
    {
        // JumpEndのアニメーションを取得
        yield return null;

        float landLength = 0.27f;

        if (animator != null)
        {
            AnimatorStateInfo stateInfo =
                animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("JumpEnd"))
            {
                landLength = stateInfo.length;
            }
        }

        // 着地アニメーションが終わるまで待つ
        yield return new WaitForSeconds(landLength);

        isLanding = false;

        // Idleへ戻る
        ChangeStatus(PlayerStatus.Idle);
    }

    private void Fall()
    {
        if (transform.position.y < -10f)
        {
            transform.position = StartPos;

            rb.linearVelocity = Vector3.zero;
        }
    }
}