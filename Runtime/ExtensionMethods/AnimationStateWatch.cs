using System;
using UnityEngine;

namespace Z3.Utils.ExtensionMethods
{
    public enum EAnimationCheck
    {
        /// <summary> Current or next </summary>
        Playing = 0,

        /// <summary> Current, no transition </summary>
        PlayingNoTransition = 1,

        /// <summary> Enter transition ended </summary>
        EnterEnd = 2,

        /// <summary> Clip reached the end. Looping: first loop </summary>
        ClipEnd = 3,

        /// <summary> Exit transition started </summary>
        ExitStart = 4,

        /// <summary> Exit transition ended </summary>
        ExitEnd = 5,
    }

    public enum EAnimationPlayEnd
    {
        DontWait = 0,
        EnterEnd = 1,
        ClipEnd = 2,
        ExitStart = 3,
        ExitEnd = 4,
    }

    public enum EAnimationStatePhase
    {
        None = 0,
        Entering = 1,
        Inside = 2,
        Exiting = 3,
    }

    public sealed class AnimationStateWatch
    {
        private Animator animator;
        private EAnimationCheck check;
        private int stateHash;
        private int layer;
        private bool requireEnter;
        private bool entered;

        public void Begin(Animator animator, string stateName, int layer, EAnimationPlayEnd end, bool requireEnter)
        {
            Begin(animator, stateName, layer, ToCheck(end), requireEnter);
        }

        public void Begin(Animator animator, string stateName, int layer, EAnimationCheck check, bool requireEnter)
        {
            this.animator = animator;
            stateHash = Animator.StringToHash(stateName);
            this.layer = layer;
            this.check = check;
            this.requireEnter = requireEnter;
            entered = false;
        }

        public bool IsMet()
        {
            EAnimationStatePhase phase = ReadPhase(animator, layer, stateHash, out float time);
            entered |= phase != EAnimationStatePhase.None;

            if (requireEnter && !entered)
                return false;

            return IsCheckMet(check, phase, time);
        }

        public static EAnimationStatePhase ReadPhase(Animator animator, int layer, int stateHash, out float time)
        {
            AnimatorStateInfo current = animator.GetCurrentAnimatorStateInfo(layer);
            bool isCurrent = Matches(current, stateHash);

            if (animator.IsInTransition(layer))
            {
                AnimatorStateInfo next = animator.GetNextAnimatorStateInfo(layer);
                if (Matches(next, stateHash)) // Also a transition to itself: the new instance
                {
                    time = next.normalizedTime;
                    return EAnimationStatePhase.Entering;
                }

                time = current.normalizedTime;
                return isCurrent ? EAnimationStatePhase.Exiting : EAnimationStatePhase.None;
            }

            time = current.normalizedTime;
            return isCurrent ? EAnimationStatePhase.Inside : EAnimationStatePhase.None;
        }

        public static bool IsCheckMet(EAnimationCheck check, EAnimationStatePhase phase, float time) => check switch
        {
            EAnimationCheck.Playing => phase != EAnimationStatePhase.None,
            EAnimationCheck.PlayingNoTransition => phase == EAnimationStatePhase.Inside,
            EAnimationCheck.EnterEnd => phase != EAnimationStatePhase.Entering,
            EAnimationCheck.ClipEnd => phase == EAnimationStatePhase.None || time >= 1f,
            EAnimationCheck.ExitStart => phase is EAnimationStatePhase.Exiting or EAnimationStatePhase.None,
            EAnimationCheck.ExitEnd => phase == EAnimationStatePhase.None,
            _ => throw new ArgumentOutOfRangeException(nameof(check), check, null),
        };

        public static EAnimationCheck ToCheck(EAnimationPlayEnd end) => end switch
        {
            EAnimationPlayEnd.EnterEnd => EAnimationCheck.EnterEnd,
            EAnimationPlayEnd.ClipEnd => EAnimationCheck.ClipEnd,
            EAnimationPlayEnd.ExitStart => EAnimationCheck.ExitStart,
            EAnimationPlayEnd.ExitEnd => EAnimationCheck.ExitEnd,
            _ => throw new ArgumentOutOfRangeException(nameof(end), end, null),
        };

        private static bool Matches(AnimatorStateInfo info, int stateHash)
        {
            return info.shortNameHash == stateHash || info.fullPathHash == stateHash;
        }
    }
}
