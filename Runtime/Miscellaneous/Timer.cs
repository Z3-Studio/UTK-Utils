using System;
using UnityEngine;

namespace Z3.Utils
{
    public class Timer : IDisposable
    {
        public event Action OnCompleted;

        public float TimeInSeconds { get; set; }

        public float Progression => Counter / TimeInSeconds;
        public bool IsCompleted => Counter >= TimeInSeconds;
        public float Counter { get; private set; }

        public void Reset()
        {
            Counter = 0;
        }

        public void FixedTick()
        {
            if (IsCompleted)
                return;

            Counter += Time.fixedDeltaTime;

            if (IsCompleted)
                OnCompleted?.Invoke();
        }

        public void Dispose()
        {
            OnCompleted = null;
        }
    }
}