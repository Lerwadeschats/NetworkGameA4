using ENet6;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{

    Player _player;

    Rigidbody2D _rb;

    float _moveDirection;

    [SerializeField]
    private InputActionReference _movement;


    [SerializeField]
    float _jumpForce, _jumpThreshold;

    bool _canJump;
    

    [SerializeField]
    float _gravityScaleFall = -1;

    float _baseGravityScale;


    //Dash
    bool _canDash;

    [SerializeField]
    float _dashForce;


    private void Awake()
    {
        _player = gameObject.GetComponent<Player>();
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _baseGravityScale = _rb.gravityScale;
        _canDash = true;
        _moveDirection = 1;

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && _canJump)
        {
            Vector2 newForce = _player.Velocity + Vector2.up * _jumpForce;
            _rb.AddForce(newForce);
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && _canDash)
        {
            Vector2 newForce = Vector2.right * _moveDirection * _dashForce;
            _rb.velocity = Vector2.zero;
            _rb.AddForce(newForce);
        }
    }

    private void FixedUpdate()
    {
        float moveInput = _movement.action.ReadValue<float>();

        if(moveInput > 0)
        {
            _moveDirection = 1;
           
        }
        else if (moveInput < 0)
        {
            _moveDirection = -1;
        }
        gameObject.transform.localScale = new Vector3(_moveDirection, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
        Vector2 newVelocity = new Vector2(moveInput * _player.Stats.speed, _rb.velocity.y);
        _rb.velocity = newVelocity;

        if (Mathf.Abs(_player.Velocity.y) > _jumpThreshold)
        {
            _canJump = false;
            if (_player.Velocity.y < 0)
            {

                _rb.gravityScale = _gravityScaleFall;
            }
            else
            {
                _rb.gravityScale = _baseGravityScale;
            }
            
        }
        else
        {
            _rb.gravityScale = _baseGravityScale;
            _canJump = true;
        }

        _player.Velocity = _rb.velocity;
        _player.Position = _rb.position;
        
    }
}
