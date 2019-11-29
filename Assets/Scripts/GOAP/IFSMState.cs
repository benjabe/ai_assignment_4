using UnityEngine;
using System.Collections;

// Code from: https://github.com/sploreg/goap/blob/master/Assets/Standard%20Assets/Scripts/AI/FSM/FSMState.cs

public interface IFSMState 
{
    void Update(FSM fsm, GameObject gameObject);
}