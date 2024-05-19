namespace DogAI
{
    using System.Collections;
    using System.Collections.Generic;
    using PlantmanAI4;
    using UnityEngine;

    public class StateFollowPlayer : MonoBehaviour, IState
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


        public float priority = 1;

        public float minDistanceForStartFollow = 5;
        public float maxDistanceToEndFollow = 2f;

        public float minTimeBetweenFollows = 3;

        public float followPathDelay = 0.5f;
        private float prevPathTime;

        public float targetSpeed01 = 1;

        public float frontOfPlayerDistance = 2.5f;

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

        public float minStateActiveDuration = 5f;

        private float lastTimeThisStateWasActive = 0;
        private float stateEnterTime;
        private bool _isActive;


        string IState.GetName()
        {
            return "StateFollowPlayer";
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
            _isActive = true;
            stateEnterTime = Time.time;
            lastTimeThisStateWasActive = Time.time;

        }

        void IState.OnExecute(float deltaTime)
        {
            lastTimeThisStateWasActive = Time.time;

            if (Time.time - prevPathTime > followPathDelay)
            {
                prevPathTime = Time.time;
                var playerFront = player.position;
                var playerY = player.position.y;
                playerFront += player.forward * frontOfPlayerDistance + Random.onUnitSphere * 0.4f * frontOfPlayerDistance;

                playerFront.y = playerY;

                Debug.DrawLine(transform.position, playerFront, Color.yellow, 0.5f);

                dogRefs.dogBrain.dogAstar.SetDestination(playerFront);
                dogRefs.dogBrain.dogAstar.dogLocomotion.targetSpeed01 = targetSpeed01;
            }
        }

        void IState.OnExit()
        {
            _isActive = false;
            lastTimeThisStateWasActive = Time.time;

        }

        bool IState.ConditionsMet()
        {
            if (player == null)
                return false;

            var dir = transform.position - player.position;
            dir.y *= 0.2f; // care less for y axis distance
            var dist = dir.magnitude;

            // if state not active 
            if (!_isActive)
            {
                // wait between follows
                if (Time.time - lastTimeThisStateWasActive < minTimeBetweenFollows)
                    return false;

                if (dist > minDistanceForStartFollow)
                    return true;
                return false;

            }
            else //if (_isActive)
            {
                if (dist < maxDistanceToEndFollow)
                    return false;
                // keep active for a certain minimum duration
                if (Time.time - stateEnterTime < minStateActiveDuration)
                    return true;
                // keep active until we are close enough to the player
                return true;
            }

        }

    }
}