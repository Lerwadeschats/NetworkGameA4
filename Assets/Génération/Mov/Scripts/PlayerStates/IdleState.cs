using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    PlayerStateMach _playerSM;
    public override void StateSeter()
    {
        base.StateSeter();
        _playerSM = (PlayerStateMach)mStateMach;
    }
    public override void StateEnter()
    {
        base.StateEnter();
    }
    public override void StateUpdate()
    {
        if (_playerSM.NwIdentity.isClient && (Input.GetKeyDown(KeyCode.D)|| Input.GetKeyDown(KeyCode.Q))) 
        {
            mStateMach._animator.SetBool("IsMoving", true);
            this.mStateMach.ChangeState(this.mStateMach._AllState[1]);
        }
    }

}
