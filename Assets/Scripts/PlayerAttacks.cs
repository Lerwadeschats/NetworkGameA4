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

    List<DamageManager> _hittedEntities = new List<DamageManager>();

    public List<DamageManager> GetHittedEntities()
    {
        return _hittedEntities;
    }
    public void OnAttack()
    {
        _player.Inputs.attack = true;
        _isOnCooldown = true;
        int attackID = Random.Range(0, 2);
        switch (attackID)
        {
            case 0:
                _player.animator.SetTrigger("Attack1");
                break;

            case 1:
                _player.animator.SetTrigger("Attack2");
                break;

            case 2:
                _player.animator.SetTrigger("Attack3");
                break;
        }

        StartCoroutine(AttackCooldown());
        
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) OnAttack();
        if (Input.GetMouseButtonDown(1)) OnBlock(true);

        if (Input.GetMouseButtonUp(0)) _player.Inputs.attack = false;
        if (Input.GetMouseButtonUp(1)) OnBlock(false);
    }

    /*public void OnBlock(InputAction.CallbackContext context)
    {
        if(context.performed && _canBlock)
        {
            _player.Inputs.block = true;
        }
        else if(context.canceled) 
        {
            _player.Inputs.block = false;
        }
    }*/

    public void OnBlock(bool isBlocking)
    {
        _player.Inputs.block = isBlocking;
        _player.animator.SetBool("IdleBlock", isBlocking);
        if (isBlocking)
        {
            _player.animator.SetTrigger("Block");
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
