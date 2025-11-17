using System;
using System.Runtime.InteropServices;
using System.Text;
using Script.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityRawInput;

namespace RealGames.Script
{
    public class UnityRawInputHandler : MonoBehaviour
    {
        public UnityEvent<RawKey> OnRawKeyInputEvent;
#if PLATFORM_STANDALONE_WIN

        private void Awake()
        {
            RawInput.WorkInBackground = true;
            RawInput.InterceptMessages = false;
            RawInput.OnKeyDown += RawInputOnOnKeyDownHandler;
            RawInput.Start();
        }

        private void RawInputOnOnKeyDownHandler(RawKey obj)
        {            
            Debug.Log(obj);
            if (obj is >= RawKey.A and <= RawKey.Z)
            {
                Debug.Log(obj);
            }
        }


        private void OnDestroy()
        {
            RawInput.OnKeyDown -= RawInputOnOnKeyDownHandler;
            RawInput.Stop();
            
        }
#endif
    }
}
