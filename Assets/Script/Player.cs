using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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

    private Rigidbody rb;

    // スピード
    public float speed = 0f;
    public float maxSpeed = 30f;
    public float acceleration = 5f;

    // 横移動速度
    public float moveSpeed = 10f;

    // ジャンプ
    public float jumpPower = 8f;
    public bool isGrounded = true;

    // 攻撃
    public float baseAttack = 10f;
    public float attackPower;

    // 速度倍率
    public float attackRate = 0.5f;

    // 入力
    private Vector2 moveInput;

    [SerializeField] private Transform cameraTransform;

    [SerializeField] private CameraController cameraController;

    void Start()
    {
        attackPower = baseAttack;

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        PlayerRun();

        attackPower = baseAttack + speed * attackRate;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Debug.Log("OnMove呼ばれた：" + moveInput);
    }

    public void OnLook(InputValue value)
    {
        cameraController.SetLookInput(value.Get<Vector2>());
    }

    public void OnJump()
    {
        Jump();
        Debug.Log("OnJump呼ばれた");
    }

    public void OnSlide()
    {
        Slide();
        Debug.Log("OnSlide呼ばれた");
    }

    // 移動処理
    public void PlayerRun()
    {
        float horizontal = moveInput.x;
        float vertical = moveInput.y;

        // カメラ基準の前方向
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // 上下の傾きを消す
        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // 入力方向
        Vector3 moveDirection =
            cameraForward * vertical +
            cameraRight * horizontal;

        if (moveDirection.magnitude > 0)
        {
            status = PlayerStatus.Run;

            // 加速
            speed += acceleration * Time.deltaTime;
            speed = Mathf.Clamp(speed, 0, maxSpeed);

            // プレイヤーを進行方向へ向ける
            transform.forward = moveDirection.normalized;

            // 移動
            transform.Translate(
                Vector3.forward * speed * Time.deltaTime
            );
        }
    }

    public void Jump()
    {
        if (!isGrounded) return;

        status = PlayerStatus.Jump;

        // 上方向へジャンプ
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            jumpPower,
            rb.linearVelocity.z
        );

        isGrounded = false;
    }

    public void WallKick()
    {
        speed += 8f;
        speed = Mathf.Clamp(speed, 0, maxSpeed);

        status = PlayerStatus.Dash;
    }

    public void Punch()
    {
        status = PlayerStatus.Punch;

        Debug.Log("パンチ ダメージ:" + attackPower);
    }

    public void Kick()
    {
        status = PlayerStatus.Kick;

        Debug.Log("キック ダメージ:" + (attackPower * 1.5f));
    }

    public void Slide()
    {
        if (!isGrounded) return;

        status = PlayerStatus.Slide;

        // スピード少し減少
        speed *= 0.8f;

        // 前方へ滑る
        rb.linearVelocity = new Vector3(
            transform.forward.x * speed,
            rb.linearVelocity.y,
            transform.forward.z * speed
        );

        // 今の向きを基準に45度傾ける（Z軸）
        transform.rotation *= Quaternion.Euler(0, 0, 45f);
        float slideDamage = attackPower + speed;
        StartCoroutine(ResetSlideRotation());

        Debug.Log("スライディング ダメージ:" + slideDamage);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;

            status = PlayerStatus.Run;
        }
    }
    private IEnumerator ResetSlideRotation()
    {
        yield return new WaitForSeconds(0.5f);

        transform.rotation = Quaternion.Euler(
            0,
            transform.eulerAngles.y,
            0
        );

        status = PlayerStatus.Run;
    }
}