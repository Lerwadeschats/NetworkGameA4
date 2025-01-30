using ENet6;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Player;
using UnityEngine.UIElements;

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

    float _baseScaleX = 1;

    float moveDirection = 1;
    private void Awake()
    {
        _player = gameObject.GetComponent<Player>();
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _baseGravityScale = _rb.gravityScale;
        _canDash = true;
        _moveDirection = 1;
        _baseScaleX = transform.localScale.x;

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        return;
        if (context.performed && _canJump)
        {
            _player.Inputs.jump = true;

            
        }
        else if (context.canceled)
        {
            _player.Inputs.jump = false;
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        return;
        if (context.performed && _canDash)
        {
            _player.Inputs.dash = true;

            
        }
        else if (context.canceled)
        {
            _player.Inputs.dash = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D)) _player.Inputs.moveRight = true;
        if (Input.GetKeyDown(KeyCode.A)) _player.Inputs.moveLeft = true;
        if (Input.GetKeyDown(KeyCode.LeftShift)) _player.Inputs.dash = true;
        if (Input.GetKeyDown(KeyCode.Space)) _player.Inputs.jump = true;
        if (Input.GetMouseButtonDown(0)) _player.Inputs.attack = true;
        if (Input.GetMouseButtonDown(1)) _player.Inputs.block = true;



        if (Input.GetKeyUp(KeyCode.D)) _player.Inputs.moveRight = false;
        if (Input.GetKeyUp(KeyCode.A)) _player.Inputs.moveLeft = false;
        if (Input.GetKeyUp(KeyCode.LeftShift)) _player.Inputs.dash = false;
        if (Input.GetKeyUp(KeyCode.Space)) _player.Inputs.jump = false;
        if (Input.GetMouseButtonUp(0)) _player.Inputs.attack = false;
        if (Input.GetMouseButtonUp(1)) _player.Inputs.block = false;
        return;

        print("hey");
        float moveInput = _movement.action.ReadValue<float>();
        if(_player != null && _player.Inputs != null)
        {
            if (moveInput > 0)
            {
                _moveDirection = 1;
                _player.Inputs.moveRight = true;
                _player.Inputs.moveLeft = false;

            }
            else if (moveInput < 0)
            {
                _moveDirection = -1;
                _player.Inputs.moveRight = false;
                _player.Inputs.moveLeft = true;
            }
            else
            {
                _player.Inputs.moveRight = false;
                _player.Inputs.moveLeft = false;
            }

            /*_rb.velocity = _player.Velocity;
            transform.position = _player.Position;*/
            //print("Velocity : " + _player.Velocity + " // Pos : " + _player.Position);
        }

    }

    public void UpdatePhysics()
    {
        

        float moveInput = 1;

        if (_player.Inputs.moveRight && !_player.Inputs.moveLeft)   
        {
            moveDirection = 1;
            moveInput = 1;
        }
        else if (!_player.Inputs.moveRight && _player.Inputs.moveLeft)
        {
            moveDirection = -1;
            moveInput = -1;
        }
        else if(!_player.Inputs.moveRight && !_player.Inputs.moveLeft)
        {
            moveInput = 0;
        }

        gameObject.transform.localScale = new Vector3(_baseScaleX * moveDirection, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
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

        if (_canDash && _player.Inputs.dash)
        {
            Vector2 newForce = Vector2.right * _moveDirection * _dashForce;
            _rb.velocity = Vector2.zero;
            _rb.AddForce(newForce);
        }

        if (_canJump && _player.Inputs.jump)
        {
            Vector2 newForce = _player.Velocity + Vector2.up * _jumpForce;
            _rb.AddForce(newForce);
        }



        _player.Velocity = _rb.velocity;
        _player.Position = transform.position;


    }

    public void UpdateVelocity(Vector2 velocity)
    {
        _rb.velocity = velocity;
    }

    public void UpdatePosition(Vector2 position)
    {
        transform.position = position;
    }

    public void UpdateStats(PlayerStats stats)
    {
        PlayerStats newStats = new PlayerStats(stats.attackValue, stats.hpValue, stats.maxHpValue, stats.speed);
        _player.Stats = newStats;
    }
}
