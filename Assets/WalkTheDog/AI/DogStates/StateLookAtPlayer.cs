namespace DogAI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using PlantmanAI4;
    using UnityEngine;

    public class StateLookAtPlayer : MonoBehaviour, IState
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

        public DogAstar dogAstar => dogRefs.dogBrain.dogAstar;

        public float priorityWhenPlayerIsMoving = 1.5f;
        public float priorityWhenPlayerIsStill = 5;

        public float minTimeInFrontOfPlayerToEnd = 3f;
        private float timeInFrontOfPlayer;

        // time with a clear raycast between dog and player.
        public float minTimeLookingAtPlayer = 3f;
        private float timeLookingAtPlayer;

        public float minTimeBetweenLooks = 3;

        public float followPathDelay = 0.5f;
        private float prevPathTime;

        public float targetSpeed01 = 1;
        public float targetSpeedWhenInFrontOfPlayer = 0.5f;
        private float curTargetSpeed01;

        // save refs to nodes that we checked and are not good currently for watching the player.
        private List<AStar.Node> notGoodNodes = new List<AStar.Node>();

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

        public float distBeforeWeRaycastFromNode = 7;

        private bool _isLookingAtPlayer;


        string IState.GetName()
        {
            return "StateLookAtPlayer";
        }

        bool IState.GetUninterruptible()
        {
            return false;
        }

        float IState.GetPriority()
        {
            if (dogBrain.playerFakeVelocity.speed > 0)
                return priorityWhenPlayerIsMoving;
            else
                return priorityWhenPlayerIsStill;
        }

        void IState.OnEnter()
        {
            _isActive = true;
            lastTimeThisStateWasActive = Time.time;

            notGoodNodes.Clear();
        }

        void IState.OnExecute(float deltaTime)
        {
            lastTimeThisStateWasActive = Time.time;

            // check looking at player state
            _isLookingAtPlayer = IsLookingAtPlayer();
            var dogHead = dogRefs.head;
            var dogHeadPos = dogHead.position;


            if (!_isLookingAtPlayer)
            {
                timeLookingAtPlayer = 0;
                dogBrain.dogLook.LookAt(null, this);

                dogRefs.dogBrain.dogVoice.Pant(0);

            }
            else
            {
                timeLookingAtPlayer += deltaTime;
                dogBrain.dogLook.LookAt(playerCamera.transform, this);

                dogRefs.dogBrain.dogVoice.Pant(0.7f);

            }

            if (Time.time - prevPathTime > followPathDelay)
            {
                prevPathTime = Time.time;

                // if we are not looking at player, make a path to a point where the player would be visible...
                if (!_isLookingAtPlayer)
                {
                    // find a node near the line of sight of the player
                    var nodeNearLine = dogAstar.aStar.GetNearestNodeToLine(playerCamera.position, playerCamera.forward, notGoodNodes);

                    Debug.DrawLine(dogHeadPos, nodeNearLine.position, Color.cyan, 3f);

                    // if node is far away, try to go closer.
                    var distToNode = Vector3.Distance(dogHeadPos, nodeNearLine.position);
                    if (distToNode > distBeforeWeRaycastFromNode)
                    {
                        dogRefs.dogBrain.dogAstar.SetDestination(nodeNearLine.position);
                        var dirToPlayerCamera = playerCamera.position - dogHeadPos;
                        dogRefs.dogLocomotion.SetTargetRotation(dirToPlayerCamera);
                        dogRefs.dogBrain.dogAstar.dogLocomotion.targetSpeed01 = curTargetSpeed01;
                    }
                    else
                    {
                        // we are close enough, raycast from here to see if we see the player. if we don't, save this node and search for another one where we can see the player. 
                        var dirToPlayer = player.position - dogHeadPos;
                        var anyHits = Physics.Raycast(dogHeadPos, dirToPlayer, out RaycastHit hit, dirToPlayer.magnitude, dogBrain.dogAstar.aStar.layerMask);
                        if (anyHits)
                        {
                            notGoodNodes.Add(nodeNearLine);

                        }
                    }
                }
                else
                {
                    var dirToPlayer = player.position - dogHeadPos;
                    dogRefs.dogBrain.dogAstar.StopMovement();
                    dogRefs.dogLocomotion.SetTargetRotation(dirToPlayer);
                }
            }

            if (!IsInFrontOfPlayer())
            {
                timeInFrontOfPlayer = 0;
                curTargetSpeed01 = Mathf.Lerp(curTargetSpeed01, targetSpeed01, 0.1f);

            }
            else
            {
                timeInFrontOfPlayer += deltaTime;
                curTargetSpeed01 = Mathf.Lerp(curTargetSpeed01, targetSpeedWhenInFrontOfPlayer, 0.1f);
            }

        }

        public bool IsLookingAtPlayer()
        {
            var playerPosition = playerCamera.position;
            // is a raycast between dog and player clear?
            var dogHeadPos = dogRefs.head.position;
            var dir = playerPosition - dogHeadPos;
            Debug.DrawRay(dogHeadPos, dir, Color.white, 0.5f);
            var anyHits = Physics.Raycast(dogHeadPos, dir, out RaycastHit hit, dir.magnitude, dogBrain.dogAstar.aStar.layerMask);
            if (anyHits)
            {
                // Debug.Log("Looking at player raycast hit: " + hit.collider.name);
                if (dogBrain.IsThisThePlayer(hit.collider))
                {
                    return true;
                }

                //Debug.DrawLine(dogRefs.head.position, hit.point, Color.red, 0.5f);
                return false;
            }
            else // no hits at all
            {
                //Debug.DrawLine(dogRefs.head.position, playerPosition, Color.green, 0.5f);
                return true;
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
            dogBrain.dogLook.LookAt(null, this);

            dogRefs.dogBrain.dogVoice.Pant(0);

        }

        bool IState.ConditionsMet()
        {
            if (player == null)
                return false;

            // var dir = transform.position - player.position;
            // dir.y *= 0.2f; // care less for y axis distance
            // var dist = dir.magnitude;

            // if state not active 
            if (!_isActive)
            {
                // wait between follows
                if (Time.time - lastTimeThisStateWasActive < minTimeBetweenLooks)
                    return false;

                return true;

            }
            else //if (_isActive)
            {
                // min time in front of player -> keep looking
                if (timeInFrontOfPlayer < minTimeInFrontOfPlayerToEnd)
                    return true;

                // min time looking at player -> keep looking
                if (timeLookingAtPlayer < minTimeLookingAtPlayer)
                    return true;

                return false;
            }

        }

    }
}