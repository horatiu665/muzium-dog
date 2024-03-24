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

        public float minTimeBetweenStateActive = 3;

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

        private float _lastTimeThisStateWasActive = 0;
        private bool _isActive;

        private float _pettingNeedOnStartPetting;

        public float pettingStopDelayAmount = 2f;
        private float _pettingStopTime;

        public float timeWithoutPetsBeforeGivingUp = 10f;
        private float _stateEnterTime;

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
            _lastTimeThisStateWasActive = Time.time;
            _stateEnterTime = Time.time;

            _pettingNeedOnStartPetting = dogRefs.dogBrain.dogPettingBrain.pettingNeed;
        }


        void IState.OnExecute(float deltaTime)
        {
            _lastTimeThisStateWasActive = Time.time;

            if (dogRefs.dogBrain.dogPettingBrain.IsBeingPetted())
            {
                dogRefs.dogBrain.dogAstar.StopMovement();

                // look at player while petting.
                // maybe change look target to random...??? sometimes ????
                dogRefs.dogBrain.dogLook.LookAt(dogBrain.mainCamera.transform, this);

                // be happy for getting pets
                dogRefs.dogBrain.dogEmotionBrain.AddHappiness(this, 10f);

                dogRefs.dogBrain.dogVoice.Pant(1f);

                _pettingStopTime = Time.time + pettingStopDelayAmount;
                return;
            }
            else if (_pettingStopTime > Time.time)
            {
                // keep panting and sitting still
                dogRefs.dogBrain.dogAstar.StopMovement();
                dogRefs.dogBrain.dogLook.LookAt(dogBrain.mainCamera.transform, this);
                dogRefs.dogBrain.dogVoice.Pant(1f);
                return;
            }
            else
            {
                dogRefs.dogBrain.dogVoice.Pant(0);

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

                dogRefs.dogBrain.dogLook.LookAt(null, this);
            }
            else
            {
                timeInFrontOfPlayer += deltaTime;
                curTargetSpeed01 = Mathf.Lerp(curTargetSpeed01, targetSpeedWhenInFrontOfPlayer, 0.1f);

                // look at player
                dogRefs.dogBrain.dogLook.LookAt(dogBrain.mainCamera.transform, this);

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
            _lastTimeThisStateWasActive = Time.time;

            dogRefs.dogBrain.dogVoice.Pant(0);

            dogRefs.dogBrain.dogLook.LookAt(null, this);

            // was satisfied?
            var pettingNeedAfter = dogRefs.dogBrain.dogPettingBrain.pettingNeed;
            var pettingNeedChange = pettingNeedAfter - _pettingNeedOnStartPetting;
            // we want pettingNeed to be reduced, to be effective.
            if (pettingNeedChange < -1.5f)
            {
                dogRefs.dogBrain.dogVoice.BarkHappy();
            }
            else if (pettingNeedChange < -0.5f)
            {
                dogRefs.dogBrain.dogVoice.BarkIntensity(Random.Range(0, 0.5f));
            }

        }

        bool IState.ConditionsMet()
        {
            if (player == null)
                return false;

            if (dogRefs.dogBrain.dogPettingBrain.IsBeingPetted())
            {
                return true;
            }

            // if we have a petting stop time, the dog waits before it leaves the petting state.
            if (_pettingStopTime > Time.time)
            {
                return true;
            }

            // if state not active 
            if (!_isActive)
            {
                // wait between follows
                if (Time.time - _lastTimeThisStateWasActive < minTimeBetweenStateActive)
                    return false;

            }
            else // if state is active
            {
                // if we're active for too long without pets maybe give up for a while.
                if (Time.time - _stateEnterTime > timeWithoutPetsBeforeGivingUp)
                {
                    return false;
                }

            }

            // if we don't have enough pets, we want pets
            return dogRefs.dogBrain.dogPettingBrain.pettingNeed > 0.7f;

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