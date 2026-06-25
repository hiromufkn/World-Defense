using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    private Player player;

    private float SlideMAX = -45;

    public float slideDamage;

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
        
       

        slideDamage =
            player.attackPower + player.speed;

        StartCoroutine(ResetSlideRotation());

        Debug.Log(
            "スライディング ダメージ:" +
            slideDamage
        );
    }

    private IEnumerator ResetSlideRotation()
    {
        yield return new WaitForSeconds(1.5f);

        transform.rotation = Quaternion.Euler(
            0,
            transform.eulerAngles.y,
            0
        );

        player.status = Player.PlayerStatus.Run;
        Debug.Log(
            "状態変更 : Slide → RUN"
        );
    }
    private void OnCollisionStay(Collision collision)
    {
        if (player.status == Player.PlayerStatus.Slide)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy =
                    collision.gameObject.GetComponent<Enemy>();

                if (enemy != null)
                {
                    Debug.Log("Enemy接触");
                    enemy.TakeDamage(slideDamage);
                }
            }
        }
    }
}