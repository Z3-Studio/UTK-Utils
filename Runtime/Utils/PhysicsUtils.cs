using UnityEngine;

namespace Z3.Utils
{
    public static class PhysicsUtils
    {
        /// <summary>
        /// <para> Check that the origin is free of collisions using a small overlapping sphere. </para>
        /// <para> If there are collisions, the raycast result will be false. </para> 
        /// </summary>
        public static bool RaycastWithSafeOrigin(Vector3 origin, Vector3 direction, out RaycastHit raycastHit, float max, int layer, float minValue = 0.02f)
        {
            if (Physics.CheckSphere(origin, minValue, layer))
            {
                raycastHit = default;
                return false;
            }

            return Physics.Raycast(origin, direction, out raycastHit, max, layer);
        }

        public static bool RaycastWithSafeOrigin(Transform reference, out RaycastHit raycastHit, float max = float.PositiveInfinity, int layer = -1, float minValue = 0.02f)
        {
            if (Physics.CheckSphere(reference.position, minValue, layer))
            {
                raycastHit = default;
                return false;
            }

            return Physics.Raycast(reference.position, reference.forward, out raycastHit, max, layer);
        }

        public static bool RaycastWithSafeOrigin_Debug(Transform reference, out RaycastHit raycastHit, float max = float.PositiveInfinity, int layer = -1, float minValue = 0.02f, float duration = 0f, string key = null)
        {
            bool result = RaycastWithSafeOrigin(reference, out raycastHit, max, layer, minValue);
            DebugDrawer.DrawRaycast(reference, raycastHit, max);
            return result;
        }
    }
}
