using UnityEngine;

namespace Z3.Utils
{
    /// <summary>
    /// Temporary
    /// </summary>
    public abstract class Monostate<T> : MonoBehaviour where T : MonoBehaviour 
    {
        protected static T Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"There already have a instances of {typeof(T).Name}, Current: {Instance.gameObject} and new {gameObject}");
                return;
            }

            Instance = this as T;
            AfterAwake();
        }

        protected virtual void AfterAwake() { }
    }
}