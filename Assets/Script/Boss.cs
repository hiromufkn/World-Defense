using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Boss : MonoBehaviour
{

    public Transform Player;
    public Transform model;
    public Transform firePoint;

    public float attackRange = 5f;
    public float fireInterval = 2f;
    public float laserTime = 0.5f;
    public float InvincibleTime = 1f;

    //private EnemySpawner spawner;
    //public GameObject nextEnemy;

    //private int direction = 1;

    private LineRenderer line;
    private Vector3 targetPosition;
    private bool isFiring = false;
    private float timer;
    private float laserTimer;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


        //spawner = FindFirstObjectByType<EnemySpawner>();
        //direction = 1;

        Player = GameObject.FindWithTag("Player").transform;

        if (model == null)
        {
            //model = transform.GetChild(0);
            model = transform.Find("Model");
        }

        line = GetComponent<LineRenderer>();

        line.positionCount = 2;

        line.enabled = false;

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
        line.SetPosition(0, firePoint.position);
        line.SetPosition(1, targetPosition);
    }
    void Update()
    {


        if (Player != null)
        {

            float distance = Vector3.Distance(transform.position, Player.position);

            bool isAttacking = distance <= attackRange;

            if (!isAttacking)
            {
                line.enabled = false;
                return;
            }

            timer += Time.deltaTime;

            if (!isFiring && timer >= fireInterval)
            {
                isFiring = true;
                laserTimer = laserTime;
                targetPosition = Player.position;
                line.enabled = true;
                FeirLaser();
            }



            if (isFiring)
            {
                laserTimer -= Time.deltaTime;

                FeirLaser();
                LaserHitCheck();

                if (laserTimer <= 0)
                {
                    isFiring = false;
                    timer = 0f;
                    line.enabled = false;
                }
            }

            Vector3 targetPos = Player.position;
            targetPos.y = model.position.y;

            model.LookAt(targetPos);

            //Debug.Log(model.eulerAngles);
        }


        if (Keyboard.current.kKey.wasPressedThisFrame)

        {
            Debug.Log("KƒL[‚Å“GŽ€–S:");
            Destroy(gameObject);

            //spawner.SpawnEnemy();
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

    void LaserHitCheck()
    {
        Vector3 start = firePoint.position;
        Vector3 end = targetPosition;

        Vector3 dir = (end - start).normalized;
        float distance = Vector3.Distance(start, end);

        if (Physics.Raycast(start, dir, out RaycastHit hit, distance))
        {
            if (hit.transform.CompareTag("Player"))
            {
                hit.transform.GetComponent<Player>().TakeDamage(10f * Time.deltaTime);
            }
        }
    }
}

    