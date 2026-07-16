using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private Player player;
    private Vector2 moveInput;

    public Transform cameraTransform;
    public CameraController cameraController;

    private bool isWallRunning = false;
    private Vector3 wallNormal;
    private int wallSide = 1;   // 右壁=-1 左壁=1

    public Transform model;

    void Start()
    {
        player = GetComponent<Player>();

        cameraTransform = Camera.main.transform;
        cameraController =
            Camera.main.GetComponent<CameraController>();
    }

    void FixedUpdate()
    {
        PlayerRun();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        cameraController.SetLookInput(value.Get<Vector2>());
    }

    public void OnJump()
    {
        Jump();
    }

    public void PlayerRun()
    {
        // SlideとWallRun中は通常移動停止
        if (player.status == Player.PlayerStatus.Slide ||
            player.status == Player.PlayerStatus.WallRun)
        {
            return;
        }

        float horizontal = moveInput.x;
        float vertical = moveInput.y;

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection =
            cameraForward * vertical +
            cameraRight * horizontal;

        // 入力あり
        if (moveDirection.magnitude > 0)
        {
            float angle = Vector3.Angle(
                transform.forward,
                moveDirection.normalized
            );

            // 真逆だけブレーキ
            if (angle > 150f && player.speed > 0)
            {
                player.speed -=
                    player.brakePower *
                    Time.fixedDeltaTime;
            }
            else
            {
                transform.forward =
                    moveDirection.normalized;

                player.speed +=
                    player.acceleration *
                    Time.fixedDeltaTime;
            }

            player.ChangeStatus(
                Player.PlayerStatus.Run
            );
        }
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

        player.speed = Mathf.Clamp(
            player.speed,
            0,
            player.maxSpeed
        );

        // 実移動
        if (player.speed > 0)
        {
            float yVelocity =
                player.rb.linearVelocity.y;

            // 地面にいるなら軽く押し付ける
            if (player.isGrounded)
            {
                yVelocity = -2f;
            }

            player.rb.linearVelocity =
                new Vector3(
                    transform.forward.x *
                    player.speed,
                    yVelocity,
                    transform.forward.z *
                    player.speed
                );
        }
    }

    // ジャンプ
    public void Jump()
    {
        if (isWallRunning)
        {
            WallJump();
            return;
        }

        if (!player.isGrounded) return;

        player.ChangeStatus(
            Player.PlayerStatus.Jump
        );

        player.rb.linearVelocity =
            new Vector3(
                player.rb.linearVelocity.x,
                player.jumpPower,
                player.rb.linearVelocity.z
            );

        player.isGrounded = false;
    }

    // 壁ジャンプ
    private void WallJump()
    {
        isWallRunning = false;

        model.localRotation = Quaternion.identity;

        player.ChangeStatus(Player.PlayerStatus.Jump);

        Vector3 jumpDir =
            (wallNormal + Vector3.up).normalized;

        player.rb.linearVelocity =
            jumpDir * player.jumpPower;
    }

    // 壁走り開始
    private void StartWallRun()
    {
        isWallRunning = true;

        player.ChangeStatus(Player.PlayerStatus.WallRun);

        Vector3 wallForward =
            Vector3.Cross(Vector3.up, wallNormal);

        if (Vector3.Dot(wallForward, transform.forward) < 0)
        {
            wallForward = -wallForward;
        }

        transform.forward = wallForward;

        // 開始時だけ壁の左右を保存
        wallSide =
            Vector3.Dot(transform.right, wallNormal) > 0
            ? -1
            : 1;

        model.localRotation = Quaternion.Euler(
    0,
    0,
    wallSide * 45
);
    }

    // 壁走り維持
    private void MaintainWallRun()
    {
        Vector3 wallForward =
            Vector3.Cross(Vector3.up, wallNormal);

        if (Vector3.Dot(wallForward, transform.forward) < 0)
        {
            wallForward = -wallForward;
        }

        // 毎フレーム傾きを固定
        model.localRotation = Quaternion.Euler(
    0,
    0,
    wallSide * 45
);

        if (player.IsHighSpeed())
        {
            player.rb.linearVelocity =
                wallForward * player.speed;
        }
        else
        {
            player.rb.linearVelocity =
                wallForward * player.speed +
                Vector3.down * 2f;
        }
    }

    // 地面接触
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    isWallRunning = false;
                    player.isGrounded = true;

                    // Slide中は回転も状態も触らない
                    if (player.status != Player.PlayerStatus.Slide)
                    {
                        model.localRotation = Quaternion.identity;

                        player.ChangeStatus(
                            Player.PlayerStatus.Idle
                        );
                    }

                    break;
                }
            }
        }
    }

    // 壁接触中
    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Wall"))
            return;

        if (player.status == Player.PlayerStatus.Slide)
            return;

        if (!player.IsMidSpeed())
            return;

        // 壁に入った瞬間だけ法線取得
        if (!isWallRunning)
        {
            wallNormal = collision.contacts[0].normal;
            StartWallRun();
        }

        MaintainWallRun();
    }

    // 壁離脱
    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Wall"))
            return;

        isWallRunning = false;

        model.localRotation = Quaternion.identity;
    }
}