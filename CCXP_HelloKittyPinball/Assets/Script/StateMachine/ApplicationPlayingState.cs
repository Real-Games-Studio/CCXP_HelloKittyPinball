using UnityEngine;

namespace Script.StateMachine
{
    public class ApplicationPlayingState : BaseState
    {
        protected ApplicationStateSystem applicationStateSystem;

        public override void EnterState(StateSystem system)
        {
            applicationStateSystem = (ApplicationStateSystem) system;
            Debug.Log("Entered generic ApplicationPlayingState");
        }
    }
}