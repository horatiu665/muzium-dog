namespace DogAI
{
    using System.Collections;
    using System.Collections.Generic;
    using PlantmanAI4;
    using UnityEngine;

    public class StateWander : MonoBehaviour, IState
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

        public string stateName = "StateWander";

        public Transform player => dogRefs.dogBrain.player;

        public float priority = 1;
        public float priorityRandomAdd = 0.1f;
        private float nextPriority;

        public Vector2 wanderRange = new Vector2(5, 10);

        public Vector2 timeBetweenWanders = new Vector2(3, 5);
        private float nextWanderTime;

        // sets target speed so the dog wanders at walking pace or running.
        public float targetSpeed01 = 1;


        string IState.GetName()
        {
            return stateName;
        }

        bool IState.GetUninterruptible()
        {
            return false;
        }

        float IState.GetPriority()
        {
            nextPriority = Random.Range(0, priorityRandomAdd);

            return priority + nextPriority;
        }

        void IState.OnEnter()
        {
        }

        void IState.OnExecute(float deltaTime)
        {
            if (Time.time > nextWanderTime)
            {
                nextWanderTime = Time.time + Random.Range(timeBetweenWanders.x, timeBetweenWanders.y);

                // find a random position to go to
                float dist = Random.Range(wanderRange.x, wanderRange.y);
                Vector3 randomPos = transform.position + Random.insideUnitSphere * dist;

                var groundedPos = dogRefs.dogBrain.dogAstar.aStar.GetGroundedPosition(randomPos, out var grounded);
                if (grounded)
                {
                    dogRefs.dogBrain.dogAstar.SetDestination(groundedPos);
                    dogRefs.dogBrain.dogAstar.dogLocomotion.targetSpeed01 = targetSpeed01;
                }
            }
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