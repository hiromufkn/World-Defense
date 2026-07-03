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
        // SlideÇ∆WallRuníÜÇÕí èÌà⁄ìÆí‚é~
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

        // ì¸óÕÇ†ÇË
        if (moveDirection.magnitude > 0)
        {
            float angle = Vector3.Angle(
                transform.forward,
                moveDirection.normalized
            );

            // ê^ãtÇæÇØÉuÉåÅ[ÉL
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
            // å∏ë¨
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

        // é¿à⁄ìÆ
        if (player.speed > 0)
        {
            float yVelocity =
                player.rb.linearVelocity.y;

            // ínñ Ç…Ç¢ÇÈÇ»ÇÁåyÇ≠âüÇµïtÇØÇÈ
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

    // ÉWÉÉÉìÉv
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

    // ï«ÉWÉÉÉìÉv
    private void WallJump()
    {
        isWallRunning = false;

        player.ChangeStatus(
            Player.PlayerStatus.Jump
        );

        Vector3 jumpDir =
            wallNormal + Vector3.up;

        player.rb.linearVelocity =
            jumpDir.normalized *
            player.jumpPower;
    }

    // ï«ëñÇËäJén
    private void StartWallRun()
    {
        isWallRunning = true;

        player.ChangeStatus(
            Player.PlayerStatus.WallRun
        );

        Vector3 wallForward =
            Vector3.Cross(
                Vector3.up,
                wallNormal
            );

        if (Vector3.Dot(
            wallForward,
            transform.forward) < 0)
        {
            wallForward = -wallForward;
        }

        transform.forward = wallForward;

        float tilt =
            Vector3.Dot(
                transform.right,
                wallNormal) > 0
            ? -45f
            : 45f;

        transform.rotation =
            Quaternion.Euler(
                0,
                transform.eulerAngles.y,
                tilt
            );
    }

    // ï«ëñÇËà€éù
    private void MaintainWallRun()
    {
        Vector3 wallForward =
            Vector3.Cross(
                Vector3.up,
                wallNormal
            );

        if (Vector3.Dot(
            wallForward,
            transform.forward) < 0)
        {
            wallForward = -wallForward;
        }

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

    // ínñ ê⁄êG
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

                    // SlideíÜÇÕâÒì]Ç‡èÛë‘Ç‡êGÇÁÇ»Ç¢
                    if (player.status != Player.PlayerStatus.Slide)
                    {
                        transform.rotation =
                            Quaternion.Euler(
                                0,
                                transform.eulerAngles.y,
                                0
                            );

                        player.ChangeStatus(
                            Player.PlayerStatus.Idle
                        );
                    }

                    break;
                }
            }
        }
    }

    // ï«ê⁄êGíÜ
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (player.status == Player.PlayerStatus.Slide)
                return;

            if (player.IsMidSpeed())
            {
                wallNormal =
                    collision.contacts[0].normal;

                if (!isWallRunning)
                {
                    StartWallRun();
                }

                MaintainWallRun();
            }
        }
    }

    // ï«ó£íE
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            isWallRunning = false;

            transform.rotation =
                Quaternion.Euler(
                    0,
                    transform.eulerAngles.y,
                    0
                );

            if (player.status != Player.PlayerStatus.Slide)
            {
                transform.rotation = Quaternion.Euler(
                    0,
                    transform.eulerAngles.y,
                    0
                );
            }
        }
    }
}