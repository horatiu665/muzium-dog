namespace DogAI
{
    using System.Collections;
    using System.Collections.Generic;
    using PlantmanAI4;
    using UnityEngine;

    public class StateKinematicDespair : MonoBehaviour, IState
    {
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

        public Transform player => dogRefs.dogBrain.player;

        public float minPriority = 1;
        public float maxPriority = 19;

        public float priorityBasedOnTimeStandingStillWithAPath = 0.1f;

        private float _timeStandingStill = 0;

        public float minTimeBetweenKinematicChanges = 5f;
        private float _timeOfLastKinematicChange = 0;

        string IState.GetName()
        {
            return "StateKinematicDespair";
        }

        bool IState.GetUninterruptible()
        {
            return false;
        }

        float IState.GetPriority()
        {
            return Mathf.Min(maxPriority, minPriority + priorityBasedOnTimeStandingStillWithAPath * _timeStandingStill);
        }

        private void Update()
        {
            Update_TimeStandStill();

        }

        private void Update_TimeStandStill()
        {
            // if not kinematic
            if (!dogRefs.dogLocomotion.rbRoot.isKinematic)
            {
                // if has destination
                if (dogRefs.dogLocomotion.hasDestination)
                {
                    // if we are standing still while supposed to be moving.
                    if (dogRefs.dogLocomotion.targetSpeed01 > 0.1f && dogRefs.dogLocomotion.currentSpeed01 < 0.1f)
                    {
                        _timeStandingStill += Time.deltaTime;

                        return;
                    }
                }
            }
            else // if kinematic...
            {
                if (dogRefs.dogLocomotion.currentSpeed01 <= 0.1f)
                {
                    _timeStandingStill += Time.deltaTime;
                    return;
                }
            }

            // we are moving prob. so cancel this.
            _timeStandingStill = 0;
        }

        void IState.OnEnter()
        {
            // toggle kinematic status.
            dogRefs.dogLocomotion.SetKinematic(!dogRefs.dogLocomotion.rbRoot.isKinematic);

            _timeOfLastKinematicChange = Time.time;
        }

        void IState.OnExecute(float deltaTime)
        {
        }

        void IState.OnExit()
        {
        }

        bool IState.ConditionsMet()
        {
            if (Time.time - _timeOfLastKinematicChange < minTimeBetweenKinematicChanges)
            {
                return false;
            }

            // if we're kinematic, we want to randomly revert after some time.
            if (dogRefs.dogLocomotion.rbRoot.isKinematic)
            {
                return true;
            }

            // if we are standing still while having a path for too long,
            // we want to try and switch kinematic status because it can be a cause for getting stuck.

            return dogRefs.dogLocomotion.allowKinematicControl;
        }

    }
}