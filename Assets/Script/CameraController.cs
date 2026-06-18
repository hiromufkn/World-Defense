using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float distance = 7f;
    [SerializeField] private float mouseSensitivity = 3f;

    private float pitch = 20f;
    private float yaw = 0f;

    private Vector2 lookInput;

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    private void LateUpdate()
    {
        yaw += lookInput.x * mouseSensitivity;
        pitch -= lookInput.y * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, -30f, 60f);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        Vector3 targetPosition =
            player.position - rotation * Vector3.forward * distance;

        transform.position = targetPosition;

        transform.LookAt(player.position + Vector3.up * 1.5f);
    }
}