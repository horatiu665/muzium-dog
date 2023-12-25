using UnityEngine;
using System.Collections;
using PlantmanAI4;

public class StateIdle : MonoBehaviour, IState
{
    public string GetName()
    {
        return "Idle";
    }

    public bool GetUninterruptible()
    {
        return false;
    }

    public float GetPriority()
    {
        return 0;
    }

    public void OnEnter()
    {
    }

    public void OnExecute(float deltaTime)
    {
    }

    public void OnExit()
    {
    }

    public bool ConditionsMet()
    {
        return true;
    }
}
