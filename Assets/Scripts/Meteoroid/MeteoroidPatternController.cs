using UnityEngine;
using UnityTemplateProjects;

namespace Meteoroid
{
    public class MeteoroidPatternController : MonoBehaviour
    {
        public MeteoroidManager MeteoroidManager;

        private bool mRunning;
        private int mFramePassed;

        public void PatternStart()
        {
            mRunning = true;
            MeteoroidManager.FireCount = 1;
            MeteoroidManager.FireSpeed = 10;
            MeteoroidManager.FireDuration = 180;
            MeteoroidManager.FireMeteoroidSize = 3;
        }

        void Update()
        {
            mFramePassed++;
        }

        public void PatternPause()
        {
            mRunning = false;
        }

        public void PatternResume()
        {
            mRunning = true;
        }

        public void PatternStop()
        {
            mRunning = false;
            mFramePassed = 0;
        }
    }
}