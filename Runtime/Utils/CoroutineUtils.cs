using System;
using System.Collections;
using UnityEngine;

namespace Z3.Utils
{
    public static class CoroutineUtils
    {
        public static void DoAfterSeconds(this MonoBehaviour behaviour, Action action, float seconds)
        {
            behaviour.StartCoroutine(DoSomethingAfterSeconds(action, seconds));
        }

        private static IEnumerator DoSomethingAfterSeconds(Action action, float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            action();
        }
    }
}