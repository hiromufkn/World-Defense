using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float distance = 7f;
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float cameraOffset = 0.7f;
    [SerializeField] private LayerMask cameraCollisionLayer;
    [SerializeField] private LayerMask SideCollisionLayer;

    [SerializeField] private float cameraRadius = 0.8f;
    [SerializeField] private float minCameraDistance = 1.0f;

    // 壁に当たったときの横移動量
    // [SerializeField] private float sideMoveAmount = 0.5f;
    [SerializeField] private float sideCheckDistance = 1.5f;
    // 画面端の壁を避けるための追加余白
    [SerializeField] private float sideCameraOffset = 0.15f;

    private float pitch = 20f;
    private float yaw = 0f;

    private Vector2 lookInput;

    public void SetLookInput(Vector2 input)
    {
        lookInput = input;
    }

    private void LateUpdate()
    {
        yaw += lookInput.x * mouseSensitivity * Time.deltaTime;
        pitch -= lookInput.y * mouseSensitivity * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, 0f, 60f);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        //プレイヤーの頭付近を基準にする
        Vector3 lookPosition = player.position + Vector3.up * 1.5f;

        //プレイヤーからカメラへ向かう
        Vector3 direction = -(rotation * Vector3.forward);

        float cameraDistance = distance;

        RaycastHit hit;

        if (Physics.SphereCast(lookPosition, cameraRadius, direction, out hit, distance, cameraCollisionLayer))
        {
            //targetPosition = hit.point - direction.normalized * cameraOffset;

            //壁の手前までカメラを近づける
            cameraDistance = hit.distance - cameraRadius - cameraOffset - 0.2f;
        }

        // 最終的なカメラ位置
        Vector3 targetPosition = lookPosition + direction * cameraDistance;


        
        

        cameraDistance = Mathf.Clamp(cameraDistance, minCameraDistance, distance);

        targetPosition = lookPosition + direction * cameraDistance;

        transform.position = targetPosition;

        transform.LookAt(player.position + Vector3.up * 1.5f);
    }
}