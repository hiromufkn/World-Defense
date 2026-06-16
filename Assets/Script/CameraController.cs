using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("追尾するプレイヤー")]
    [SerializeField] private Transform player;

    [Header("カメラ位置設定")]
    [SerializeField] private Vector3 offset = new Vector3(0, 3, -7);

    [Header("追従速度")]
    [SerializeField] private float followSpeed = 5f;

    private void LateUpdate()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        // プレイヤーを基準にしたカメラの目標位置
        Vector3 targetPosition = player.position + player.rotation * offset;

        // なめらかに移動
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );

        // プレイヤーを見る
        transform.LookAt(player.position + Vector3.up * 1.5f);
    }
}
