using UnityEngine;

namespace Z3.Utils
{
    public static class GizmosUtils
    {
        public static void DrawRaycast(Vector3 origin, Vector3 direction, float max, int layer, bool active = false)
        {
            bool hit = PhysicsUtils.RaycastWithSafeOrigin(origin, direction, out RaycastHit raycastHit, max, layer);

            Vector3 end = hit ? raycastHit.point : origin + direction.normalized * max;

            DrawRaycast(origin, end, hit, active);
        }

        public static void DrawRaycast(Vector3 start, Vector3 end, bool hit, bool active)
        {
            Gizmos.color = !active ? new(1f, 1f, 1f, 0.5f) : hit ? Color.magenta : Color.black;

            Gizmos.DrawWireSphere(start, 0.06f);
            Gizmos.DrawLine(start, end);

            if (hit)
                Gizmos.DrawWireSphere(end, 0.03f);
        }
    }
}