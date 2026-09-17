using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting.Antlr3.Runtime;

public class Beam : MonoBehaviour
{

    public float moveRange = 3f;
    public Transform Player;
    public Transform firePoint;
    Vector3 Distance;
    public float attackRange = 5f;
    public float fireInterval = 2f;
    public float laserTime = 0.5f;
    public float InvincibleTime = 1f;
    public float beamWidth = 0.1f;
    public float beamLengthScale = 0.2f;
    public ParticleSystem beamEffect;
    public float damage = 5f;
    public float attackDistance = 10f;

    //private EnemySpawner spawner;
    //public GameObject nextEnemy;

    //private int direction = 1;
    private Vector3 StartPos;
    private Vector3 targetPosition;
    private bool isFiring = false;
    private float timer;
    private float laserTimer;
    private bool isInvincible = false;
    private Renderer[] renderers;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        //spawner = FindFirstObjectByType<EnemySpawner>();
        //direction = 1;

        Player = GameObject.FindWithTag("Player").transform;


        //beamEffect = GetComponentInChildren<ParticleSystem>();

        if (beamEffect != null)
        {
            beamEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }


    void Update()
    {
        if (Player == null || beamEffect == null)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(
        transform.position,
        Player.position);

        if (!isFiring)
        {
            if (distanceToPlayer <= attackDistance)
            {
                StartBeam();
            }
            return;
        }

        laserTimer -= Time.deltaTime;

        Vector3 targetPosition = Player.position;

        Vector3 direction = targetPosition - transform.position;

        float beamDistance = direction.magnitude;

        if (beamDistance > 0.01f)
        {

            beamEffect.transform.position = transform.position;

            beamEffect.transform.rotation = Quaternion.LookRotation(direction);

            beamEffect.transform.localScale = new Vector3(beamWidth, beamWidth, beamDistance / 10 * beamLengthScale);

            //beamEffect.Play(true);
        }

        LaserHitCheck(direction, beamDistance);

        if (laserTimer <= 0f)
        {
            StopBeam();
        }
    }


    void StartBeam()
    {
        isFiring = true;
        laserTimer = laserTime;
        timer = 0f;

        beamEffect.Play(true);
    }

    void StopBeam()
    {
        isFiring = false;

        beamEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        beamEffect.transform.localPosition = Vector3.zero;

        beamEffect.transform.localRotation = Quaternion.identity;

        beamEffect.transform.localScale = Vector3.one;

    }

    void LaserHitCheck(Vector3 direction, float distance)
    {
        if (distance <= 0.01f)
        {
            return;
        }


        if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, distance))
        {
            if (hit.transform.CompareTag("Player"))
            {
                Player player = hit.transform.GetComponent<Player>();

                if (player != null)
                {
                    player.TakeDamage(damage * Time.deltaTime);
                }
            }
        }
    }
}