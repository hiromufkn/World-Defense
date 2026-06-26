using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Player player;
    public Slider hpSlider;

    void Start()
    {
        hpSlider.maxValue = player.maxHp;
        hpSlider.value = player.playerHp;
    }

    void Update()
    {
        hpSlider.value = player.playerHp;
    }
}