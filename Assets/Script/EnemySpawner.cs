using System.Threading;
using UnityEngine;


public class EnemySpawner: MonoBehaviour
{

    public GameObject[] enemies;
    public Transform spawnPosition;

    public float spawnInterval=3.0f;
    private float timer;
    private int enemyIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //timer += Time.deltaTime;

        //if(timer>=spawnInterval)
        //{
        //    SpawnEnemy();
        //    timer = 0;
        //}
    }

    public void SpawnEnemy()
    {
        if(enemyIndex>=enemies.Length)
        {
            return;
        }
        

        Instantiate(
            enemies[enemyIndex],
            spawnPosition.position,
            Quaternion.identity

            );

        enemyIndex++;
    }
}
