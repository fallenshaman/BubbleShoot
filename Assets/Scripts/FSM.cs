using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AsyncModel;

public class State<STATE>
{
    public STATE Type { get; private set; }
    
    private Action actionOnEnter = () => { };
    private Action actionOnExit = () => { };

    public State(STATE eState, Action OnEnter = null, Action OnExit = null)
    {
        Type = eState;

        if (OnEnter != null)
            actionOnEnter = OnEnter;

        if (OnExit != null)
            actionOnExit = OnExit;
    }

    public void OnEnter()
    {
        actionOnEnter();
    }

    public void OnExit()
    {
        actionOnExit();
    }
}

public class FSM<STATE>
{
    private State<STATE> currentState = null;

    private Dictionary<STATE, State<STATE>> dicStates = new Dictionary<STATE, State<STATE>>();
    
    public void AddState(State<STATE> newState)
    {
        Debug.Assert(!dicStates.ContainsKey(newState.Type), string.Format("State [{0}] is already exist", newState.Type.ToString()));

        dicStates[newState.Type] = newState;
    }

    public void Start(STATE state)
    {
        ChangeState(state);
    }

    public STATE CurrentStateType() { return currentState.Type; }

    private bool inTransition = false;

    public bool IsInTransition { get { return inTransition; } }

    public void ChangeState(STATE state)
    {
        if (currentState != null && currentState.Type.Equals(state))
            return;

        State<STATE> nextState = null;

        if(!dicStates.TryGetValue(state, out nextState))
        {
            Debug.LogError("Can't find state : " + state.ToString());
            return;

        }
        
        if (currentState != null)
        {
            currentState.OnExit();
        }

        currentState = nextState;
        currentState.OnEnter();
    }
}