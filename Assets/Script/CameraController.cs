using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float distance = 7f;
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float cameraOffset = 1.0f;
    [SerializeField] private LayerMask cameraCollisionLayer;
    [SerializeField] private LayerMask SideCollisionLayer;

    [SerializeField] private float cameraRadius = 1.0f;
    [SerializeField] private float minCameraDistance = 1.0f;
    [SerializeField] private float cameraSmoothTime = 0.08f;
    [SerializeField] private float sideCheckDistance = 1.5f;
    // 画面端の壁を避けるための追加余白
    [SerializeField] private float sideCameraOffset = 0.15f;

    private float pitch = 20f;
    private float yaw = 0f;
    private float currentCameraDistance;
    private float cameraDistanceVelocity;

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

        float targetCameraDistance = distance;

        RaycastHit hit;

        

        if (Physics.SphereCast(lookPosition,cameraRadius,direction,out hit,distance,cameraCollisionLayer))
        {
            // 壁の手前まで
            targetCameraDistance =hit.distance - cameraRadius - cameraOffset- 0.1f;

            // 壁が近すぎる場合は0.05mまで許可
            // minCameraDistanceで1mに戻さない
            targetCameraDistance =Mathf.Max(targetCameraDistance, 0.05f);
        }


        // ========================================
        // ③ 壁がない場合だけ通常の最低距離を適用
        // ========================================

        if (!Physics.SphereCast(lookPosition,cameraRadius,direction,out hit,distance,cameraCollisionLayer))
        {
            targetCameraDistance =Mathf.Clamp(targetCameraDistance,minCameraDistance,distance);
        }


        // ========================================
        // ④ カメラ距離を滑らかに変更
        // ========================================

        if (currentCameraDistance == 0f)
        {
            currentCameraDistance = targetCameraDistance;
        }

        currentCameraDistance = Mathf.SmoothDamp(currentCameraDistance,targetCameraDistance,ref cameraDistanceVelocity,cameraSmoothTime);


        // ========================================
        // ⑤ 壁より奥へ行かないようにする
        // ========================================

        if (Physics.SphereCast(lookPosition,cameraRadius,direction,out hit,distance,cameraCollisionLayer))
        {
            float safeDistance =hit.distance- cameraRadius- cameraOffset- 0.1f;

            safeDistance =Mathf.Max(safeDistance, 0.05f);

            // 現在のカメラ距離が壁を越えないようにする
            currentCameraDistance =Mathf.Min(currentCameraDistance,safeDistance);
        }


        // ========================================
        // ⑥ カメラ位置
        // ========================================

        Vector3 targetPosition =lookPosition+ direction * currentCameraDistance;


        // ========================================
        // ⑦ カメラ自身が壁の中に入っていないか確認
        // ========================================

        if (Physics.CheckSphere(targetPosition,cameraRadius,cameraCollisionLayer))
        {
            // 壁の中だった場合、
            // プレイヤー方向へ戻す
            float safeDistance = currentCameraDistance;

            for (int i = 0; i < 30; i++)
            {
                safeDistance -= 0.05f;

                if (safeDistance <= 0.05f)
                {
                    safeDistance = 0.05f;
                    break;
                }

                Vector3 checkPosition =lookPosition+ direction * safeDistance;

                if (!Physics.CheckSphere(checkPosition,cameraRadius,cameraCollisionLayer))
                {
                    break;
                }
            }

            currentCameraDistance = safeDistance;

            targetPosition =lookPosition+ direction * currentCameraDistance;
        }


        //cameraDistance = Mathf.Clamp(cameraDistance, minCameraDistance, distance);

        transform.position = targetPosition;

        transform.LookAt(player.position + Vector3.up * 1.5f);
    }
}
