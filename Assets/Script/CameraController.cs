using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float distance = 7f;
    [SerializeField] private float mouseSensitivity = 3f;

    [SerializeField] private float cameraOffset = 0.5f;

    [SerializeField] private LayerMask cameraCollisionLayer;
    [SerializeField] private float cameraRadius=0.5f;

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
        Vector3 lookPosition=player.position+Vector3.up * 1.5f;

        Vector3 targetPosition =
            lookPosition - rotation * Vector3.forward * distance;
        //プレイヤーからカメラ予定位置までレイを飛ばす
        Vector3 direction = targetPosition - lookPosition;
        float targetDistance = direction.magnitude;

        RaycastHit hit;

        if(Physics.SphereCast(lookPosition,cameraRadius,direction.normalized,out hit,targetDistance,cameraCollisionLayer))
        {
            targetPosition = hit.point - direction.normalized * cameraOffset;

            //// 壁に当たった場所までの距離
            //float safeDistance = hit.distance - cameraOffset;

            //safeDistance = Mathf.Max(safeDistance, 0.5f);

            //// カメラの高さを変えずに、距離だけ縮める
            //targetPosition =
            //    lookPosition - direction * safeDistance;

            //// 高さを元のカメラ位置に戻す
            //targetPosition.y =
            //    lookPosition.y -
            //    (rotation * Vector3.forward * distance).y;
        }

        transform.position = targetPosition;

        transform.LookAt(player.position + Vector3.up * 1.5f);
    }
}