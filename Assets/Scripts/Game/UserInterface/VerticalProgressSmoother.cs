using UnityEngine;
using DaggerfallWorkshop.Game.Serialization;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// A vertical progress/indicator bar. Modified to smoothly change
    /// </summary>
    public class VerticalProgressSmoother : VerticalProgress
    {
        private float prevPercent;
        private float targetPercent;
        private float timer;
        private const float timerMax = 0.4f;
        public bool cycleTimer;

        /// <summary>
        /// Start a smooth change with delay and target
        /// </summary>
        /// <param name="target">Target percent to change bar to</param>
        public void BeginSmoothChange(float target)
        {
            if (cycleTimer == false)
            {
                cycleTimer = true;
                timer = -0.5f;
            }
            else
                timer = -0.25f;

            prevPercent = Amount;
            targetPercent = target;
        }

        public void Cycle()
        {
            if (!cycleTimer)
                return;       
            
            timer += Time.deltaTime;

            if (timer >= 0)
            {
                float t = Mathf.Clamp01(timer / timerMax);
                Amount = Mathf.Lerp(prevPercent, targetPercent, t);

                if (timer >= timerMax)
                    cycleTimer = false;
            }
        }
    }
}
