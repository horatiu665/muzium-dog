namespace DogAI
{
    using System.Collections;
    using System.Collections.Generic;
    using PlantmanAI4;
    using ToyBoxHHH.ReadOnlyUtil;
    using UnityEngine;

    public class StateLook : MonoBehaviour, IState
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

        private DogLookingBrain dogLookingBrain => dogRefs.dogBrain.dogLookingBrain;

        private DogLookableObject _targetLookObject;

        public float updatePathfindingRate = 2f;
        private float _updatePathfindingTimer;

        public Vector2 moveSpeed01Range = new Vector2(0.5f, 1f);

        private float _totalTimeLooking = 0f;
        public float timeSpentLookingPerObject = 3f;

        public Vector2 maxTimeSpentLookingPerTurnRange = new Vector2(3, 12);
        private float maxTimeSpentLookingPerTurn;
        [ReadOnly, SerializeField]
        private float _lookStartTime;

        public float minTimeBetweenLookState = 7f;
        private float _lastLookStateTime;
        private bool _isLooking;

        private bool _isActive; // state

        private List<Collider> objectsLooked = new List<Collider>();

        string IState.GetName()
        {
            return "StateLook";
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
            objectsLooked.Clear();
            FindObjectToLook();
            _lookStartTime = 0;
            _isActive = true;
            maxTimeSpentLookingPerTurn = Random.Range(maxTimeSpentLookingPerTurnRange.x, maxTimeSpentLookingPerTurnRange.y);
        }

        private void FindObjectToLook()
        {
            var lo = dogLookingBrain.GetObjectToLookAt();
            _targetLookObject = lo;

        }

        void IState.OnExecute(float deltaTime)
        {
            Update_GoToLookable(deltaTime);
        }

        void Update_GoToLookable(float deltaTime)
        {
            if (_targetLookObject != null)
            {
                // set move speed
                var moveSpeed01 = Mathf.Lerp(moveSpeed01Range.x, moveSpeed01Range.y, Random.value);
                dogRefs.dogBrain.dogLocomotion.targetSpeed01 = moveSpeed01;

                dogRefs.dogBrain.dogLook.LookAt(_targetLookObject.transform, this);
            }
        }

        void IState.OnExit()
        {
            _isActive = false;
            _totalTimeLooking = 0;

            _lookStartTime = 0;
            _lastLookStateTime = Time.time;
            _isLooking = false;
            objectsLooked.Clear();

            dogRefs.dogBrain.dogLook.LookAt(null, this);

            dogRefs.dogBrain.dogVoice.Sniff(0f);


        }

        bool IState.ConditionsMet()
        {
            // if dog has lookable object in sight
            if (!dogLookingBrain.AnyLookables())
            {
                return false;
            }

            if (_isActive)
            {
                if (_totalTimeLooking > maxTimeSpentLookingPerTurn)
                {
                    return false;
                }
            }

            // if too soon to look again
            if (Time.time - _lastLookStateTime < minTimeBetweenLookState)
            {
                return false;
            }

            return true;

        }

    }
}