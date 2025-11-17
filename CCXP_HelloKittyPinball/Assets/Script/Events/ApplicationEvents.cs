
using System;
using Script.StateMachine;
using UnityEngine;

namespace Script.Events
{
    public static class ApplicationEvents
    {
        public static Action<BaseState> OnApplicationStateChanged;
        public static Action OnStartApplication;
        public static Action OnTimeOutReset;
        public static Action OnFinishApplication;
        public static Action OnResetApplication;
        public static Action<KeyCode> OnKeyPressed;
    }
}