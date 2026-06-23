using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    private Player Player;

    void Start()
    {
        Player = GetComponent<Player>();
    }

    public void OnSlide()
    {
        Slide();
    }

    public void Punch()
    {
        Player.status = Player.PlayerStatus.Punch;

        Debug.Log("パンチ ダメージ:" + Player.attackPower);
    }

    public void Kick()
    {
        Player.status = Player.PlayerStatus.Kick;

        Debug.Log(
            "キック ダメージ:" +
            (Player.attackPower * 1.5f)
        );
    }

    public void Slide()
    {
        if (!Player.isGrounded) return;

        Player.status = Player.PlayerStatus.Slide;

        Player.speed *= 0.8f;

        Player.rb.linearVelocity = new Vector3(
            transform.forward.x * Player.speed,
            Player.rb.linearVelocity.y,
            transform.forward.z * Player.speed
        );

        transform.rotation *= Quaternion.Euler(0, 0, 45f);

        float slideDamage =
            Player.attackPower + Player.speed;

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

        Player.status = Player.PlayerStatus.Run;
    }
}