using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    Enemy _enemy;

    [SerializeField]
    List<PlayerDamageManager> _hittedPlayers;

    bool _canHit;

    [SerializeField]
    float _attackSpeed = 1f;

    bool _arePlayerInRange;

    private void Awake()
    {
        
        _canHit = true;
    }

    private void Update()
    {
        if(_arePlayerInRange && _canHit)
        {
            for (int i = 0; i < _hittedPlayers.Count; i++)
            {
                _hittedPlayers[i].OnBeingAttacked(_enemy.Attack);
                
            }
            _canHit = false;
            StartCoroutine(HitCooldown());
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            _arePlayerInRange = true;
            _hittedPlayers.Add(other.gameObject.GetComponent<PlayerDamageManager>());
            
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (_hittedPlayers.Count == 1)
            {
                _arePlayerInRange = false;
            }

            _hittedPlayers.Remove(other.gameObject.GetComponent<PlayerDamageManager>());
        }
    }

    IEnumerator HitCooldown()
    {
        float timer = _attackSpeed;
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        _canHit = true;

    }
}
