using System.Collections;
using UnityEngine;

public class PatrolState : State
{
    public float _maxSpeed = 5;
    public float _accSpeed = 5f;
    public float _turnTime = 1;

    public float _detectionRange = 4;

    

    EnemyStateMach _enemyStateMach = null;
    // 0-1 : Down R-L || 2-3 : Right Left 
    readonly Vector2[] rays = new Vector2[4] { new Vector2(1, -1), new Vector2(-1, -1), new Vector2(1, 0), new Vector2(-1, 0) };
    Coroutine _coroutine;

    public override void StateSeter()
    {
        base.StateSeter();
        _enemyStateMach = (EnemyStateMach)mStateMach;
    }
    public override void StateEnter()
    {
        base.StateEnter();
        _enemyStateMach.CanMove = true;
    }
    public override void StateUpdate()
    {
        #region DetectPlayers
        RaycastHit2D playerHit;
        if(!_enemyStateMach.IsGoingLeft)
        {
            playerHit = Physics2D.Raycast(new Vector2(transform.position.x + transform.localScale.x / 1.9f, transform.position.y), rays[2], _detectionRange);
            Debug.DrawLine(new Vector2(transform.position.x + transform.localScale.x / 1.9f, transform.position.y), new Vector2(transform.position.x+ _detectionRange, transform.position.y) , Color.red);

        }
        else
        {
            playerHit = Physics2D.Raycast(new Vector2(transform.position.x - transform.localScale.x / 1.9f, transform.position.y), rays[3], _detectionRange);
            Debug.DrawLine(new Vector2(transform.position.x - transform.localScale.x / 1.9f, transform.position.y), new Vector2(transform.position.x - _detectionRange, transform.position.y), Color.red);
        }

        if (playerHit && playerHit.collider.CompareTag("Player"))
        {
            _enemyStateMach._playerToFollow = playerHit.transform.gameObject;
            _enemyStateMach.ChangeState(mStateMach._AllState[1]);
            return;
        }
        #endregion
        #region Movement
        RaycastHit2D groundHit;
        if (!_enemyStateMach.IsGoingLeft)
        {
            groundHit = Physics2D.Raycast(new Vector2(transform.position.x + transform.localScale.x / 1.9f, transform.position.y), rays[0], 1);
            if (groundHit) Debug.DrawLine(new Vector2(transform.position.x + transform.localScale.x / 1.9f, transform.position.y), groundHit.point, Color.blue);

        }
        else
        {
            groundHit = Physics2D.Raycast(new Vector2(transform.position.x - transform.localScale.x / 1.9f, transform.position.y), rays[1], 1);
            if (groundHit) Debug.DrawLine(new Vector2(transform.position.x - transform.localScale.x / 1.9f, transform.position.y), groundHit.point, Color.blue);
        }


        if (!groundHit)
        {
            if (_coroutine == null) _coroutine = StartCoroutine(ChangeDir());
        }
        else if (_enemyStateMach.CanMove)
        {
            if (groundHit.normal == Vector2.up)
            {
                if (_enemyStateMach.Speed < _maxSpeed) _enemyStateMach.Speed += _accSpeed * Time.deltaTime;
                _enemyStateMach.Rb.velocity = _enemyStateMach.IsGoingLeft ? new Vector2(-_enemyStateMach.Speed, _enemyStateMach.Rb.velocity.y) : new Vector2(_enemyStateMach.Speed, _enemyStateMach.Rb.velocity.y);
            }
            else if (groundHit.normal == Vector2.left || groundHit.normal == Vector2.right)
            {
                if (_coroutine == null) _coroutine = StartCoroutine(ChangeDir());
            }
        }
        else
        {
            if (_enemyStateMach.Speed > 0) _enemyStateMach.Speed -= _accSpeed * Time.deltaTime;
        }
        #endregion
    }
    public override void StateExit()
    {
        base.StateExit();
        StopAllCoroutines();
    }

    public IEnumerator ChangeDir()
    {
        _enemyStateMach.CanMove = false;
        yield return new WaitForSeconds(_turnTime);
        _enemyStateMach.IsGoingLeft = !_enemyStateMach.IsGoingLeft;
        _enemyStateMach.CanMove = true;
        yield return new WaitForSeconds(_turnTime / 2);
        _coroutine = null;
    }
}
