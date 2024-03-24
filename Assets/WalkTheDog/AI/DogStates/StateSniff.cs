namespace DogAI
{
    using System.Collections;
    using System.Collections.Generic;
    using PlantmanAI4;
    using ToyBoxHHH.ReadOnlyUtil;
    using UnityEngine;

    public class StateSniff : MonoBehaviour, IState
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

        public DogSniffingBrain sniffBrain => dogRefs.dogBrain.dogSniffingBrain;

        public float priority = 10;

        private DogSniffableObject _targetSniffable;

        public float updatePathfindingRate = 2f;
        private float _updatePathfindingTimer;

        public Vector2 moveSpeed01Range = new Vector2(0.5f, 1f);


        private float _totalTimeSniffing = 0f;
        public float timeSpentSniffingPerObject = 3f;

        public Vector2 maxTimeSpentSniffingPerTurnRange = new Vector2(3, 12);
        private float maxTimeSpentSniffingPerTurn;
        [ReadOnly, SerializeField]
        private float _sniffStartTime;

        public float minTimeBetweenSniffState = 7f;
        private float _lastSniffStateTime;
        private bool _isSniffing;

        private bool _isActive; // state

        private List<DogSniffableObject> objectsSniffed = new List<DogSniffableObject>();

        string IState.GetName()
        {
            return "StateSniff";
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
            objectsSniffed.Clear();
            FindObjectToSniff();
            _sniffStartTime = 0;
            _isActive = true;
            maxTimeSpentSniffingPerTurn = Random.Range(maxTimeSpentSniffingPerTurnRange.x, maxTimeSpentSniffingPerTurnRange.y);
        }

        private void FindObjectToSniff()
        {
            _targetSniffable = sniffBrain.GetRandomSniffable();

            int monteCarloAttempts = 5;
            while (objectsSniffed.Contains(_targetSniffable) && monteCarloAttempts-- > 0)
            {
                _targetSniffable = sniffBrain.GetRandomSniffable();
            }

            var moveSpeed01 = Mathf.Lerp(moveSpeed01Range.x, moveSpeed01Range.y, Random.value);
            dogRefs.dogBrain.dogLocomotion.targetSpeed01 = moveSpeed01;
        }

        void IState.OnExecute(float deltaTime)
        {
            Update_GoToSniffable(deltaTime);
        }

        void Update_GoToSniffable(float deltaTime)
        {
            if (_targetSniffable != null)
            {
                var distToSniffable = Vector3.Distance(dogRefs.transform.position, _targetSniffable.sniffPosition);
                // if we're not close enough, walk there
                if (distToSniffable > sniffBrain.sniffDistance)
                {
                    dogRefs.dogBrain.dogAstar.SetDestination(_targetSniffable.sniffPosition);
                    sniffBrain.sniffAnimationTarget = null;

                    dogRefs.dogBrain.dogLook.LookAt(null, this);
                    dogRefs.dogBrain.dogVoice.Sniff(0f);

                }
                else
                // if we're close enough to the target.
                {
                    // if we aren't sniffing yet
                    if (!_isSniffing)
                    {
                        // start sniffing
                        _isSniffing = true;
                        _sniffStartTime = Time.time;

                        sniffBrain.sniffAnimationTarget = _targetSniffable;

                        var lookFwd = dogRefs.transform.forward;
                        dogRefs.dogBrain.dogLook.LookAtDirection(lookFwd, this);

                        dogRefs.dogBrain.dogVoice.Sniff(1f);

                    }
                    else if (_isSniffing)
                    {
                        // continue sniffing.
                        dogRefs.dogBrain.dogVoice.Sniff(1f);

                        // if time is out, find new sniff target.
                        if (Time.time - _sniffStartTime > timeSpentSniffingPerObject)
                        {
                            _totalTimeSniffing += timeSpentSniffingPerObject;

                            objectsSniffed.Add(_targetSniffable);
                            sniffBrain.sniffAnimationTarget = null;
                            _isSniffing = false;
                            FindObjectToSniff();

                        }
                    }
                }

            }
            else
            {
                // sniff in the air???
                sniffBrain.sniffAnimationTarget = null;
            }
        }

        void IState.OnExit()
        {
            _isActive = false;
            _totalTimeSniffing = 0;

            _sniffStartTime = 0;
            _lastSniffStateTime = Time.time;
            sniffBrain.sniffAnimationTarget = null;
            _isSniffing = false;
            objectsSniffed.Clear();

            dogRefs.dogBrain.dogLook.LookAt(null, this);

            dogRefs.dogBrain.dogVoice.Sniff(0f);


        }

        bool IState.ConditionsMet()
        {
            // if dog has sniffable object in sight
            if (!sniffBrain.AnySniffables())
            {
                return false;
            }

            if (_isActive)
            {
                if (_totalTimeSniffing > maxTimeSpentSniffingPerTurn)
                {
                    return false;
                }
            }

            // if too soon to sniff again
            if (Time.time - _lastSniffStateTime < minTimeBetweenSniffState)
            {
                return false;
            }

            return true;

        }

    }
}