using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy : MonoBehaviour
{
    public float Speed = 3f;
    public float moveRange = 3f;
    public float Hp = 500f;
    public Transform Player;
    public Transform model;

    //private EnemySpawner spawner;
    //public GameObject nextEnemy;

    private int direction=1;
    private Vector3 StartPos;
    private LineRenderer line;



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

        line = GetComponent<LineRenderer>();

        line.positionCount = 2;

    }

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        Debug.Log("“–‚½‚Á‚½");
    //        Destroy(gameObject);
    //    }
    //}

    // Update is called once per frame

    void FeirLaser()
    {
        line.SetPosition(0,model.position);
        line.SetPosition(1, Player.position);
    }
    void Update()
    {
   

        if (Player != null)
        {
             FeirLaser();

            Vector3 targetPos = Player.position;
            targetPos.y = model.position.y;

            model.LookAt(targetPos);

            //Debug.Log(model.eulerAngles);



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

    public void TakeDamage(float  damage=1f)
    {
        Hp -= damage;

        if (Hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
