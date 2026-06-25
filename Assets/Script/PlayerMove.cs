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
        player.ChangeStatus(Player.PlayerStatus.Run);
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
            player.speed += player.acceleration * Time.fixedDeltaTime;
            player.speed = Mathf.Clamp(
                player.speed,
                0,
                player.maxSpeed
            );

            transform.forward = moveDirection.normalized;

            player.rb.linearVelocity = new Vector3(
                moveDirection.normalized.x * player.speed,
                player.rb.linearVelocity.y,
                moveDirection.normalized.z * player.speed
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
}