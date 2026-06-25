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

    public void SetLookInput(Vector2 input)
    {
        lookInput = input;
    }

    private void LateUpdate()
    {
        yaw += lookInput.x * mouseSensitivity * Time.deltaTime; ;
        pitch -= lookInput.y * mouseSensitivity * Time.deltaTime; ;

        pitch = Mathf.Clamp(pitch, -30f, 60f);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        Vector3 targetPosition =
            player.position - rotation * Vector3.forward * distance;

        transform.position = targetPosition;

        transform.LookAt(player.position + Vector3.up * 1.5f);
    }
}