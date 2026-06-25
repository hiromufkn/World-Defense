using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerStatus
    {
        Idle,
        Run,
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

    [HideInInspector] public Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        attackPower = baseAttack;

        // ‰Šúó‘Ô•Û‘¶
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
        if ((status == PlayerStatus.Slide ||
             status == PlayerStatus.Punch ||
             status == PlayerStatus.Kick) &&
            (newStatus == PlayerStatus.Run ||
             newStatus == PlayerStatus.Idle))
            return;

        Debug.Log(
            "ó‘Ô•ÏX : " +
            status +
            " ¨ " +
            newStatus
        );

        previousStatus = status;
        status = newStatus;
    }
}