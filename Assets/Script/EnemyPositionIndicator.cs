using UnityEngine;

public class EnemyPositionIndicator : MonoBehaviour
{
    public Transform[] enemies;
    public Transform Player;

    public float screenMargin = 100f;
    public float height = 2.5f;

    private RectTransform rect;
    private RectTransform canvasRect;
    private Camera cam;

    void Start()
    {
        rect = GetComponent<RectTransform>();

        Canvas canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();

        cam = Camera.main;
    }

    void Update()
    {
        if (Player == null || enemies.Length == 0 || cam == null)
        {
            return;
        }

        // ˆê”Ô‹ß‚¢“G‚ð’T‚·
        Transform nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Transform enemy in enemies)
        {
            if (enemy == null)
            {
                continue;
            }

            float distance = Vector3.Distance(
                Player.position,
                enemy.position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy == null)
        {
            return;
        }

        // ˆê”Ô‹ß‚¢“G‚ÌˆÊ’u‚ðŽæ“¾
        Vector3 enemyPosition =
            nearestEnemy.position + Vector3.up * height;

        Vector3 screenPos =
            cam.WorldToScreenPoint(enemyPosition);

        // “G‚ªƒJƒƒ‰‚ÌŒã‚ë‚É‚¢‚éê‡
        if (screenPos.z < 0)
        {
            screenPos.x = Screen.width - screenPos.x;
            screenPos.y = Screen.height - screenPos.y;
        }

        Vector2 canvasPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            null,
            out canvasPos
        );

        float halfWidth = canvasRect.rect.width / 2f;
        float halfHeight = canvasRect.rect.height / 2f;

        canvasPos.x = Mathf.Clamp(
            canvasPos.x,
            -halfWidth + screenMargin,
            halfWidth - screenMargin
        );

        canvasPos.y = Mathf.Clamp(
            canvasPos.y,
            -halfHeight + screenMargin,
            halfHeight - screenMargin
        );

        rect.anchoredPosition = canvasPos;

        // “G‚Ì•ûŒü‚Ö–îˆó‚ðŒü‚¯‚é
        Vector2 direction = canvasPos;

        if (direction.sqrMagnitude > 0.01f)
        {
            float angle =
                Mathf.Atan2(direction.y, direction.x)
                * Mathf.Rad2Deg;

            rect.localRotation =
                Quaternion.Euler(0f, 0f, angle - 90f);
        }
    }
}