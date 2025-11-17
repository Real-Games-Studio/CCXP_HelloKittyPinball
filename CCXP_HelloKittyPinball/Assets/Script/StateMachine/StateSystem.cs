using Script.Events;
using UnityEngine;

namespace Script.StateMachine
{
    public class StateSystem : MonoBehaviour
    {
        protected BaseState currentState;
        protected BaseState lastState;

        public string debugCurrentState;

        public virtual void Start() { }

        public virtual void SwitchState(BaseState state)
        {
            var name = state.GetType().ToString().Split("."); 
            debugCurrentState = name[^1];
            if (currentState != null)
            {
                currentState.ExitState();
            }
            lastState = currentState;
            currentState = state;

            ApplicationEvents.OnApplicationStateChanged?.Invoke(currentState);
            currentState.EnterState(this);
        }

        public virtual void Update()
        {
            if (currentState != null)
            {
                currentState.UpdateState();
            }
        }

        public bool IsCurrentState(BaseState state)
        {
            return currentState == state;
        }

        public BaseState GetLastState()
        {
            return lastState;
        }
    }
}