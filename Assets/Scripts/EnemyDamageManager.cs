using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamageManager : DamageManager
{
    Enemy _enemy;

    [SerializeField]
    Slider  _slider;
    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        
    }
    public override void OnBeingAttacked(float damages)
    {
        base.OnBeingAttacked(damages);
        OnTakingDamages(damages);
    }

    public override void OnTakingDamages(float damages)
    {
        base.OnTakingDamages(damages);

        float newHp = _enemy.HpValue - damages;
        
        _enemy.HpValue = newHp;
        
       
    }

    private void Update()
    {
        
        _slider.value = _enemy.HpValue / _enemy.MaxHpValue;
    }
}
