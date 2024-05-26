namespace DogAI
{
    using System.Collections;
    using System.Collections.Generic;
    using PlantmanAI4;
    using ToyBoxHHH;
    using UnityEngine;

    public class StateInFrontOfPlayer : MonoBehaviour, IState
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

        public float minTimeInFrontOfPlayerToEnd = 3f;
        private float timeInFrontOfPlayer;

        public float minTimeBetweenFollows = 3;

        public float followPathDelay = 0.5f;
        private float prevPathTime;

        public float targetSpeed01 = 1;
        public float targetSpeedWhenInFrontOfPlayer = 0.5f;
        private float curTargetSpeed01;

        public float frontOfPlayerDistance = 2.5f;

        public float maxPlayerExtrapolateVelocity = 5;

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

        public Transform playerCamera => dogRefs.dogBrain.mainCamera.transform;

        public DogBrain dogBrain => dogRefs.dogBrain;

        private float lastTimeThisStateWasActive = 0;
        private bool _isActive;

        [Space]
        public bool lookAtPlayerWhenInFront = false;
        public bool doSoundWhenInFront = false;
        public SmartSoundDog soundWhenInFront;

        string IState.GetName()
        {
            return "StateInFrontOfPlayer";
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
            lastTimeThisStateWasActive = Time.time;

        }

        void IState.OnExecute(float deltaTime)
        {
            lastTimeThisStateWasActive = Time.time;

            // how often to recalc path
            if (Time.time - prevPathTime > followPathDelay)
            {
                prevPathTime = Time.time;
                var playerFront = player.position;
                var playerY = player.position.y;
                playerFront += player.forward * frontOfPlayerDistance + Random.onUnitSphere * 0.4f * frontOfPlayerDistance;
                var pfv = dogBrain.playerFakeVelocity;
                var playerFakeVelocity = pfv.velocity;
                playerFront += Vector3.ClampMagnitude(playerFakeVelocity, maxPlayerExtrapolateVelocity);

                playerFront.y = playerY;

                // find nearest node to playerFront and use that Y to avoid airborne destination.
                var nearestNode = dogBrain.dogAstar.aStar.GetNearestNode(playerFront);
                playerFront.y = nearestNode.position.y;

                Debug.DrawLine(transform.position, playerFront, Color.yellow, 0.5f);

                dogRefs.dogBrain.dogAstar.SetDestination(playerFront);
                dogRefs.dogBrain.dogAstar.dogLocomotion.targetSpeed01 = curTargetSpeed01;
            }

            if (!IsInFrontOfPlayer())
            {
                timeInFrontOfPlayer = 0;
                curTargetSpeed01 = Mathf.Lerp(curTargetSpeed01, targetSpeed01, 0.1f);

                dogRefs.dogBrain.dogLook.LookAt(null, this);

            }
            else
            {
                timeInFrontOfPlayer += deltaTime;
                curTargetSpeed01 = Mathf.Lerp(curTargetSpeed01, targetSpeedWhenInFrontOfPlayer, 0.1f);

                if (lookAtPlayerWhenInFront)
                {
                    dogRefs.dogBrain.dogLook.LookAt(playerCamera.transform, this);
                }

                if (doSoundWhenInFront)
                {
                    if (!soundWhenInFront.audio.isPlaying)
                    {
                        soundWhenInFront.Play();
                    }
                }
            }

        }

        public bool IsInFrontOfPlayer()
        {
            var dir = transform.position - player.position;
            dir.y *= 0.2f; // care less for y axis distance
            if (Vector3.Dot(dir.normalized, playerCamera.forward) < 0.5f)
                return false;
            return true;
        }

        void IState.OnExit()
        {
            _isActive = false;
            timeInFrontOfPlayer = 0;
            lastTimeThisStateWasActive = Time.time;

            dogRefs.dogBrain.dogLook.LookAt(null, this);

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
                if (lastTimeThisStateWasActive != 0 && (Time.time - lastTimeThisStateWasActive < minTimeBetweenFollows))
                    return false;

                if (dist > minDistanceForStartFollow)
                    return true;

                return false;

            }
            else //if (_isActive)
            {
                // keep active until we are close enough to the player
                // if (dist < maxDistanceToEndFollow)
                //     return false;

                // keep active until we are in front of the player for long enough
                if (timeInFrontOfPlayer < minTimeInFrontOfPlayerToEnd)
                    return true;

                return false;
            }

        }

    }
}