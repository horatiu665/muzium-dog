namespace DogAI
{
    using System.Collections;
    using System.Collections.Generic;
    using PlantmanAI4;
    using UnityEngine;

    public class StateGetPoem : MonoBehaviour, IState
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

        public string stateName = "StateGetPoem";

        public Transform player => dogRefs.dogBrain.player;

        public float priority = 5;
        public float priorityRandomAdd = 0.1f;
        private float nextPriority;

        // sets target speed so the dog wanders at walking pace or running.
        public float targetSpeedGoAway01 = 1;
        public float targetSpeedReturn01 = 0.5f;

        public float pathCalculationInterval = 5f;

        public Transform poemPosition;

        private bool hasPoem = false;
        public PoemInteractableInDogsMouth poemInteractable;

        private float timePoemDelivered;
        public float minTimeBetweenPoems = 180;

        private void OnEnable()
        {
            poemInteractable.OnInteractedEvent += HumanTakePoemFromDog;
            poemInteractable.gameObject.SetActive(false);
            // to make sure colliders don't go haywire ?!?!
            Physics.SyncTransforms();

        }

        private void OnDisable()
        {
            poemInteractable.OnInteractedEvent -= HumanTakePoemFromDog;

        }

        string IState.GetName()
        {
            return stateName;
        }

        bool IState.GetUninterruptible()
        {
            // can't leave this state if has poem. until you pick up the poem. otherwise it will run back to the castle lol
            return hasPoem;
        }

        float IState.GetPriority()
        {
            nextPriority = Random.Range(0, priorityRandomAdd);

            return priority + nextPriority;
        }

        void IState.OnEnter()
        {
            hasPoem = false;

        }

        public float distToPlayerToStop = 4f;

        void IState.OnExecute(float deltaTime)
        {
            if (!hasPoem)
            {
                var groundedPos = dogRefs.dogBrain.dogAstar.aStar.GetGroundedPosition(poemPosition.position, out var grounded);
                if (grounded)
                {
                    dogRefs.dogBrain.dogAstar.SetDestination(groundedPos);
                    dogRefs.dogBrain.dogAstar.dogLocomotion.targetSpeed01 = targetSpeedGoAway01;

                    if (Vector3.Distance(transform.position, groundedPos) < 1)
                    {
                        DogPickUpPoem();
                    }
                }

            }
            else
            {
                // it HAS to be kinematic for the poem transport to work.
                dogRefs.dogBrain.dogLocomotion.SetKinematic(true);

                // if close to player, stop
                if (Vector3.Distance(transform.position, player.position) < distToPlayerToStop)
                {
                    dogRefs.dogBrain.dogAstar.StopMovement();
                    dogRefs.dogBrain.dogLocomotion.SetTargetRotation(player.position - transform.position);

                    // consider whimpering or something

                }
                else
                {

                    bool shouldRecalcPath = !dogRefs.dogBrain.dogAstar.hasPath;
                    if (dogRefs.dogBrain.dogAstar.hasPath)
                    {
                        if (Time.time - dogRefs.dogBrain.dogAstar.lastPathCalculationTime > pathCalculationInterval)
                        {
                            shouldRecalcPath = true;
                        }
                    }
                    if (shouldRecalcPath)
                    {
                        // go to player
                        var playerFront = player.position;
                        var playerY = player.position.y;
                        playerFront += player.forward * 1.5f + Random.onUnitSphere * 0.3f;
                        var pfv = dogRefs.dogBrain.playerFakeVelocity;
                        var playerFakeVelocity = pfv.velocity;
                        playerFront += Vector3.ClampMagnitude(playerFakeVelocity, 3f);

                        playerFront.y = playerY;

                        var nearestNode = dogRefs.dogBrain.dogAstar.aStar.GetNearestNode(playerFront);
                        playerFront.y = nearestNode.position.y;

                        Debug.DrawLine(transform.position, playerFront, Color.yellow, 0.5f);

                        dogRefs.dogBrain.dogAstar.SetDestination(playerFront);
                        dogRefs.dogBrain.dogAstar.dogLocomotion.targetSpeed01 = targetSpeedReturn01;
                    }
                }
            }

        }

        void DogPickUpPoem()
        {
            hasPoem = true;

            poemInteractable.gameObject.SetActive(true);
            Physics.SyncTransforms();

        }

        public void HumanTakePoemFromDog()
        {
            hasPoem = false;
            poemInteractable.gameObject.SetActive(false);
            Physics.SyncTransforms();

            PoemSystem.instance.SetPoemVisible(true);

            timePoemDelivered = Time.time;
        }


        void IState.OnExit()
        {
            dogRefs.dogBrain.dogLocomotion.SetKinematic(false);

        }

        bool IState.ConditionsMet()
        {
            // if not enough time has passed since last poem, don't bring another one.
            if (Time.time - timePoemDelivered < minTimeBetweenPoems)
            {
                return false;
            }

            return true;
        }

    }
}