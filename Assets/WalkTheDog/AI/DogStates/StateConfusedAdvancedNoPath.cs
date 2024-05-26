namespace DogAI
{
    using System.Collections;
    using System.Collections.Generic;
    using PlantmanAI4;
    using UnityEngine;

    public class StateConfusedAdvancedNoPath : MonoBehaviour, IState
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

        private float lookAroundConfusedTime;

        public Vector2 whimperDelayRange = new Vector2(5, 10);
        private float whimperTime;


        [Header("Confusion steps")]
        public List<ConfusionStep> steps = new();

        private int currentConfusionStepIndex = 0;
        private ConfusionStep currentConfusionStep; // updated OnExit to prevent weird glitches with ConditionsMet().
        private float lastConfusionTime = 0f;


        [System.Serializable]
        public class ConfusionStep
        {
            public float timeSpentConfused;
            public float whimperChance = 0.5f;
            public bool shouldStopMoving = false;

            public bool goToNearestNode = false;

            // if confused twice within this time, increase the confusion step
            public float confusionStepIncreaseCooldown = 3f;

            // if not confused within this time, decrease the confusion step
            public float confusionStepDecreaseCooldown = 20f;


        }

        string IState.GetName()
        {
            return "StateConfusedAdvancedNoPath";
        }

        bool IState.GetUninterruptible()
        {
            return false;
        }

        float IState.GetPriority()
        {
            return priority;
        }

        private void Awake()
        {
            if (currentConfusionStep == null)
            {
                currentConfusionStep = steps[currentConfusionStepIndex];
            }
        }

        void IState.OnEnter()
        {
            if (currentConfusionStep == null)
            {
                currentConfusionStep = steps[currentConfusionStepIndex];
            }

            // don't stop moving.
            // dogRefs.dogBrain.dogAstar.StopMovement();
            // dogRefs.dogLocomotion.StopMovement();
            // dogRefs.dogLocomotion.StopRotation();

            // if confused again, increase confusion step
            // do it here in OnExit so the new confusion parameters are valid only from the next check.
            var timeSinceLastConfusion = Time.time - lastConfusionTime;
            if (timeSinceLastConfusion < currentConfusionStep.confusionStepIncreaseCooldown)
            {
                currentConfusionStepIndex++;
                if (currentConfusionStepIndex >= steps.Count)
                {
                    currentConfusionStepIndex = steps.Count - 1;
                }
            }
            lastConfusionTime = Time.time;

            // look confused?
            LookAtRandomPlace();
            var whimperChance = currentConfusionStep.whimperChance;
            if (Random.value < whimperChance)
            {
                dogRefs.dogBrain.dogVoice.Whimper();
            }
            whimperTime = Time.time + Random.Range(whimperDelayRange.x, whimperDelayRange.y);
        }

        private void LookAtRandomPlace()
        {
            dogRefs.dogBrain.dogLook.LookAtDirection(Random.onUnitSphere, this);
            lookAroundConfusedTime = Time.time + Mathf.Lerp(0.5f, 1.5f, Random.value);
        }

        private void Update()
        {
            // rare update
            if (Time.frameCount % 60 == 0)
            {
                // decrease confusion step over time.
                if (Time.time - lastConfusionTime > currentConfusionStep.confusionStepDecreaseCooldown)
                {
                    currentConfusionStepIndex--;
                    if (currentConfusionStepIndex < 0)
                    {
                        currentConfusionStepIndex = 0;
                    }
                    currentConfusionStep = steps[currentConfusionStepIndex];
                }
            }

        }

        void IState.OnExecute(float deltaTime)
        {
            if (Time.time > lookAroundConfusedTime)
            {
                LookAtRandomPlace();
            }

            if (currentConfusionStep.shouldStopMoving)
            {
                dogRefs.dogBrain.dogAstar.StopMovement();
                dogRefs.dogLocomotion.StopMovement();
            }

            if (currentConfusionStep.goToNearestNode)
            {
                // go to nearest node
                var dogAstar = dogRefs.dogBrain.dogAstar;
                if (!dogAstar.hasDestination)
                {
                    var nearestNode = dogAstar.aStar.GetNearestNodeWithClearView(dogRefs.transform.position);
                    dogAstar.SetDestination(nearestNode.position);
                }
            }
        }

        void IState.OnExit()
        {
            dogRefs.dogBrain.dogLook.LookAt(null, this);

            currentConfusionStep = steps[currentConfusionStepIndex];

        }

        bool IState.ConditionsMet()
        {
            if (currentConfusionStep == null)
            {
                currentConfusionStep = steps[0];
            }

            if (Time.time - dogRefs.dogBrain.dogAstar.cantFindPathTime < currentConfusionStep.timeSpentConfused)
                return true;

            return false;
        }

    }
}