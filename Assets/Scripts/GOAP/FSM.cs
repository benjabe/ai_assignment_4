using System.Collections.Generic;
using UnityEngine;
using System.Collections;

// Code from: https://github.com/sploreg/goap/blob/master/Assets/Standard%20Assets/Scripts/AI/FSM/FSM.cs

/**
 * Stack-based Finite State Machine.
 * Push and pop states to the FSM.
 * 
 * States should push other states onto the stack 
 * and pop themselves off.
 */
using System;


public class FSM {

    private Stack<IFSMState> stateStack = new Stack<IFSMState>();

    public delegate void IFSMState(FSM fsm, GameObject gameObject);


    public void Update(GameObject gameObject) {
        if (stateStack.Peek() != null)
            stateStack.Peek().Invoke(this, gameObject);
    }

    public void PushState(IFSMState state) {
        stateStack.Push(state);
    }

    public void PopState() {
        stateStack.Pop();
    }
}