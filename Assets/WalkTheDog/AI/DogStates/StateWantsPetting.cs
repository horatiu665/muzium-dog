namespace DogAI
{
    using System.Collections;
    using System.Collections.Generic;
    using PlantmanAI4;
    using UnityEngine;

    public class StateWantsPetting : MonoBehaviour, IState
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

        public float recalculatePathDelay = 0.5f;
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


        string IState.GetName()
        {
            return "StateWantsPetting";
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

            if (dogRefs.dogBrain.dogPettingBrain.IsBeingPetted())
            {
                dogRefs.dogBrain.dogAstar.StopMovement();

                // look at player while petting.
                // maybe change look target to random...??? sometimes ????
                dogRefs.dogBrain.dogLook.LookAt(dogBrain.mainCamera.transform);


                // be happy for getting pets
                dogRefs.dogBrain.dogEmotionBrain.AddHappiness(this, 10f);
                return;
            }

            if (Time.time - prevPathTime > recalculatePathDelay)
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

                // unless the player is looking over a ledge.... thus maybe we should raycast down from the front of the player.
                //

                Debug.DrawLine(transform.position, playerFront, Color.yellow, 0.5f);

                dogRefs.dogBrain.dogAstar.SetDestination(playerFront);
                dogRefs.dogBrain.dogAstar.dogLocomotion.targetSpeed01 = curTargetSpeed01;
            }

            if (!IsInFrontOfPlayer())
            {
                timeInFrontOfPlayer = 0;
                curTargetSpeed01 = Mathf.Lerp(curTargetSpeed01, targetSpeed01, 0.1f);

                dogRefs.dogBrain.dogLook.LookAt(null);
            }
            else
            {
                timeInFrontOfPlayer += deltaTime;
                curTargetSpeed01 = Mathf.Lerp(curTargetSpeed01, targetSpeedWhenInFrontOfPlayer, 0.1f);

                // look at player
                dogRefs.dogBrain.dogLook.LookAt(dogBrain.mainCamera.transform);

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

        }

        bool IState.ConditionsMet()
        {
            if (player == null)
                return false;

            if (dogRefs.dogBrain.dogPettingBrain.IsBeingPetted())
            {
                return true;
            }

            // if we don't have enough pets, we want pets
            return dogRefs.dogBrain.dogPettingBrain.pettingNeed > 0.7f;

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
                // keep active until we are close enough to the player
                // if (dist < maxDistanceToEndFollow)
                //     return false;

                // keep active until we are in front of the player for long enough
                if (timeInFrontOfPlayer < minTimeInFrontOfPlayerToEnd)
                    return true;

                return false;
            }

        }


        private void OnGUI()
        {
            if (_isActive)
            {
                GUI.Label(new Rect(10, 10, 200, 20), "front: " + timeInFrontOfPlayer.ToString("F2"));
            }
        }
    }
}