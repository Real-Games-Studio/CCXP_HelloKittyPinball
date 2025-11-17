using Script.Events;
using UnityEngine;

namespace Script.StateMachine
{
    public class ApplicationWaitingState : BaseState
    {
        protected ApplicationStateSystem applicationStateSystem;
        private bool hasStartMenu;
        private bool hasGameIntro;
        public override void EnterState(StateSystem system)
        {
            applicationStateSystem = (ApplicationStateSystem)system;
            ApplicationEvents.OnStartApplication += OnStartApplicationHandler;
            hasGameIntro = applicationStateSystem.hasGameIntro;
            hasStartMenu = applicationStateSystem.hasStartMenu;
        }

        public virtual void OnStartApplicationHandler()
        {
            if (hasStartMenu)
            {
                applicationStateSystem.SwitchState(applicationStateSystem.appStartMenuState);
                return;
            }
            if (hasGameIntro)
            {
                applicationStateSystem.SwitchState(applicationStateSystem.appIntroState);
                return;
            }
            
            Debug.Log("Start");
            applicationStateSystem.StartGame();
        }

        public override void ExitState()
        {
            ApplicationEvents.OnStartApplication -= OnStartApplicationHandler;
        }
    }
}