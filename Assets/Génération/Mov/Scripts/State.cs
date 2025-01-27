using NaughtyAttributes;
using System;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    public bool HasBeenSet { get; protected set; } = false;
    public bool IsComplete { get; protected set; } = false;

    public StateMach mStateMach { get; set; }
    public virtual void StateSeter()
    {
        if (mStateMach == null)
        {
            mStateMach = GetComponent<StateMach>();
        }

        print(GetType().Name + " Set !");
        HasBeenSet = true;
    }
    public virtual void StateEnter()
    {
        print(GetType().Name + " Enter");

    }
    public abstract void StateUpdate();
    public virtual void StateExit()
    {
        print(GetType().Name + " State Exited");
    }

}
