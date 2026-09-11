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

    // 着地処理中か
    private bool isLanding = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        playerHp = maxHp;
        attackPower = baseAttack;

        previousStatus = status;

        // AnimatorがInspectorで設定されていない場合の保険
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    void Update()
    {
        attackPower = baseAttack + speed * attackRate;
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

        // 攻撃中はRunで上書き禁止
        if ((status == PlayerStatus.Slide ||
             status == PlayerStatus.Punch ||
             status == PlayerStatus.Kick) &&
             newStatus == PlayerStatus.Run)
            return;

        // Jump / Fall中はRun禁止
        if ((status == PlayerStatus.Jump ||
             status == PlayerStatus.Fall) &&
             newStatus == PlayerStatus.Run)
            return;

        // Slide中はJump以外禁止
        if (status == PlayerStatus.Slide)
        {
            if (newStatus != PlayerStatus.Jump)
                return;
        }

        // ノックバック中は通常状態に変更しない
        if (status == PlayerStatus.KnockBack &&
            newStatus != PlayerStatus.Run &&
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
        playerHp -= damage;

        if (playerHp <= 0)
        {
            Debug.Log("死亡");
        }
    }

    // スピードの現在の段階
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
                animator.Play("wallRun");
                break;

            case PlayerStatus.Jump:
                // ジャンプ開始
                animator.Play("JumpStart");
                break;

            case PlayerStatus.Fall:
                // 空中
                animator.Play("JumpAir");
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

        // Jump / Fall以外での通常の接触なら何もしない
        if (status != PlayerStatus.Jump &&
            status != PlayerStatus.Fall)
            return;

        isGrounded = true;
        isLanding = true;

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
}