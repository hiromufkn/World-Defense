using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private Player player;

    private Vector2 moveInput;

    public Transform cameraTransform;
    public CameraController cameraController;

    // 壁ダッシュ用
    private bool isWallRunning = false;
    private Vector3 wallNormal;

    void Start()
    {
        player = GetComponent<Player>();
        cameraTransform = Camera.main.transform;
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        PlayerRun();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        player.ChangeStatus(Player.PlayerStatus.Run);
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

        if (moveDirection.magnitude > 0)
        {
            float angle = Vector3.Angle(
                transform.forward,
                moveDirection.normalized
            );

            // 真逆入力だけブレーキ
            if (angle > 150f && player.speed > 0)
            {
                player.speed -=
                    player.brakePower * Time.fixedDeltaTime;
            }
            else
            {
                // 向き変更は即許可
                transform.forward =
                    moveDirection.normalized;

                player.speed +=
                    player.acceleration * Time.fixedDeltaTime;
            }

            player.ChangeStatus(Player.PlayerStatus.Run);
        }
        else
        {
            player.speed -=
                player.deceleration * Time.fixedDeltaTime;

            if (player.speed <= 0)
            {
                player.speed = 0;
                player.ChangeStatus(Player.PlayerStatus.Idle);
            }
        }

        player.speed = Mathf.Clamp(
            player.speed,
            0,
            player.maxSpeed
        );

        if (player.speed > 0)
        {
            player.rb.linearVelocity = new Vector3(
                transform.forward.x * player.speed,
                player.rb.linearVelocity.y,
                transform.forward.z * player.speed
            );
        }
    }

    // 壁ダッシュ
    private void StartWallRun()
    {
        isWallRunning = true;

        player.ChangeStatus(Player.PlayerStatus.WallRun);

        // 壁に沿う方向
        Vector3 wallForward =
            Vector3.Cross(Vector3.up, wallNormal);

        // 今の向きと逆なら反転
        if (Vector3.Dot(wallForward, transform.forward) < 0)
        {
            wallForward = -wallForward;
        }

        // 向きを壁走り方向へ
        transform.forward = wallForward;

        // 左右どちらの壁かで傾きを変える
        float tilt = Vector3.Dot(transform.right, wallNormal) > 0
            ? -45f
            : 45f;

        transform.rotation = Quaternion.Euler(
            transform.eulerAngles.x,
            transform.eulerAngles.y,
            tilt
        );

        if (player.speed >= player.highSpeed)
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

    private void MaintainWallRun()
    {
        Vector3 wallForward =
            Vector3.Cross(Vector3.up, wallNormal);

        if (Vector3.Dot(wallForward, transform.forward) < 0)
        {
            wallForward = -wallForward;
        }

        if (player.speed >= player.highSpeed)
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



    public void Jump()
    {
        if (isWallRunning)
        {
            WallJump();
            return;
        }

        if (!player.isGrounded) return;

        player.ChangeStatus(Player.PlayerStatus.Jump);

        player.rb.linearVelocity = new Vector3(
            player.rb.linearVelocity.x,
            player.jumpPower,
            player.rb.linearVelocity.z
        );

        player.isGrounded = false;
    }

    // 壁からの飛び移り
    private void WallJump()
    {
        isWallRunning = false;

        player.ChangeStatus(Player.PlayerStatus.Jump);

        Vector3 jumpDir =
            wallNormal + Vector3.up;

        player.rb.linearVelocity =
            jumpDir.normalized * player.jumpPower;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (player.status == Player.PlayerStatus.WallRun)
                return;

            if (player.status != Player.PlayerStatus.Slide)
            {
                transform.rotation = Quaternion.Euler(
                    0,
                    transform.eulerAngles.y,
                    0
                );
            }

            player.isGrounded = true;

            // Slide中はIdleにしない
            if (player.status != Player.PlayerStatus.Slide)
            {
                player.ChangeStatus(Player.PlayerStatus.Idle);
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (player.status == Player.PlayerStatus.Slide)
                return;

            if (player.speed >= player.midSpeed)
            {
                wallNormal = collision.contacts[0].normal;

                if (!isWallRunning)
                {
                    StartWallRun();
                }

                MaintainWallRun();
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            isWallRunning = false;

            transform.rotation = Quaternion.Euler(
                0,
                transform.eulerAngles.y,
                0
            );
        }
    }
}