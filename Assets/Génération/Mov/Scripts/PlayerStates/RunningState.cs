using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningState : State
{
    public float _acceleration = 5f;
    public float _maxSpeed = 5;
    public int _turnMinus = 4;

    

    private PlayerStateMach _playerSM;

    public override void StateSeter()
    {
        base.StateSeter();
        _playerSM = (PlayerStateMach)mStateMach;

    }
    public override void StateUpdate()
    {
        if (_playerSM.NwIdentity.isClient)
        {

        }
        if (Input.GetKey(KeyCode.D))
        {
            if (!_playerSM.FacingRight && _playerSM.Speed > 3) _playerSM.Speed -= _turnMinus;
            _playerSM.FacingRight = true;
            if (_playerSM.Speed < _maxSpeed) _playerSM.Speed += _acceleration * Time.deltaTime;
            mStateMach._animator.SetBool("IsMoving", true);

        }
        else if (Input.GetKey(KeyCode.Q))
        {
            if (_playerSM.FacingRight && _playerSM.Speed > 3) _playerSM.Speed -= _turnMinus;
            _playerSM.FacingRight = false;
            if (_playerSM.Speed < _maxSpeed) _playerSM.Speed += _acceleration * Time.deltaTime;
            mStateMach._animator.SetBool("IsMoving", true);

        }
        else if (_playerSM.Speed > 0) { _playerSM.Speed -= _acceleration * _acceleration * _acceleration * Time.deltaTime; if (_playerSM.Speed < 0) _playerSM.Speed = 0; }
        else
        {
            _playerSM.Speed = 0;
            mStateMach._animator.SetBool("IsMoving", false);
            mStateMach.ChangeState(mStateMach._AllState[0]);        
        }

        _playerSM.Rb.velocity = _playerSM.FacingRight ? new Vector2(_playerSM.Speed, _playerSM.Rb.velocity.y) : new Vector2(-_playerSM.Speed, _playerSM.Rb.velocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && _playerSM.JumpLeft > 0)
        {
            _playerSM.JumpLeft--;
            _playerSM.Rb.velocity = new Vector2(_playerSM.Rb.velocity.x, _playerSM.JumpForce);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!HasBeenSet) { return; }
        if (collision.gameObject.CompareTag("ground"))
        {
            Vector2 normal = collision.contacts[0].normal;
            if (normal == Vector2.up)
            {
                _playerSM.JumpLeft = _playerSM.NbJump;
            }
            else if (normal == Vector2.left || normal == Vector2.right)
            {
                if (_playerSM.JumpLeft < 1) _playerSM.JumpLeft = 1;
                _playerSM.Speed = 0;
            }
        }
    }
}
