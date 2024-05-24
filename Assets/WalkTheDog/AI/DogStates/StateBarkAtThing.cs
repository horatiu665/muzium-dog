namespace DogAI
{
    using System.Collections;
    using System.Collections.Generic;
    using PlantmanAI4;
    using ToyBoxHHH.ReadOnlyUtil;
    using UnityEngine;

    public class StateBarkAtThing : MonoBehaviour, IState
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

        public DogBarkingBrain barkBrain => dogRefs.dogBrain.dogBarkingBrain;

        public float priority = 10;

        private DogBarkableObject _targetObj;

        public float updatePathfindingRate = 2f;
        private float _updatePathfindingTimer;

        public Vector2 moveSpeed01Range = new Vector2(0.5f, 1f);


        private float _totalTimeBarking = 0f;
        public float timeSpentBarkingPerObject = 3f;

        public float maxTimeSpentBarkingPerTurn = 10f;
        [ReadOnly, SerializeField]
        private float _barkStartTime;

        public float minTimeBetweenBarkState = 7f;
        private float _lastBarkStateTime;
        private bool _isBarking;

        private List<DogBarkableObject> objectsBarked = new List<DogBarkableObject>();

        public Vector2 barkIntervalRange = new Vector2(0.4f, 1f);
        private float actualBarkInterval;
        private float _lastBarkTime;

        private float _barkPose;
        private float _barkPoseSmooth;

        string IState.GetName()
        {
            return "StateBarkAtThing";
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
            objectsBarked.Clear();
            FindObjectToBark();
            _barkStartTime = 0;
        }

        private void FindObjectToBark()
        {
            _targetObj = barkBrain.GetRandom();

            int monteCarloAttempts = 5;
            while (objectsBarked.Contains(_targetObj) && monteCarloAttempts-- > 0)
            {
                _targetObj = barkBrain.GetRandom();
            }

            var moveSpeed01 = Mathf.Lerp(moveSpeed01Range.x, moveSpeed01Range.y, Random.value);
            dogRefs.dogBrain.dogLocomotion.targetSpeed01 = moveSpeed01;
        }

        void IState.OnExecute(float deltaTime)
        {
            // if we have a target object
            if (_targetObj != null)
            {
                // we should go near enough
                var dist = Vector3.Distance(dogRefs.transform.position, _targetObj.barkPosition);
                // if we're not close enough, walk there
                if (dist > barkBrain.barkableRadius)
                {
                    dogRefs.dogBrain.dogAstar.SetDestination(_targetObj.barkPosition);
                    barkBrain.dogBarkableObjectTarget = null;

                    dogRefs.dogBrain.dogLook.LookAt(null, this);

                    _barkPose = 0f;

                }
                else
                // if we're close enough to the target.
                {

                    // if we aren't barking yet
                    if (!_isBarking)
                    {
                        // start barking
                        _isBarking = true;
                        _barkStartTime = Time.time;

                        barkBrain.dogBarkableObjectTarget = _targetObj;

                        // look at barked obj
                        dogRefs.dogBrain.dogLook.LookAtPosition(_targetObj.barkPosition, this);

                        _barkPose = 1;

                        // dogRefs.dogBrain.dogVoice.Sniff(1f);
                        DoBark();

                    }
                    else if (_isBarking)
                    {
                        // continue barking
                        DoBark();

                        _barkPose = 1;

                        // if time is out, find new sniff target.
                        if (Time.time - _barkStartTime > timeSpentBarkingPerObject)
                        {
                            _totalTimeBarking += timeSpentBarkingPerObject;

                            objectsBarked.Add(_targetObj);
                            barkBrain.dogBarkableObjectTarget = null;
                            _isBarking = false;
                            FindObjectToBark();

                            _barkPose = 0;

                        }
                    }
                }

            }
            else
            {
                _barkPose = 0;

                // sniff in the air???
                // barkBrain.sniffAnimationTarget = null;
            }
        }

        void DoBark()
        {
            // consider putting this function in the BarkBrain with all the animation, sound etc that a bark involves.
            if (Time.time > _lastBarkTime + actualBarkInterval)
            {
                dogRefs.dogBrain.dogVoice.BarkAny();
                dogRefs.dogBrain.dogBarkingBrain.Bark();
                dogRefs.anim.SetTrigger("Bark");

                _lastBarkTime = Time.time;
                actualBarkInterval = Mathf.Lerp(barkIntervalRange.x, barkIntervalRange.y, Random.value);
            }
        }

        private void Update() {
            _barkPoseSmooth = Mathf.Lerp(_barkPoseSmooth, _barkPose, 0.2f);
            dogRefs.anim.SetFloat("BarkPose", _barkPoseSmooth);
        }

        void IState.OnExit()
        {
            _totalTimeBarking = 0;

            _barkStartTime = 0;
            _lastBarkStateTime = Time.time;
            barkBrain.dogBarkableObjectTarget = null;
            _isBarking = false;
            objectsBarked.Clear();

            dogRefs.dogBrain.dogLook.LookAt(null, this);

            // dogRefs.dogBrain.dogVoice.Sniff(0f);

            _barkPose = 0;

        }

        bool IState.ConditionsMet()
        {
            // if dog has sniffable object in sight
            if (!barkBrain.AnyBarkables())
            {
                return false;
            }

            if (_totalTimeBarking > maxTimeSpentBarkingPerTurn)
            {
                return false;
            }

            // if too soon to bark again
            if (Time.time - _lastBarkStateTime < minTimeBetweenBarkState)
            {
                return false;
            }

            return true;

        }

    }
}