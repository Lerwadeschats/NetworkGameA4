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

    bool _isOnGround;
    public Rigidbody2D Rb { get => _rb; set => _rb = value; }
    public bool IsOnGround { get => _isOnGround; set => _isOnGround = value; }

    private void Awake()
    {
        _player = gameObject.GetComponent<Player>();
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _baseGravityScale = _rb.gravityScale;
        _canDash = true;
        _moveDirection = 1;
        _baseScaleX = transform.localScale.x;

    }

    /*public void OnJump(InputAction.CallbackContext context)
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
    }*/

    /*public void OnDash(InputAction.CallbackContext context)
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
    }*/

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D)) _player.Inputs.moveRight = true;
        if (Input.GetKeyDown(KeyCode.A)) _player.Inputs.moveLeft = true;
        if (Input.GetKeyDown(KeyCode.LeftShift)) _player.Inputs.dash = true;
        if (Input.GetKeyDown(KeyCode.Space)) _player.Inputs.jump = true;
        



        if (Input.GetKeyUp(KeyCode.D)) _player.Inputs.moveRight = false;
        if (Input.GetKeyUp(KeyCode.A)) _player.Inputs.moveLeft = false;
        if (Input.GetKeyUp(KeyCode.LeftShift)) _player.Inputs.dash = false;
        if (Input.GetKeyUp(KeyCode.Space)) _player.Inputs.jump = false;
        


        UpdatePhysics();
        return;

        /*print("hey");
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

            *//*_rb.velocity = _player.Velocity;
            transform.position = _player.Position;*//*
            //print("Velocity : " + _player.Velocity + " // Pos : " + _player.Position);
        }*/

    }

    public void UpdatePhysics()
    {
        

        float moveInput = 1;

        if (_player.Inputs.moveRight && !_player.Inputs.moveLeft)   
        {
            _player.animator.SetInteger("AnimState", 1);
            _moveDirection = 1;
            moveInput = 1;
        }
        else if (!_player.Inputs.moveRight && _player.Inputs.moveLeft)
        {
            _player.animator.SetInteger("AnimState", 1);
            _moveDirection = -1;
            moveInput = -1;
        }
        else if(!_player.Inputs.moveRight && !_player.Inputs.moveLeft)
        {
            _player.animator.SetInteger("AnimState", 0);
            moveInput = 0;
        }

        gameObject.transform.localScale = new Vector3(_baseScaleX * _moveDirection, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
        Vector2 newVelocity = new Vector2(moveInput * _player.Stats.speed, _rb.velocity.y);
        _rb.velocity = newVelocity;

        print(_canJump);

        if (!_isOnGround)
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


        //Check Velocity Y pour anim
        _player.animator.SetBool("Grounded", _isOnGround);
        
        _player.animator.SetFloat("AirSpeedY", _player.Velocity.y);

        if (_canDash && _player.Inputs.dash)
        {
            Vector2 newForce = new Vector2(_moveDirection * _dashForce, 0);
            _rb.velocity = Vector2.zero;
            _rb.AddForce(newForce);
        }

        if (_canJump && _player.Inputs.jump)
        {
            _player.animator.SetTrigger("Jump");
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
