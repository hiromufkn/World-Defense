using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    private Player core;

    void Start()
    {
        core = GetComponent<Player>();
    }

    public void OnSlide()
    {
        Slide();
    }

    public void Punch()
    {
        core.status = Player.PlayerStatus.Punch;

        Debug.Log("パンチ ダメージ:" + core.attackPower);
    }

    public void Kick()
    {
        core.status = Player.PlayerStatus.Kick;

        Debug.Log(
            "キック ダメージ:" +
            (core.attackPower * 1.5f)
        );
    }

    public void Slide()
    {
        if (!core.isGrounded) return;

        core.status = Player.PlayerStatus.Slide;

        core.speed *= 0.8f;

        core.rb.linearVelocity = new Vector3(
            transform.forward.x * core.speed,
            core.rb.linearVelocity.y,
            transform.forward.z * core.speed
        );

        transform.rotation *= Quaternion.Euler(0, 0, 45f);

        float slideDamage =
            core.attackPower + core.speed;

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

        core.status = Player.PlayerStatus.Run;
    }
}