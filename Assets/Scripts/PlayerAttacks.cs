using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttacks : MonoBehaviour
{
    [SerializeField]
    Player _player;


    bool _canHit = false;
    bool _isOnCooldown = false;
    bool _canBlock = true;

    [SerializeField]
    List<DamageManager> _hittedEntities = new List<DamageManager>();

    public List<DamageManager> GetHittedEntities()
    {
        return _hittedEntities;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && _canHit && !_isOnCooldown)
        {
            _player.Inputs.attack = true;
            _isOnCooldown = true;
            StartCoroutine(AttackCooldown());
        }
        else if (context.canceled)
        {

            _player.Inputs.attack = false;
        }
    }

   

    public void OnBlock(InputAction.CallbackContext context)
    {
        if(context.performed && _canBlock)
        {
            _player.Inputs.block = true;
        }
        else if(context.canceled) 
        {
            _player.Inputs.block = false;
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

    IEnumerator AttackCooldown()
    {
        float timer = 0.5f;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        _isOnCooldown = false;

    }

}
