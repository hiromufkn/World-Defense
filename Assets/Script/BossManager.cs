using UnityEngine;

public class BossManager : MonoBehaviour
{
    public GameObject boss;

    private int enemyCount;
    private int deadEnemyCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        enemyCount = FindObjectsByType<Enemy>().Length;

        deadEnemyCount = 0;

        Debug.Log("ìGÇÃêî:"+ enemyCount);
       }

    public void EnemyDied()
    {
        deadEnemyCount++;

        Debug.Log("éÄñSêî:" + deadEnemyCount + "/" + enemyCount);

        if(deadEnemyCount>=enemyCount)
        {
            Debug.Log("É{ÉXèoåª");
            showBoss();
        }
    }

    public void showBoss()
    {
        Instantiate(boss,transform.position, transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
