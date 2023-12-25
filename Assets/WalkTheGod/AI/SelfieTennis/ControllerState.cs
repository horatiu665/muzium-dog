namespace SelfieTennis
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using PlantmanAI4;

    public class ControllerState : MonoBehaviour, PlantmanAI4.IState
    {
        [SerializeField]
        private List<IState> _states;
        public List<IState> states
        {
            get
            {
                if (_states == null || _states.Count == 0)
                {
                    SetupStates();
                }
                // only update outside of playmode if any state is null
                if (!Application.isPlaying && _states.Any(s => s == null))
                {
                    SetupStates();
                }
                return _states;
            }
        }

        public IState currentState;

        public bool initStates;

        public new string name = "Controller State";
        public float priority;

        // if this is not part of another state machine, it should run based on unity's events
        public bool runIndependently = false;

        public void EditorInitStates()
        {
            if (_states == null || _states.Count == 0)
            {
                SetupStates();
            }
        }

        public float updateTime = 0f;
        float updateTimer = 0f;

        public Gradient color;
        public bool drawStateSphere;
        public Vector2 stateSphereSize;

        bool InitStates()
        {
            // sets initial state, etc
            var validStates = states.Where(s => s.ConditionsMet() && (s as MonoBehaviour).isActiveAndEnabled);
            if (!validStates.Any())
            {
                return false;
            }
            IState highestPriorityState = validStates.First();
            float maxPriority = float.MinValue;
            // for (int i = 0; i < validStates.Count(); i++)
            foreach (var s in validStates)
            {
                var p = s.GetPriority();
                if (p > maxPriority)
                {
                    maxPriority = p;
                    highestPriorityState = s;
                }
            }
            currentState = highestPriorityState;
            currentState.OnEnter();
            return true;
        }

        void UpdateStates(float deltaTime)
        {
            if (currentState == null)
            {
                if (!InitStates())
                {
                    return;
                }
            }

            if (!currentState.GetUninterruptible())
            {
                // takes list of valid states, chooses highest distTargetPriority. 
                var validStates = states.Where(s => s.ConditionsMet() && (s as MonoBehaviour).isActiveAndEnabled);
                if (!validStates.Any())
                {
                    return;
                }
                IState highestPriorityState = validStates.First();
                float maxPriority = float.MinValue;
                foreach (var s in validStates)
                // for (int i = 0; i < validStates.Count(); i++)
                {
                    var p = s.GetPriority();
                    if (p > maxPriority)
                    {
                        maxPriority = p;
                        highestPriorityState = s;
                    }
                }

                // if different than current state, switch to it.
                if (highestPriorityState != currentState)
                {
                    currentState.OnExit();
                    RemoveStarFromName();
                    currentState = highestPriorityState;
                    currentState.OnEnter();
                    AddStarToName();
                }
            }

            currentState.OnExecute(deltaTime);

        }

        public string GetName()
        {
            if (currentState != null)
            {
                return name + " > " + currentState.GetName();
            }
            else
            {
                return name;
            }
        }

        public bool GetUninterruptible()
        {
            if (currentState == null)
            {
                return false;
            }
            return currentState.GetUninterruptible();
        }

        public float GetPriority()
        {
            // find the highest distTargetPriority among states. not just current state (because nothing will activate ever if only current state is considered
            var maxPriority = states.Max(s => s.ConditionsMet() ? s.GetPriority() : 0);
            return priority + maxPriority;
        }

        private void RemoveStarFromName()
        {
#if UNITY_EDITOR
            var n = (currentState as Component).name;
            if (n[0] == '*')
                n = n.Substring(1);
            (currentState as Component).name = n;
#endif
        }

        private void AddStarToName()
        {
#if UNITY_EDITOR
            var n = (currentState as Component).name;
            if (n[0] != '*')
                n = "*" + n;
            (currentState as Component).name = n;
#endif
        }

        public void OnEnter()
        {
            InitStates();
        }

        public void OnExecute(float deltaTime)
        {
            if (runIndependently)
            {
                UpdateStates(deltaTime);
            }
            else
            {
                updateTimer += deltaTime;
                if (updateTimer >= updateTime)
                {
                    updateTimer = 0;
                    UpdateStates(Mathf.Max(updateTime, Time.deltaTime));
                }
            }
        }

        public void OnExit()
        {
            if (currentState != null)
            {
                currentState.OnExit();
            }
            currentState = null;
        }

        public bool ConditionsMet()
        {
            if (states == null)
                return false;
            return states.Any(s => s.ConditionsMet());
        }

        void SetupStates()
        {
            _states = GetComponentsInChildren<IState>(true)
                .Where(s => s != (IState)this)
                .Where(s =>
                {
                    if (!(s is ControllerState))
                    {
                        // only return true when the GetCompInParent returns this state, not another which might be a child.
                        // this means the states that have a different parent will not be returned.
                        return ((MonoBehaviour)s).GetComponentInParent<ControllerState>() == this;
                    }
                    return true;
                })
                .ToList();

        }

        // unity messages

        void Start()
        {
            SetupStates();

            if (initStates)
            {
                InitStates();
            }

            // always run RunIndependent coroutine, which does nothing when runIndependently is false.
            StartCoroutine(UpdateInfreq());

        }

        IEnumerator UpdateInfreq()
        {
            var r = Random.Range(0f, updateTime);
            yield return new WaitForSeconds(r);
            while (true)
            {
                if (runIndependently)
                {
                    UpdateStates(Mathf.Max(updateTime, Time.deltaTime));
                }
                yield return new WaitForSeconds(updateTime);
            }
        }

        void OnDrawGizmos()
        {
            if (color == null)
                return;
            if (states == null)
                return;
            if (currentState == null)
                return;

            if (drawStateSphere)
            {
                Gizmos.color = color.Evaluate(states.IndexOf(currentState) / (float)(states.Count - 1));
                Gizmos.DrawSphere(transform.position + Vector3.up * stateSphereSize.x, stateSphereSize.y);
            }
        }

    }
}