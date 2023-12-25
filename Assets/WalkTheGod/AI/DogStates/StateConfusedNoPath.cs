namespace DogAI
{
    using System.Collections;
    using System.Collections.Generic;
    using PlantmanAI4;
    using UnityEngine;

    public class StateConfusedNoPath : MonoBehaviour, IState
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

        public float priority = 10;

        public float confusionTime = 1f;


        string IState.GetName()
        {
            return "StateConfusedNoPath";
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
            dogRefs.dogLocomotion.StopMovement();
            // look confusied?
        }

        void IState.OnExecute(float deltaTime)
        {
        }

        void IState.OnExit()
        {
        }

        bool IState.ConditionsMet()
        {
            if (Time.time - dogRefs.dogBrain.dogAstar.cantFindPathTime < confusionTime)
                return true;

            return false;
        }

    }
}