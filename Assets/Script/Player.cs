using UnityEngine;

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

    [Header("Speed Level")]
    public float lowSpeed = 10f;
    public float midSpeed = 20f;
    public float highSpeed = 30f;

    [Header("Move")]
    public float moveSpeed = 10f;

    [Header("Jump")]
    public float jumpPower = 8f;
    public bool isGrounded = true;

    [Header("Attack")]
    public float baseAttack = 10f;
    public float attackPower;
    public float attackRate = 0.5f;
    public bool isAttack = false;

    [Header("Status")]
    public float maxHp = 100f;
    public float playerHp;

    [HideInInspector] public Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        playerHp = maxHp;
        attackPower = baseAttack;

        previousStatus = status;
    }

    void Update()
    {
        attackPower = baseAttack + speed * attackRate;
    }

    public void ChangeStatus(PlayerStatus newStatus)
    {
        // “¯‚¶ó‘Ô‚È‚ç–³‹
        if (status == newStatus) return;

        // €–S’†‚Í‰½‚àã‘‚«‚Å‚«‚È‚¢
        if (status == PlayerStatus.Dead) return;

        // ƒ_ƒ[ƒW’†‚à—Dæ
        if (status == PlayerStatus.Damage &&
            newStatus != PlayerStatus.Dead)
            return;

        // UŒ‚’†‚ÍRun‚Åã‘‚«‹Ö~
        if ((status == PlayerStatus.Slide ||
             status == PlayerStatus.Punch ||
             status == PlayerStatus.Kick) &&
             newStatus == PlayerStatus.Run)
            return;

        // Jump’†‚ÍRun‹Ö~
        if ((status == PlayerStatus.Jump ||
             status == PlayerStatus.Fall) &&
             newStatus == PlayerStatus.Run)
            return;

        // UŒ‚’†‚ÍIdle‚Å‚àã‘‚«‹Ö~
        // Slide’†‚ÍJumpˆÈŠO‹Ö~
        if (status == PlayerStatus.Slide)
        {
            if (newStatus != PlayerStatus.Jump)
                return;
        }

        Debug.Log(
            "ó‘Ô•ÏX : " +
            status +
            " ¨ " +
            newStatus
        );

        previousStatus = status;
        status = newStatus;
    }
    public void TakeDamage(float damage = 1f)
    {
        playerHp -= damage;

        if (playerHp <= 0)
        {
            Debug.Log("€–S");
        }
    }

    // ƒXƒs[ƒh‚ÌŒ»İ‚Ì’iŠK
    public bool IsMidSpeed()
    {
        return speed >= midSpeed;
    }

    public bool IsHighSpeed()
    {
        return speed >= highSpeed;
    }
}