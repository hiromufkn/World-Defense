using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private Player player;

    private Vector2 moveInput;

    public Transform cameraTransform;
    public CameraController cameraController;

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
        if (player.status == Player.PlayerStatus.Slide)
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



    public void Jump()
    {
        if (!player.isGrounded) return;

        player.ChangeStatus(Player.PlayerStatus.Jump);

        player.rb.linearVelocity = new Vector3(
            player.rb.linearVelocity.x,
            player.jumpPower,
            player.rb.linearVelocity.z
        );

        player.isGrounded = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            player.isGrounded = true;
            player.ChangeStatus(Player.PlayerStatus.Idle);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector3 normal = collision.contacts[0].normal;

            Vector3 velocity = player.rb.linearVelocity;

            velocity = Vector3.ProjectOnPlane(
                velocity,
                normal
            );

            player.rb.linearVelocity = velocity;
        }
    }
}