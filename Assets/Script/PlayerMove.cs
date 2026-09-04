using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    //==============================
    // プレイヤー情報
    //==============================

    // Playerスクリプト取得用
    private Player player;

    // 入力された移動方向(WASD)
    private Vector2 moveInput;

    //==============================
    // カメラ
    //==============================

    // メインカメラ
    public Transform cameraTransform;

    // カメラ操作スクリプト
    public CameraController cameraController;

    //==============================
    // 壁走り
    //==============================

    // 壁走り中か
    private bool isWallRunning = false;

    // 壁の法線
    private Vector3 wallNormal;

    // 壁走り方向
    private Vector3 wallForward;

    // 左右どちらの壁か
    private int wallSide;

    private float wallRunCooldown = 0f;

    // L字などで別の壁に近づいたときの検出距離
    [SerializeField]
    private float cornerCheckDistance = 0.5f;

    // 見た目だけ傾けるモデル
    [SerializeField]
    private Transform model;

    private Quaternion defaultModelRotation;

    // ノックバックに使う
    private float knockBackTime = 10f;
    private float knockBackTimer = 0f;

    //==============================
    // 初期化
    //==============================

    private void Start()
    {
        player = GetComponent<Player>();

        cameraTransform = Camera.main.transform;
        cameraController =
            Camera.main.GetComponent<CameraController>();

        defaultModelRotation = model.localRotation;
    }

    //==============================
    // 毎フレーム物理更新
    //==============================

    private void FixedUpdate()
    {
        if (wallRunCooldown > 0f)
        {
            wallRunCooldown -= Time.fixedDeltaTime;
        }

        // ノックバック中は移動をなくす
        if(player.status == Player.PlayerStatus.KnockBack)
        {
            knockBackTimer -= Time.fixedDeltaTime;

            if (knockBackTimer <= 0f)
            {
                player.ChangeStatus(
                    Player.PlayerStatus.Run
                );
            }
            return;
        }

        PlayerRun();
    }

    //==============================
    // 移動入力
    //==============================

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    //==============================
    // カメラ入力
    //==============================

    public void OnLook(InputValue value)
    {
        cameraController.SetLookInput(
            value.Get<Vector2>()
        );
    }

    //==============================
    // ジャンプ入力
    //==============================

    public void OnJump()
    {
        Jump();
    }

    //==============================
    // 通常移動
    //==============================

    private void PlayerRun()
    {
        // スライディング中は動かさない
        if (!CanMove())
            return;

        //--------------------------------
        // カメラ基準の移動方向作成
        //--------------------------------

        Vector3 cameraForward =
            cameraTransform.forward;

        Vector3 cameraRight =
            cameraTransform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection =
            cameraForward * moveInput.y +
            cameraRight * moveInput.x;

        //--------------------------------
        // 入力あり
        //--------------------------------

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            // 向き変更
            moveDirection.Normalize();

            // 先に角度を取得
            float angle =
    Vector3.Angle(
        transform.forward,
        moveDirection);

            if (angle > 150f)
            {
                // ブレーキ
                player.speed -=
                    player.brakePower *
                    Time.fixedDeltaTime;

                // 十分減速したら向きを変える
                if (player.speed <= player.turnSpeedThreshold)
                {
                    Quaternion targetRotation =
                        Quaternion.LookRotation(moveDirection);

                    transform.rotation =
                        Quaternion.RotateTowards(
                            transform.rotation,
                            targetRotation,
                            720f * Time.fixedDeltaTime
                        );
                }
            }
            else
            {
                Quaternion targetRotation =
                    Quaternion.LookRotation(moveDirection);

                transform.rotation =
                    Quaternion.RotateTowards(
                        transform.rotation,
                        targetRotation,
                        720f * Time.fixedDeltaTime
                    );

                player.speed +=
                    player.acceleration *
                    Time.fixedDeltaTime;
            }
        }
        //--------------------------------
        // 入力なし
        //--------------------------------
        else
        {
            // 減速
            player.speed -=
                player.deceleration *
                Time.fixedDeltaTime;

            if (player.speed <= 0)
            {
                player.speed = 0;

                player.ChangeStatus(
                    Player.PlayerStatus.Idle
                );
            }
        }

        //--------------------------------
        // スピード制限
        //--------------------------------

        player.speed = Mathf.Clamp(
            player.speed,
            0,
            player.maxSpeed
        );

        //--------------------------------
        // Rigidbodyへ反映
        //--------------------------------

        Vector3 velocity =
            transform.forward * player.speed;

        velocity.y =
            player.rb.linearVelocity.y;

        player.rb.linearVelocity =
            velocity;
    }

    //==============================
    // ジャンプ
    //==============================

    private void Jump()
    {
        if (isWallRunning)
        {
            WallJump();
            return;
        }

        if (!player.isGrounded)
            return;

        player.ChangeStatus(
            Player.PlayerStatus.Jump);

        Vector3 velocity =
            player.rb.linearVelocity;

        velocity.y =
            player.jumpPower / 2;

        player.rb.linearVelocity =
            velocity;

        player.isGrounded = false;
    }

    private void StartWallRun()
    {
        isWallRunning = true;
        player.isGrounded = false;

        player.ChangeStatus(Player.PlayerStatus.WallRun);

        // 壁に沿う方向を作る
        wallForward =
            Vector3.Cross(Vector3.up, wallNormal);

        // プレイヤーの向きと逆なら反転
        if (Vector3.Dot(
            wallForward,
            transform.forward) < 0)
        {
            wallForward = -wallForward;
        }

        // プレイヤー本体は壁方向を向く
        transform.forward = wallForward;

        // 左右判定
        wallSide =
            Vector3.Dot(transform.right, wallNormal) > 0
            ? -1
            : 1;

        // 見た目だけ傾ける
        model.localRotation =
    defaultModelRotation *
    Quaternion.Euler(0, 0, wallSide * 45);
    }
    

    private void MaintainWallRun()
    {

        // L字の内側などで、進行方向に別の壁があるか確認
        if (IsApproachingAnotherWall())
        {
            EndWallRun();
            return;
        }

        // 高速なら落ちない
        if (player.IsHighSpeed())
        {
            player.rb.linearVelocity =
                wallForward *
                player.speed;
        }
        // 中速なら少しずつ落ちる
        else
        {
            player.rb.linearVelocity =
                wallForward *
                player.speed +
                Vector3.down * 2f;
        }
    }
    private void EndWallRun()
    {
        isWallRunning = false;

        model.localRotation = defaultModelRotation;

        if (player.status ==
            Player.PlayerStatus.WallRun)
        {
            player.ChangeStatus(
                Player.PlayerStatus.Run
            );
        }
    }

    private void WallJump()
    {
        Debug.Log("wallNormal = " + wallNormal);
        EndWallRun();

        player.ChangeStatus(Player.PlayerStatus.Jump);

        // 壁から離れる力
        Vector3 away = wallNormal * 2.0f; 
                                            // 現段階では横にジャンプしてスピード感が持たせたいからawayの方を大きくしてある
        // 上方向
        Vector3 up = Vector3.up * 1.0f;

        Vector3 jumpDirection =
    (away + up).normalized;

        player.rb.linearVelocity =
            jumpDirection * player.jumpPower;

        player.isGrounded = false;
        Debug.Log(player.rb.linearVelocity);

        wallRunCooldown = 0.25f;
    }

    //==============================
    // 現在操作できるか
    //==============================
    private bool CanMove()
    {
        return player.status != Player.PlayerStatus.Slide &&
               player.status != Player.PlayerStatus.WallRun;
    }

    

    private bool IsApproachingAnotherWall()
    {
        Vector3 origin =
            transform.position;

        Vector3 direction =
            wallForward;

        if (Physics.Raycast(
            origin,
            direction,
            out RaycastHit hit,
            cornerCheckDistance))
        {
            // 現在走っている壁とは別の壁にぶつかりそう
            if (hit.collider.CompareTag("Wall"))
            {
                // 現在の壁かどうか確認
                float normalDot =
                    Vector3.Dot(
                        hit.normal,
                        wallNormal
                    );

                // 法線が違うなら別の壁
                if (normalDot < 0.9f)
                {
                    return true;
                }
            }
        }

        return false;
    }

    //==============================
    // 壁走りできるか
    //==============================
    private bool CanWallRun()
    {
        return
            wallRunCooldown <= 0f &&
            player.IsMidSpeed() &&
            player.status != Player.PlayerStatus.Slide;
    }

    // 障害物衝突時ノックバック
    private void PlayerKnockBack(Transform objectPos, float knockBackPower)
    {
        // ノックバック中
        player.ChangeStatus(Player.PlayerStatus.KnockBack);

        // タイマー開始
        knockBackTimer = knockBackTime;

        // 障害物からプレイヤーに向かう方向
        Vector3 knockBackDirection =
            transform.position - objectPos.position;

        // 水平方向だけにする
        knockBackDirection.y = 0;

        knockBackDirection.Normalize();

        // ノックバック
        player.rb.linearVelocity =
            knockBackDirection * knockBackPower;

        // 少し上に飛ばす
        player.rb.linearVelocity +=
            Vector3.up * 10f;
    }

    //==============================
    // 地面判定
    //==============================

    private void OnCollisionEnter(Collision collision)
    {
        if (isWallRunning)
        {
            EndWallRun();
        }

        player.isGrounded = true;

        if (player.status != Player.PlayerStatus.Slide)
        {
            player.ChangeStatus(Player.PlayerStatus.Idle);
        }

        // 障害物との衝突時
        if (collision.gameObject.CompareTag("Object"))
        {
            Debug.Log("衝突");
            PlayerKnockBack(collision.transform, 10f);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Wall"))
            return;

        if (!CanWallRun())
            return;

        // まだ壁走りしていない場合だけ
        // 壁の法線を取得する
        if (!isWallRunning)
        {
            wallNormal =
                collision.contacts[0].normal;

            StartWallRun();
        }

        MaintainWallRun();
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Wall"))
            return;

        EndWallRun();
    }
}