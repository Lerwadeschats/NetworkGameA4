using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamageManager : DamageManager
{
    Player _player;

    [SerializeField]
    Slider _slider;
    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    public override void OnBeingAttacked(float damages)
    {
        base.OnBeingAttacked(damages);
    }

    public override void OnTakingDamanges(float damages)
    {
        base.OnTakingDamanges(damages);
        float newHp = _player.Stats.hpValue - damages;
        if (newHp < 0)
            newHp = 0;

        Player.PlayerStats newStats = new Player.PlayerStats(_player.Stats.attackValue, newHp, _player.Stats.maxHpValue, _player.Stats.speed);
        _player.Stats = newStats;

        _slider.value = newHp / _player.Stats.maxHpValue;
    }
    
}
