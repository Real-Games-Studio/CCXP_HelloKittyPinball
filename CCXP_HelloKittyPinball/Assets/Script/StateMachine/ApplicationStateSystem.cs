using System.Collections;
using Script.Events;
using UnityEngine;

namespace Script.StateMachine
{
    public class ApplicationStateSystem : StateSystem
    {
        
        public bool hasStartMenu;
        public bool hasGameIntro;
        public BaseState playingState;
        public BaseState waitingState;
        public BaseState appStartMenuState;
        public BaseState appIntroState;

        public IEnumerator CurrenteCourotine;
        public override void Start()
        {
            hasGameIntro = appIntroState != null;
            hasStartMenu = appStartMenuState != null;
            SwitchState(waitingState);
        }

        public virtual void StartGame()
        {
            SwitchState(playingState);
        }

        public IEnumerator WaitToIdle(BaseState state, float timeToWait)
        {
            Debug.Log($"started waiting for {state} during {timeToWait}");
            yield return  new WaitForSecondsRealtime(timeToWait);
            if (currentState == state)
            {
                SwitchState(waitingState);
            }
        }

        public void ResetApplication()
        {
            SwitchState(waitingState);
        }
        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }
    }
}