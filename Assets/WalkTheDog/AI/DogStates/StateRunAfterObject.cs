namespace DogAI
{
    using System.Collections;
    using System.Collections.Generic;
    using PlantmanAI4;
    using UnityEngine;

    public class StateRunAfterObject : MonoBehaviour, IState
    {
        public float priority = 1;

        string IState.GetName()
        {
            return "StateRunAfterObject";
        }

        bool IState.GetUninterruptible()
        {
            return false;
        }

        float IState.GetPriority()
        {
            return priority;
        }

        void IState.OnEnter()
        {
        }

        void IState.OnExecute(float deltaTime)
        {
        }

        void IState.OnExit()
        {
        }

        bool IState.ConditionsMet()
        {
            return true;
        }

    }
}