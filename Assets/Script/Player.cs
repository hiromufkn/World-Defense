using UnityEngine;
using UnityEngine.InputSystem;

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

    // スピード
    public float speed = 0f;
    public float maxSpeed = 30f;
    public float acceleration = 5f;

    // 横移動速度
    public float moveSpeed = 10f;

    // ジャンプ
    public float jumpPower = 8f;

    // 攻撃
    public float baseAttack = 10f;
    public float attackPower;

    // 速度倍率
    public float attackRate = 0.5f;

    // 入力
    private Vector2 moveInput;

    void Start()
    {
        attackPower = baseAttack;
    }

    void Update()
    {
        PlayerRun();

        attackPower = baseAttack + speed * attackRate;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // 移動処理
    public void PlayerRun()
    {
        float horizontal = moveInput.x;
        float vertical = moveInput.y;

        Vector3 move = Vector3.zero;

        // 前進
        if (vertical > 0)
        {
            status = PlayerStatus.Run;

            speed += acceleration * Time.deltaTime;
            speed = Mathf.Clamp(speed, 0, maxSpeed);

            move += Vector3.forward * speed * Time.deltaTime;
        }

        // 減速
        if (vertical < 0)
        {
            speed -= acceleration * Time.deltaTime;
            speed = Mathf.Clamp(speed, 0, maxSpeed);
        }

        // 横移動
        if (horizontal != 0)
        {
            move += Vector3.right * horizontal * moveSpeed * Time.deltaTime;
        }

        transform.Translate(move);
    }

    public void Jump()
    {
        status = PlayerStatus.Jump;

        float verticalPower = jumpPower;
        float forwardPower = speed;

        Debug.Log("ジャンプ 高さ:" + verticalPower +
                  " 飛距離:" + forwardPower);
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
        status = PlayerStatus.Slide;

        float slideDamage = attackPower + speed;

        Debug.Log("スライディング ダメージ:" + slideDamage);
    }
}