using System;
using System.Collections.Generic;

namespace Z3.Utils
{
    /// <summary>
    /// Useful for handling situations where multiple systems may enable or disable a shared state.
    /// Triggers <see cref="OnChangeState"/> when the list becomes empty or non-empty.
    /// </summary>
    /// <remarks>
    /// Example: Different systems blocking player input
    /// </remarks>
    public class ModifiersList<T>
    {
        public event Action<bool> OnChangeState;
        public bool HasActiveModifier => modifierList.Count > 0;

        private readonly List<T> modifierList = new();

        public void Clear()
        {
            modifierList.Clear();
            OnChangeState?.Invoke(false);
        }

        public void HandleModifier(T modifier, bool include)
        {
            if (include)
            {
                AddModifier(modifier);
            }
            else
            {
                RemoveModifier(modifier);
            }
        }

        public void AddModifier(T modifier)
        {
            if (modifierList.Contains(modifier))
                return;

            modifierList.Add(modifier);

            if (modifierList.Count == 1)
            {
                OnChangeState?.Invoke(true);
            }
        }

        public void RemoveModifier(T modifier)
        {
            if (!modifierList.Contains(modifier))
                return;

            modifierList.Remove(modifier);

            if (modifierList.Count == 0)
            {
                OnChangeState?.Invoke(false);
            }
        }

        public bool ContainsModifier(T modifier)
        {
            return modifierList.Contains(modifier);
        }

        public bool ContainsUniqueModifier(T modifier)
        {
            return modifierList.Count == 1 && modifierList.Contains(modifier);
        }
    }
}
