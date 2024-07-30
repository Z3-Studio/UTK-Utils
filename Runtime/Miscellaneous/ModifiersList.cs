using System;
using System.Collections.Generic;

namespace Z3.Utils
{
    public class ModifiersList<T>
    {
        public Action<bool> OnChangeState;
        public bool HasActiveModifier => modifierList.Count > 0;

        private readonly List<T> modifierList = new List<T>();

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
    }
}