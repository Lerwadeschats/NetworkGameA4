using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateMach : MonoBehaviour
{
    public State _current = null;
    public Animator _animator = null;

    public List<State> _AllState = new List<State>();

    public List<State> GetAllStates() { return _AllState = GetComponents<State>().ToList(); }
    
    public virtual void Start()
    {
        _AllState = GetAllStates();
        _AllState.Where(a => a.enabled = false);
        _AllState[0].enabled = true;
        
        _animator = GetComponent<Animator>();
        if (_current == null ) _current = _AllState[0];

        if (!_current.HasBeenSet) _current.StateSeter();
        _current.StateEnter();

    }
    // Update is called once per frame
    public virtual void Update()
    {
        _current.StateUpdate();
    }
    public State ChangeState(State newState)
    {
        _current.StateExit();
        _current.enabled = false;
        _current = newState;
        _current.enabled = true;
        if (!_current.HasBeenSet) _current.StateSeter();
        _current.StateEnter();
        return newState;
    }

    [Button]
    private void SetAllStates()
    {
        _AllState = GetAllStates();
    }
}
