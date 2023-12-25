using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlantmanAI4
{

    public class ControllerState : MonoBehaviour, PlantmanAI4.IState
    {
        public List<IState> states = new List<IState>();
        public IState currentState;

        public new string name = "Controller State";
        public float priority;

        // if this is not part of another state machine, it should run based on unity's events
        public bool runIndependently = false;

        [Header("Debug")]
        public bool debugStateChanges = false;

        [Space]
        public bool drawStateSphere;
        public Gradient color = new Gradient() { colorKeys = new GradientColorKey[] { new GradientColorKey(Color.red, 0f), new GradientColorKey(Color.green, 1f) } };
        public Vector2 stateSphereSize;

        public event System.Action<IState> OnEnterState, OnExitState;

        bool InitStates()
        {
            if (states.Count == 0)
            {
                // get child components which have this as the direct parent (except ourself)
                states = GetComponentsInChildren<IState>(true).Where(s => (s as Component).GetComponentInParent<ControllerState>() == this && (s != (this as IState))).ToList();
            }
            // sets initial state, etc
            var validStates = states.Where(s => s.ConditionsMet());
            if (validStates.Count() > 0)
                currentState = validStates.Aggregate((s1, s2) =>
                    s1.GetPriority() < s2.GetPriority() ? s2 : s1);
            if (currentState == null)
                return false;

            currentState.OnEnter();
            OnEnterState?.Invoke(currentState);

            AddStarToName();
            if (debugStateChanges)
                Debug.Log("Enter state ->> " + currentState.GetName(), currentState as Component);

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
                // takes list of valid states, chooses highest priority. 
                var validStates = states.Where(s => s.ConditionsMet());
                var highestPriorityState = validStates.Aggregate((s1, s2)
                    => s1.GetPriority() < s2.GetPriority() ? s2 : s1);

                // if different than current state, switch to it.
                if (highestPriorityState != currentState)
                {
                    OnExitState?.Invoke(currentState);
                    currentState.OnExit();
                    RemoveStarFromName();
                    if (debugStateChanges)
                        Debug.Log("<<- End state " + currentState.GetName(), currentState as Component);

                    currentState = highestPriorityState;
                    currentState.OnEnter();
                    OnEnterState?.Invoke(currentState);

                    AddStarToName();
                    if (debugStateChanges)
                        Debug.Log("Enter state ->> " + currentState.GetName(), currentState as Component);
                }
            }

            currentState.OnExecute(deltaTime);

        }

        private void RemoveStarFromName()
        {
            var n = (currentState as Component).name;
            if (n[0] == '*')
                n = n.Substring(1);
            (currentState as Component).name = n;
        }

        private void AddStarToName()
        {
            var n = (currentState as Component).name;
            if (n[0] != '*')
                n = "*" + n;
            (currentState as Component).name = n;

        }

        public string GetName()
        {
            return name;
        }

        public bool GetUninterruptible()
        {
            return currentState.GetUninterruptible();
        }

        public float GetPriority()
        {
            return priority;
        }

        public void OnEnter()
        {
            InitStates();
        }

        public void OnExecute(float deltaTime)
        {
            UpdateStates(deltaTime);
        }

        public void OnExit()
        {
            OnExitState?.Invoke(currentState);
            currentState.OnExit();
            RemoveStarFromName();
            if (debugStateChanges)
                Debug.Log("<<- End state " + currentState.GetName(), currentState as Component);

            currentState = null;
        }

        public bool ConditionsMet()
        {
            return states.Any(s => s.ConditionsMet());
        }



        // unity messages


        void Start()
        {
            if (runIndependently)
            {
                InitStates();
            }
        }

        void Update()
        {
            if (runIndependently)
            {
                UpdateStates(Time.deltaTime);
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

                DrawString(currentState.GetName(), transform.position + Vector3.up * stateSphereSize.x, Color.white);

            }
        }

        public static void DrawString(string text, Vector3 worldPos, Color? textColor = null, Color? backColor = null)
        {
#if UNITY_EDITOR
            UnityEditor.Handles.BeginGUI();
            var restoreTextColor = GUI.color;
            var restoreBackColor = GUI.backgroundColor;

            GUI.color = textColor ?? Color.white;
            GUI.backgroundColor = backColor ?? Color.black;

            var view = UnityEditor.SceneView.currentDrawingSceneView;
            if (view != null && view.camera != null)
            {
                Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
                if (screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.z < 0)
                {
                    GUI.color = restoreTextColor;
                    UnityEditor.Handles.EndGUI();
                    return;
                }
                Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
                var r = new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y);
                GUI.Box(r, text, UnityEditor.EditorStyles.numberField);
                GUI.Label(r, text);
                GUI.color = restoreTextColor;
                GUI.backgroundColor = restoreBackColor;
            }
            UnityEditor.Handles.EndGUI();
#endif
        }
    }

}