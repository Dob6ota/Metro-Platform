using System;
using UnityEngine;

namespace MetroPlatform
{
    public class TimerCountdown
    {
        public bool Debugging = false;
        public string Name = "DefoultName";

        [SerializeField] private float _timeTotal;
        [SerializeField] private float _timeCounter;

        [field: SerializeField] public bool IsRunning { get; private set; }

        public bool IsFineshed => _timeCounter <= 0;
        public float Progress => 1 - (_timeCounter / _timeTotal);
        public float ProgressReversed => _timeCounter / _timeTotal;

        public event Action TimerStart = delegate { };
        public event Action TimerStop = delegate { };

        public TimerCountdown(float timeTotal, string name = "")
        {
            _timeTotal = timeTotal;
            Name = name;
        }

        public void Start(float newTimeTotal) 
        {
            _timeTotal = newTimeTotal;
            Start();
        }

        public void Start()
        {
            if(Debugging)
            {
                Debug.Log($"Time {Name} Srart. TimeTotal: {_timeTotal}");
            }

            _timeCounter = _timeTotal;
            if(!IsRunning)
            {
                IsRunning = true;
                TimerStart.Invoke();
            }
        }
        public void Stop()
        {
            if (Debugging)
            {
                Debug.Log($"Time {Name} Stop. TimeTotal: {_timeTotal}");
            }

            if (IsRunning)
            {
                IsRunning = false;
                TimerStop.Invoke();
            }
        }

        public void Tick(float deltaTime)
        {
            if (IsRunning && _timeCounter > 0)
            {
                _timeCounter -= deltaTime;
            }
            if (IsRunning && _timeCounter < 0)
            {
                Stop();
            }
        }

        public void Reset()
        {
            _timeCounter = _timeTotal;
        }
        public void Pause()
        {
            IsRunning = false;
        }
        public void Resume()
        {
            IsRunning = true;
        }
        public void SwitchTimerOnOroff()
        {
            IsRunning = !IsRunning; ;
        }

    }
}
