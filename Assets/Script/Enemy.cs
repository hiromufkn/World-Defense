using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Enemy : MonoBehaviour
{
    public float Speed = 3f;
    public float moveRange = 3f;
    public float Hp = 500f;
    public Transform Player;
    public Transform model;
    public Transform firePoint;
    Vector3 Distance;
    public float attackRange = 10f;
    public float fireInterval = 2f;
    public float laserTime = 0.5f;
    public float InvincibleTime = 1f;
    
    //private EnemySpawner spawner;
    //public GameObject nextEnemy;

    private int direction = 1;
    private Vector3 StartPos;
    private LineRenderer line;
    private Vector3 targetPosition;
    private bool isFiring = false;
    private float timer;
    private float laserTimer;
    private bool isInvincible = false;
    private Renderer[] renderers; 



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

        line.enabled = false;

        renderers = GetComponentsInChildren<Renderer>();

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

            if(!isAttacking)
            {
                line.enabled = false;
                return;
            }

            timer += Time.deltaTime;

            if(!isFiring&&timer>=fireInterval)
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

                if(laserTimer<=0)
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

    public void TakeDamage(float damage = 1f)
    {
        if(isInvincible)
        {
            return;
        }

        Hp -= damage;

        if (Hp <= 0)
        {
            Destroy(gameObject);
            return;
        }

        StartCoroutine(Invincible());
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

    System.Collections.IEnumerator Invincible()
    {
        isInvincible = true;

        float timer = 0f;

        while(timer<InvincibleTime)
        {
            foreach(Renderer r in renderers)
            {
                r.enabled = false;
            }

            yield return new WaitForSeconds(0.1f);

            foreach(Renderer r in renderers)
            {
                r.enabled = true;
            }

            yield return new WaitForSeconds(0.1f);
            timer += 0.2f;
        }

        foreach(Renderer r in renderers)
        {
            r.enabled = true;
        }

        isInvincible = false;
    }
}
