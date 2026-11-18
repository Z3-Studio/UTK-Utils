using System.Collections;
using System.Linq;
using UnityEngine;

namespace Z3.Utils.ExtensionMethods
{
    public static class AnimatorExtensions
    {
        public static void PlayStateAllLayers(this Animator animator, string stateName, float transition = 0.1f)
        {
            int stateId = Animator.StringToHash(stateName);
            for (int layerIndex = 0; layerIndex <= animator.layerCount; layerIndex++)
            {
                if (!animator.HasState(layerIndex, stateId))
                    return;

                animator.CrossFadeInFixedTime(stateName, transition, layerIndex);
            }
        }

        public static void PlayOrResetState(this Animator animator, string stateName, float transition = 0.1f, int layerIndex = 0)
        {
            if (animator.IsState(stateName, layerIndex))
            {
                animator.Play(stateName, layerIndex, 0f);
            }
            else
            {
                animator.CrossFadeInFixedTime(stateName, transition, layerIndex);
            }
        }

        public static bool IsState(this Animator animator, string stateName, int layer = 0) // Can be in transition between Target -> Next
        {
            return animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName);
        }

        public static bool IsStatePlaying(this Animator animator, string stateName, int layerIndex = 0) // Is playing and not finished
        {
            AnimatorStateInfo current = animator.GetCurrentAnimatorStateInfo(layerIndex);
            if (!current.IsName(stateName))
                return false;

            return current.normalizedTime < 1f;
        }

        public static bool IsFullyInState(this Animator animator, string stateName, int layer = 0) // No transitions
        {
            bool isName = animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName);
            if (isName)
            {
                return !animator.IsInTransition(layer);
            }
            return false;
        }

        public static bool IsStateFinished(this Animator animator, string stateName, int layerIndex = 0) // End of this state. Not recommended to use in states with transitions
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            return stateInfo.IsName(stateName) && stateInfo.normalizedTime >= 1f;
        }

        public static bool IsCurrentOrNextState(this Animator animator, string stateName, int layerIndex = 0) // If is playing only A, or transition between A -> B  or B -> A, all cases A will be true
        {
            if (animator.IsInTransition(layerIndex) && animator.GetNextAnimatorStateInfo(layerIndex).IsName(stateName))
                return true;

            return animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
        }

        public static bool IsInTransition(this Animator animator, int layer)
        {
            return animator.GetAnimatorTransitionInfo(layer).normalizedTime > 0;
        }

        public static void PlayOppositeAnimation(this Animator animator, string stateName, float transition)
        {
            float currentProgress = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            float newProgress = Mathf.Clamp01(1f - currentProgress);
            animator.CrossFade(stateName, transition, -1, newProgress);
        }

        public static float GetClipDuration(this Animator animator, string clipName)
        {
            AnimationClip clip = animator.GetClip(clipName);
            return clip != null ? clip.length : 0f;
        }

        public static AnimationClip GetClip(this Animator animator, string clipName)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            return clips.FirstOrDefault(clip => clip.name.Contains(clipName));
        }

        public static void TryPlay(this Animator animator, string stateName)
        {
            if (animator.runtimeAnimatorController)
            {
                animator.Play(stateName);
            }
        }

        public static void TrySetFloat(this Animator animator, string name, float value)
        {
            if (animator.runtimeAnimatorController)
            {
                animator.SetFloat(name, value);
            }
        }

        public static IEnumerator PlayCoroutine(this Animator animator, string animation, int layerIndex = 0)
        {
            animator.Play(animation);
            yield return new WaitForEndOfFrame();

            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(layerIndex);
            float animationDuration = info.length;

            yield return new WaitForSeconds(animationDuration);
            yield return new WaitForEndOfFrame();
        }
    }
}