using System.Collections;
using System.Linq;
using UnityEngine;

namespace Z3.Utils.ExtensionMethods
{
    public static class AnimatorExtensions
    {
        public static void PlayStateAllLayers(this Animator animator, string stateName, float transition = 0.25f)
        {
            for (int layerIndex = 0; layerIndex <= animator.layerCount; layerIndex++)
            {
                PlayState(animator, stateName, transition, layerIndex);
            }
        }

        /// <summary>
        /// Similar than <see cref="Animator.CrossFadeInFixedTime"/> but the transition is normalized, using the state to be calculated
        /// </summary>
        public static void PlayState(this Animator animator, string stateName, float transition = 0.25f, int layerIndex = 0)
        {
            AnimatorStateInfo current = animator.GetCurrentAnimatorStateInfo(layerIndex);
            animator.CrossFade(stateName, transition / current.length, layerIndex);
        }

        public static bool IsState(this Animator animator, string stateName, int layer)
        {
            bool isName = animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName);
            if (isName)
            {
                return !animator.IsInTransition(layer);
            }
            return false;
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