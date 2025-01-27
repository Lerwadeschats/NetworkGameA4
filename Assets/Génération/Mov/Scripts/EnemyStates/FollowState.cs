using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class FollowState : State
{
    public float _maxSpeed = 7.5f;
    public float _accSpeed = 7.5f;
    public float _turnTime = .2f;
    [Space(10)]
    public float _circleDetection = 7;
    public float _circleAttackDetection = 2;


    Transform _playerToFollow = null;
    public bool IsGoingLeft() { return true; }
    EnemyStateMach _enemyStateMach;
    public override void StateSeter()
    {
        base.StateSeter();
        _enemyStateMach = (EnemyStateMach)mStateMach;
    }
    public override void StateEnter()
    {
        base.StateEnter();
        mStateMach._animator.SetTrigger("EnemyTrigger");
        _playerToFollow = _enemyStateMach._playerToFollow.transform;
    }

    public override void StateExit()
    {
        base.StateExit();
        StopAllCoroutines();
    }

    public override void StateUpdate()
    {
        _enemyStateMach.IsGoingLeft = (transform.position.x - _playerToFollow.position.x) > 0;
        float dist = Vector2.Distance(this.transform.position, _playerToFollow.position);
        if(dist < _circleAttackDetection)
        {
            StartCoroutine(AttackTime());
            _enemyStateMach.CanMove = false;
            mStateMach._animator.SetTrigger("Attack");
        }
        if(dist < _circleDetection)
        {
            if (_enemyStateMach.CanMove)
            {
                if (_enemyStateMach.Speed < _maxSpeed) _enemyStateMach.Speed += _accSpeed * Time.deltaTime;
                _enemyStateMach.Rb.velocity = _enemyStateMach.IsGoingLeft ? new Vector2(-_enemyStateMach.Speed, _enemyStateMach.Rb.velocity.y) : new Vector2(_enemyStateMach.Speed, _enemyStateMach.Rb.velocity.y);
            }
            else
            {
                _enemyStateMach.Speed = 0;
            }
        }
        else
        {
            mStateMach.ChangeState(mStateMach._AllState[0]);
        }
    }
    private IEnumerator AttackTime() 
    { 
        yield return new WaitForSeconds(1.2f);
        _enemyStateMach.CanMove = true;
    }
    private void OnDrawGizmos()
    {
        if (enabled)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(this.transform.position, _circleDetection);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, _circleAttackDetection);

        }
    }
}
