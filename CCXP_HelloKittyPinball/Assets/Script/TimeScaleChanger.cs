using UnityEngine;

namespace Script
{
    public class TimeScaleChanger : MonoBehaviour
    {
        public float timeToChange;
    
        private void Awake()
        {
            Time.timeScale = timeToChange;
        }
    }
}
