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
        OnTakingDamages(damages);
    }

    public override void OnTakingDamages(float damages)
    {
        base.OnTakingDamages(damages);
        float newHp = _player.Stats.hpValue - damages;
        if (newHp < 0)
            newHp = 0;

        Player.PlayerStats newStats = new Player.PlayerStats(_player.Stats.attackValue, newHp, _player.Stats.maxHpValue, _player.Stats.speed);
        _player.Stats = newStats;

        
    }

    public override void GetHurtEffects()
    {
        base.GetHurtEffects();
        _player.animator.SetTrigger("Hurt");
    }

    private void Update()
    {
        _slider.value = _player.Stats.hpValue / _player.Stats.maxHpValue;
    }

}
