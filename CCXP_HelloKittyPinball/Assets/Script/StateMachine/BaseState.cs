using Script.Utils;

namespace Script.StateMachine
{
    public abstract class BaseState
    {
        public virtual void EnterState(StateSystem system)
        {
            system.LogError($"{this.GetType()} must be implemented");
        }

        public virtual void UpdateState()
        {
        }

        public virtual void ExitState()
        {
        }

        public virtual void OnPlayerDetected()
        {
        }

        public virtual void OnDialogueEnd()
        {
        }
    }
}