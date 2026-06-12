using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerStatus
    {
        Idle,      // 待機
        Run,       // 移動
        Jump,      // ジャンプ
        Fall,      // 落下
        Attack,    // 通常攻撃
        Dash,      // 体当たり
        Slide,     // スライディング
        Damage,    // 被弾
        Dead       // 死亡
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerRun()
    {

    }
}
