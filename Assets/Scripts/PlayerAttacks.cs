using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttacks : MonoBehaviour
{
    [SerializeField]
    Player _player;


    bool _canHit = false;

    [SerializeField]
    List<DamageManager> _hittedEntities = new List<DamageManager>();

    public void OnAttack(InputAction.CallbackContext context)
    {
        if(context.performed && _canHit)
        {
            foreach(DamageManager entity in _hittedEntities)
            {
                entity.OnTakingDamanges(_player.Stats.attackValue);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy" || (other.gameObject.tag == "Player" && other.gameObject != _player.gameObject))
        {
            _canHit = true;
            
            _hittedEntities.Add(other.gameObject.GetComponent<DamageManager>());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy" || (other.gameObject.tag == "Player" && other.gameObject != _player.gameObject))
        {
            if (_hittedEntities.Count == 1)
            {
                _canHit = false;
            }

            _hittedEntities.Remove(other.gameObject.GetComponent<DamageManager>());
        }
    }

}
