using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy : MonoBehaviour
{
    public float Speed = 3f;
    public float moveRange = 3f;
    public Transform Player;
    public Transform model;

    //private EnemySpawner spawner;
    //public GameObject nextEnemy;

    private int direction=1;
    private Vector3 StartPos;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartPos = transform.position;

        //spawner = FindFirstObjectByType<EnemySpawner>();
        direction = 1;

        Player = GameObject.FindWithTag("Player").transform;

        if (model == null)
        {
            //model = transform.GetChild(0);
            model = transform.Find("Model");
        }

    }

    // Update is called once per frame
    void Update()
    {
   

        if (Player != null)
        {


            Vector3 targetPos = Player.position;
            targetPos.y = model.position.y;

            model.LookAt(targetPos);

            Debug.Log(model.eulerAngles);



            if (Keyboard.current.kKey.wasPressedThisFrame)

            {

                Destroy(gameObject);

                //spawner.SpawnEnemy();
            }
        }
        

    //    transform.Translate(Vector3.right * Speed * direction * Time.deltaTime);

    //    if (transform.position.x > StartPos.x + moveRange)
    //    {
    //        direction = -1;
    //    }

    //    if(transform.position.x<StartPos.x-moveRange)
    //    {
    //        direction = 1;
    //    }

    }
}
