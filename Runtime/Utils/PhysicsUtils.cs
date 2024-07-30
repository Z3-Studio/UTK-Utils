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
    }
}