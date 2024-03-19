namespace DogAI
{
    using System.Collections;
    using System.Collections.Generic;
    using PlantmanAI4;
    using UnityEngine;

    public class Idle : MonoBehaviour, IState
    {

        private ControllerState _controller;
        public ControllerState controller
        {
            get
            {
                if (_controller == null)
                {
                    _controller = GetComponentInParent<ControllerState>();
                }
                return _controller;
            }
        }


        private DogRefs _dogRefs;
        public DogRefs dogRefs
        {
            get
            {
                if (_dogRefs == null)
                {
                    _dogRefs = GetComponentInParent<DogRefs>();
                }
                return _dogRefs;
            }
        }


        string IState.GetName()
        {
            return "Idle";
        }

        bool IState.GetUninterruptible()
        {
            return false;
        }

        float IState.GetPriority()
        {
            return 0;
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