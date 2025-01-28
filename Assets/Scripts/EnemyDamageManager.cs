using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

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
        OnTakingDamanges(damages);
    }

    public override void OnTakingDamanges(float damages)
    {
        base.OnTakingDamanges(damages);

        float newHp = _enemy.HpValue - damages;
        if (newHp <= 0)
        {
            _enemy.Death();
            newHp = 0;
        }
            
        _enemy.HpValue = newHp;

        
    }

    private void Update()
    {
        _slider.value = _enemy.HpValue / _enemy.MaxHpValue;
    }
}
