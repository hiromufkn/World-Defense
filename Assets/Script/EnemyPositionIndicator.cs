using UnityEngine;

public class EnemyPositionIndicator : MonoBehaviour
{
    // EnemySetを複数設定
    public Transform[] enemySets;

    // EnemySetごとの「！」を設定
    public RectTransform[] indicators;

    // 敵の中心から上に何m離すか
    public float height = 2.5f;

    private RectTransform canvasRect;
    private Camera cam;

    void Start()
    {
        Canvas canvas = GetComponentInParent<Canvas>();

        canvasRect = canvas.GetComponent<RectTransform>();

        cam = Camera.main;
    }

    void Update()
    {
        if (enemySets == null ||indicators == null || cam == null)
        {
            return;
        }

        // EnemySetを1つずつ処理
        for (int i = 0; i < enemySets.Length; i++)
        {
            Transform enemySet = enemySets[i];

            if (enemySet == null)
            {
                continue;
            }

            // 対応する「！」がない
            if (i >= indicators.Length ||indicators[i] == null)
            {
                continue;
            }

            RectTransform indicator = indicators[i];

            // EnemySet内のEnemyを取得
            Enemy[] enemyList =
                enemySet.GetComponentsInChildren<Enemy>();

            // 敵がいない
            if (enemyList.Length == 0)
            {
                //indicator.gameObject.SetActive(false);
                indicator.GetComponent<UnityEngine.UI.Image>().enabled = true;
                continue;
            }

            // ========================================
            // 敵3体の中心を計算
            // ========================================

            Vector3 enemyCenter = Vector3.zero;
            int enemyCount = 0;

            foreach (Enemy enemy in enemyList)
            {
                if (enemy == null)
                {
                    continue;
                }

                enemyCenter += enemy.transform.position;
                enemyCount++;
            }

            if (enemyCount == 0)
            {
                indicator.gameObject.SetActive(false);
                continue;
            }

            enemyCenter /= enemyCount;

            // ========================================
            // 敵の中心より上
            // ========================================

            Vector3 enemyPosition =enemyCenter + Vector3.up * height;

            // ========================================
            // ワールド座標 → スクリーン座標
            // ========================================

            Vector3 screenPos =cam.WorldToScreenPoint(enemyPosition);

            // カメラの後ろなら非表示
            if (screenPos.z < 0)
            { 
                continue;
            }

            // ========================================
            // 表示
            // ========================================

            indicator.gameObject.SetActive(true);

            // ========================================
            // スクリーン座標 → Canvas座標
            // ========================================

            Vector2 canvasPos;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,screenPos,null,out canvasPos);

            // ========================================
            // 「！」を移動
            // ========================================

            indicator.anchoredPosition = canvasPos;
        }
    }
}