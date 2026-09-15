using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using static Enemy;
using Unity.VisualScripting;

public class Boss : MonoBehaviour
{
 
    public Transform Player;
    public Transform model;
    public Transform firePoint;

    public float attackRange = 5f;
    public float fireInterval = 2f;
    public float laserTime = 0.5f;
    public float InvincibleTime = 1f;
    public float Hp = 1000;
    public float MaxHp = 1000;
    public ParticleSystem beamEffect;
Å@Å@public ParticleSystem deathEffect;
    public float beamWidth = 0.1f;
    public float beamLengthScale = 0.2f;



    //private EnemySpawner spawner;
    //public GameObject nextEnemy;

    //private int direction = 1;

    private LineRenderer line;
    private Vector3 targetPosition;
    private bool isFiring = false;
    private float timer;
    private float laserTimer;
    private bool isInvincible = false;
    private Renderer[] renderers;
    private Slider hpSlider;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Hp = MaxHp;

        hpSlider = GetComponentInChildren<Slider>();

        hpSlider.maxValue = MaxHp;
        hpSlider.value = Hp;

        renderers = GetComponentsInChildren<Renderer>();

        //spawner = FindFirstObjectByType<EnemySpawner>();
        //direction = 1;

        Player = GameObject.FindWithTag("Player").transform;

        if (model == null)
        {
            //model = transform.GetChild(0);
            model = transform.Find("Model");
        }

        if (beamEffect != null)
        {
            beamEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        //line = GetComponent<LineRenderer>();

        //line.positionCount = 2;

        //line.enabled = false;

    }

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        Debug.Log("ìñÇΩÇ¡ÇΩ");
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
                //line.enabled = false;
                if (beamEffect != null)
                {
                    beamEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
                isFiring = false;
                return;
            }

            timer += Time.deltaTime;

            if (!isFiring && timer >= fireInterval)
            {
                isFiring = true;
                laserTimer = laserTime;
                targetPosition = Player.position;
                //line.enabled = true;
                //FeirLaser();
                if (beamEffect != null)
                {
                    Vector3 direction = targetPosition - firePoint.position;
                    float beamDistance = direction.magnitude;

                    beamEffect.transform.position = firePoint.position;

                    beamEffect.transform.rotation = Quaternion.LookRotation(direction);

                    beamEffect.transform.localScale = new Vector3(beamWidth, beamWidth, beamDistance / 10 * beamLengthScale);

                    beamEffect.Play(true);
                }
            }



            if (isFiring)
            {
                laserTimer -= Time.deltaTime;

                //FeirLaser();
                LaserHitCheck();

                if (laserTimer <= 0)
                {
                    isFiring = false;
                    timer = 0f;
                    //line.enabled = false;
                    if (beamEffect != null)
                    {
                        beamEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

                        beamEffect.transform.localPosition = Vector3.zero;

                        beamEffect.transform.localRotation = Quaternion.identity;

                        beamEffect.transform.localScale = Vector3.one;
                    }
                }
            }

            Vector3 targetPos = Player.position;
            targetPos.y = model.position.y;

            model.LookAt(targetPos);

            //Debug.Log(model.eulerAngles);
        }


        if (Keyboard.current.kKey.wasPressedThisFrame)

        {
            Debug.Log("KÉLÅ[Ç≈ìGéÄñS:");
            if(deathEffect!=null)
            {
                ParticleSystem effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
                effect.Play();
                Destroy(effect.gameObject, 2f);
            }
            
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
        if (isInvincible)
        {
            return;
        }

        Hp -= damage;

        hpSlider.value = Hp;

        if (Hp <= 0)
        {
            if(deathEffect!=null)
            {
                ParticleSystem effect = Instantiate(deathEffect, transform.position,Quaternion.identity);
                effect.Play();
                Destroy(effect.gameObject, 2f);
            }
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

        while (timer < InvincibleTime)
        {
            foreach (Renderer r in renderers)
            {
                r.enabled = false;
            }

            yield return new WaitForSeconds(0.1f);

            foreach (Renderer r in renderers)
            {
                r.enabled = true;
            }

            yield return new WaitForSeconds(0.1f);
            timer += 0.2f;
        }

        foreach (Renderer r in renderers)
        {
            r.enabled = true;
        }

        isInvincible = false;
    }
}

    