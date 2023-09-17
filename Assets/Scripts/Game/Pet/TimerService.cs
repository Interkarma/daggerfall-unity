using System;
using DaggerfallWorkshop.Game;
using UnityEngine;

namespace Game.Pet
{
    public class TimerService : MonoBehaviour
    {
        private float _currentTime;
        private int _lastTime;
        private int _totalTime;
        private bool _timerIsOn;

        private Action _onTimerUp;

        public void StartTimer(int totalTime, Action onTimerUp)
        {
            _onTimerUp = onTimerUp;
            _totalTime = totalTime;
            _currentTime = 0;
            _lastTime = 0;
            _timerIsOn = true;
        }

        private void Update()
        {
            if (GameManager.IsGamePaused) return;

            if (!_timerIsOn) return;

            _currentTime += Time.deltaTime;
            _lastTime = (int) _currentTime;
            Debug.Log("timer is : " + _lastTime);

            if (_lastTime != _totalTime) return;

            _onTimerUp?.Invoke();
            StopTimer();
        }

        public void StopTimer()
        {
            _timerIsOn = false;
            _onTimerUp = null;
        }
    }
}