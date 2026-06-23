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
        // “¯‚¶ó‘Ô‚È‚ç‰½‚à‚µ‚È‚¢
        if (status == newStatus) return;

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