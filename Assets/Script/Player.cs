using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerStatus
    {
        Idle,       // 待機
        Run,        // 走行
        Jump,       // ジャンプ
        Fall,       // 落下
        Punch,      // パンチ
        Kick,       // キック
        Slide,      // スライディング
        Dash,       // 加速移動・体当たり
        Damage,     // ダメージ
        Dead        // 死亡
    }

    // 現在の状態
    public PlayerStatus status = PlayerStatus.Idle;

    // スピード
    public float speed = 0f;
    public float maxSpeed = 30f;
    public float acceleration = 5f;
    public float deceleration = 10f;

    // ジャンプ
    public float jumpPower = 8f;

    // 攻撃
    public float baseAttack = 10f;
    public float attackPower;

    // 速度による攻撃倍率
    public float attackRate = 0.5f;


    void Start()
    {
        attackPower = baseAttack;
    }


    void Update()
    {
        PlayerRun();

        // 速度に応じて攻撃力アップ
        attackPower = baseAttack + speed * attackRate;
    }


    // 常に前進して加速する
    public void PlayerRun()
    {
        status = PlayerStatus.Run;

        // 時間経過で加速
        speed += acceleration * Time.deltaTime;

        // 最大速度制限
        speed = Mathf.Clamp(speed, 0, maxSpeed);

        // 前方向へ移動
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }


    // ジャンプ
    public void Jump()
    {
        status = PlayerStatus.Jump;

        // 高さは固定
        float verticalPower = jumpPower;

        // 飛距離は speed に依存
        float forwardPower = speed;

        Debug.Log(
            "ジャンプ 高さ:" + verticalPower +
            " 飛距離:" + forwardPower
        );
    }


    // 壁キックなど足を使った動作
    public void WallKick()
    {
        // 追加加速
        speed += 8f;

        // 最大速度を超えない
        speed = Mathf.Clamp(speed, 0, maxSpeed);

        status = PlayerStatus.Dash;
    }


    // パンチ
    public void Punch()
    {
        status = PlayerStatus.Punch;

        Debug.Log(
            "パンチ ダメージ:" + attackPower
        );
    }


    // キック
    public void Kick()
    {
        status = PlayerStatus.Kick;

        Debug.Log(
            "キック ダメージ:" + (attackPower * 1.5f)
        );
    }


    // スライディング
    public void Slide()
    {
        status = PlayerStatus.Slide;

        // 高速時ほど強い
        float slideDamage = attackPower + speed;

        Debug.Log(
            "スライディング ダメージ:" + slideDamage
        );
    }
}