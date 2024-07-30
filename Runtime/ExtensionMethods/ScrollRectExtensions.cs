using System;
using UnityEngine;
using UnityEngine.UI;

namespace Z3.Utils.ExtensionMethods
{
    public static class ScrollRectExtensions
    {
        /// <summary> Useful to create function like "Refresh" or "LoadMore" </summary>
        public static void ScrollLimit(this ScrollRect scroll, Action onScrollUp, Action onScrollDown, float spaceToActiveScrollUp = 80f, float spaceToActiveScrollDown = 80f)
        {
            GetDifference(scroll, out float upDifference, out float downDifference);

            if (upDifference > spaceToActiveScrollUp)
            {
                onScrollUp?.Invoke();
            }
            else if (downDifference < spaceToActiveScrollDown)
            {
                onScrollDown?.Invoke();
            }
        }

        public static void GetDifference(this ScrollRect scroll, out float upDifference, out float downDifference)
        {
            Vector3[] viewPortCorners = new Vector3[4];
            Vector3[] contentCorners = new Vector3[4];
            scroll.viewport.GetWorldCorners(viewPortCorners);
            scroll.content.GetWorldCorners(contentCorners);

            upDifference = viewPortCorners[2].y - contentCorners[2].y;
            downDifference = viewPortCorners[0].y - contentCorners[0].y;
        }
    }
}