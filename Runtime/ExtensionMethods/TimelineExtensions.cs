using UnityEngine.Playables;

namespace Z3.Utils.ExtensionMethods
{
    public static class TimelineExtensions
    {
        public static int GetCurrentInput(this Playable playable)
        {
            int inputCount = playable.GetInputCount();
            int currentInput = -1;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                if (inputWeight > 0)
                    currentInput = i;
            }

            return currentInput;
        }

        public static bool HasInputOfType<T>(this Playable playable) where T : class, IPlayableBehaviour, new()
        {
            int inputCount = playable.GetInputCount();

            for (int i = 0; i < inputCount; i++)
            {
                Playable input = playable.GetInput(i);

                if (input.GetPlayableType() == typeof(T))
                    return true;
            }

            return false;
        }

        public static bool IsType<T>(this Playable playable) where T : class, IPlayableBehaviour, new()
        {
            return playable.GetPlayableType() == typeof(T);
        }

        public static T GetBehaviour<T>(this Playable playable) where T : class, IPlayableBehaviour, new()
        {
            ScriptPlayable<T> scriptPlayable = (ScriptPlayable<T>)playable;
            return scriptPlayable.GetBehaviour();
        }
    }
}