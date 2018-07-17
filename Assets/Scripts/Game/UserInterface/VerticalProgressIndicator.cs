using UnityEngine;
using DaggerfallWorkshop.Game.Serialization;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// A vertical progress/indicator bar. Modified to smoothly change
    /// </summary>
    public class VerticalProgressIndicator : VerticalProgress
    {
        private float prevPercent;
        private float targetPercent;
        private float timer;
        private const float timerMax = 0.4f;
        private bool cycleTimer;

        public void BeginSmoothChange(float target)
        {
            cycleTimer = true;
            timer = -0.5f;
            prevPercent = Amount;
            targetPercent = target;
            if (targetPercent > prevPercent)
                timer = timerMax;
        }

        public void Cycle()
        {
            if (!cycleTimer)
                return;       
            
            timer += Time.deltaTime;

            if (timer > 0)
            {
                float t = Mathf.Clamp01(timer / timerMax);
                Amount = Mathf.Lerp(prevPercent, targetPercent, t);

                if (timer >= timerMax)
                    cycleTimer = false;
            }
        }
    }
}
