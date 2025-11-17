using System.Collections.Generic;
using System.Linq;
using Script.Events;
using UnityEngine;

namespace Script.Utils
{
    public static class UltiInput
    {
        static readonly KeyCode[] _keyCodes =
            System.Enum.GetValues(typeof(KeyCode))
                .Cast<KeyCode>()
                .Where(k => ((int) k < (int) KeyCode.Mouse0))
                .ToArray();

        public static IEnumerable<KeyCode> GetCurrentKeysDown()
        {
            if (Input.anyKeyDown)
                for (int i = 0; i < _keyCodes.Length; i++)
                    if (Input.GetKeyDown(_keyCodes[i]))
                        yield return _keyCodes[i];
        }

        public static IEnumerable<KeyCode> GetCurrentKeys()
        {
            if (Input.anyKey)
                for (int i = 0; i < _keyCodes.Length; i++)
                    if (Input.GetKey(_keyCodes[i]))
                        yield return _keyCodes[i];
        }

        public static IEnumerable<KeyCode> GetCurrentKeysUp()
        {
            for (int i = 0; i < _keyCodes.Length; i++)
                if (Input.GetKeyUp(_keyCodes[i]))
                    yield return _keyCodes[i];
        }
        
    }
    public class KeyboardHandler : MonoBehaviour
    {
        private void Update()
        {
            if (Input.anyKeyDown == true)
            {
                var keysDown = UltiInput.GetCurrentKeysDown();

                foreach (var key in keysDown)
                {
                    ApplicationEvents.OnKeyPressed?.Invoke(key);
                }
            }
        }
    }
}