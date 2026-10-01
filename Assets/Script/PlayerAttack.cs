using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    private Player player;

    private float SlideMAX = 0;//-45;

    public float slideDamage;

    void Start()
    {
        player = GetComponent<Player>();
    }

    public void OnSlide()
    {
        if (CanSlide())
        {
            Slide();
        }
    }

    public void Punch()
    {
        player.ChangeStatus(Player.PlayerStatus.Punch);

        Debug.Log("パンチ ダメージ:" + player.attackPower);
    }

    public void Kick()
    {
        player.ChangeStatus(Player.PlayerStatus.Kick);

        Debug.Log(
            "キック ダメージ:" +
            (player.attackPower * 1.5f)
        );
    }

    public void Slide()
    {
        if (!player.isGrounded) return;

        player.ChangeStatus(Player.PlayerStatus.Slide);

        player.speed *= 0.8f;

        player.rb.linearVelocity = new Vector3(
            transform.forward.x * player.speed,
            /*player.rb.linearVelocity.y*/0f,
            transform.forward.z * player.speed
        );

            transform.rotation = Quaternion.Euler(SlideMAX, transform.eulerAngles.y, 0f);
        
       

        slideDamage =
            (player.attackPower + player.speed) * 2;

        StartCoroutine(ResetSlideRotation());

        Debug.Log(
            "スライディング ダメージ:" +
            slideDamage
        );
    }

    private IEnumerator ResetSlideRotation()
    {
        yield return new WaitForSeconds(1.2f);

        transform.rotation = Quaternion.Euler(
            0,
            transform.eulerAngles.y,
            0
        );

        player.ChangeStatus(Player.PlayerStatus.Run);
        Debug.Log(
            "状態変更 : Slide → RUN"
        );
    }

    private bool CanSlide()
    {
        return !player.IsLowSpeed();
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision発生：" + other.gameObject.name);


        if (player.status != Player.PlayerStatus.Slide)
        
            return;
        
            Enemy enemy = other.gameObject.GetComponentInParent<Enemy>();

                if (enemy != null)
                {
                    Debug.Log("Enemy接触");
                    enemy.TakeDamage(slideDamage);
                    return;
                }
            

                Boss boss = other.gameObject.GetComponentInParent<Boss>();

                if(boss!=null)
                {
                    Debug.Log("Boss接触");
                    boss.TakeDamage(slideDamage);
                }
            }
    }
