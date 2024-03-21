using System.Threading;
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

        public float confusionTime = 3f;

        private float lookAroundConfusedTime;

        public Vector2 whimperDelayRange = new Vector2(5, 10);
        private float whimperTime;

        public float whimperChance = 0.7f;


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
            dogRefs.dogBrain.dogAstar.StopMovement();
            dogRefs.dogLocomotion.StopMovement();
            dogRefs.dogLocomotion.StopRotation();

            // look confused?
            LookAtRandomPlace();
            if (Random.value < whimperChance)
            {
                dogRefs.dogBrain.dogVoice.Whimper();
            }
            whimperTime = Time.time + Random.Range(whimperDelayRange.x, whimperDelayRange.y);
        }

        private void LookAtRandomPlace()
        {
            dogRefs.dogBrain.dogLook.LookAtDirection(Random.onUnitSphere);
            lookAroundConfusedTime = Time.time + Mathf.Lerp(0.5f, 1.5f, Random.value);
        }

        void IState.OnExecute(float deltaTime)
        {
            if (Time.time > lookAroundConfusedTime)
            {
                LookAtRandomPlace();
            }

            if (Time.time > whimperTime)
            {
                if (Random.value < whimperChance)
                {
                    dogRefs.dogBrain.dogVoice.Whimper();
                }
                whimperTime = Time.time + Random.Range(whimperDelayRange.x, whimperDelayRange.y);
            }
        }

        void IState.OnExit()
        {
            dogRefs.dogBrain.dogLook.LookAt(null);
        }

        bool IState.ConditionsMet()
        {
            if (Time.time - dogRefs.dogBrain.dogAstar.cantFindPathTime < confusionTime)
                return true;

            return false;
        }

    }
}