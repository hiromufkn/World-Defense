using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    private Player player;

    private Enemy enemy;

    private float SlideMAX = -45;

    void Start()
    {
        player = GetComponent<Player>();
    }

    public void OnSlide()
    {
        Slide();
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
            player.rb.linearVelocity.y,
            transform.forward.z * player.speed
        );

            transform.rotation = Quaternion.Euler(SlideMAX, transform.eulerAngles.y, 0f);
        
       

        float slideDamage =
            player.attackPower + player.speed;

        if (player.isAttack)
        {
            enemy.TakeDamage(slideDamage);
        }

        StartCoroutine(ResetSlideRotation());

        Debug.Log(
            "スライディング ダメージ:" +
            slideDamage
        );
    }

    private IEnumerator ResetSlideRotation()
    {
        yield return new WaitForSeconds(0.5f);

        transform.rotation = Quaternion.Euler(
            0,
            transform.eulerAngles.y,
            0
        );

        player.status = Player.PlayerStatus.Run;
    }
    private void OnCollisionEnter(Collision collision)
    {
            if(player.status == Player.PlayerStatus.Slide)
            {
                if (collision.gameObject.CompareTag("Enemy"))
                {
                    player.isAttack = true;
                }
            }
    }
}