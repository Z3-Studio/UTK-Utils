using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Z3.Utils
{
    public class DebugDrawer : Monostate<DebugDrawer>
    {
        public static bool Active { get; set; } = true;

        private static Dictionary<string, GizmosHandler> gizmosHandler = new();

        private static void Add(Action drawMethod, float duration, string key)
        {
            if (!Application.isPlaying) // TODO: Review it
                return;

            duration = Mathf.Max(duration, Time.deltaTime);

            key ??= Guid.NewGuid().ToString();

            GizmosHandler newHandle = new GizmosHandler(drawMethod, duration);

            gizmosHandler.Add(key, newHandle);

            if (Instance != null)
                return;

            GameObject instance = new GameObject("GlobalGizmos [Generated]");
            instance.AddComponent<DebugDrawer>();
            DontDestroyOnLoad(instance);
        }

        private void OnDrawGizmos()
        {
            foreach ((string key, GizmosHandler handler) in gizmosHandler.ToList())
            {
                if (handler.Finish)
                {
                    handler.Dispose();
                    gizmosHandler.Remove(key);
                }
                else if (Active)
                {
                    handler.Draw();
                }
            }
        }

        public static void DrawSphere(Vector3 position, float size, float duration = 0f, string key = null) => DrawSphere(position, size, Color.magenta, duration, key);

        public static void DrawSphere(Vector3 position, float size, Color color, float duration = 0f, string key = null)
        {
            Add(Draw, duration, key);

            void Draw()
            {
                Gizmos.color = color;
                Gizmos.DrawWireSphere(position, size);
            }
        }

        public static void DrawRaycast(Vector3 origin, Vector3 direction, float distance, int layer, float duration = 0f, string key = null)
        {
            PhysicsUtils.RaycastWithSafeOrigin(origin, direction, out RaycastHit raycastHit, distance, layer);
            DrawRaycast(origin, direction, raycastHit, distance, duration, key);
        }

        public static void DrawRaycast(Vector3 origin, Vector3 direction, RaycastHit raycastHit, float distance, float duration = 0f, string key = null)
        {
            DrawRaycast(origin, direction, raycastHit, distance, Color.green, Color.red, duration, key);
        }

        public static void DrawRaycast(Vector3 origin, Vector3 direction, RaycastHit raycastHit, float distance, Color hitColor, Color failColor, float duration = 0f, string key = null)
        {
            if (raycastHit.collider != null)
            {
                Add(DrawHitCollider, duration, key);

                void DrawHitCollider()
                {
                    Gizmos.color = hitColor;
                    Gizmos.DrawWireSphere(origin, 0.06f);
                    Gizmos.DrawLine(origin, raycastHit.point);
                    Gizmos.DrawWireSphere(raycastHit.point, .03f);
                }
            }
            else
            {
                Vector3 endPoint = origin + direction.normalized * distance;
                Add(DrawFail, duration, key); 
                
                void DrawFail()
                {
                    Gizmos.color = failColor;
                    Gizmos.DrawWireSphere(origin, 0.06f);
                    Gizmos.DrawLine(origin, endPoint);
                }
            }
        }
    }

    public class GizmosHandler : IDisposable
    {
        private Action drawMethod;
        private float disposeTime;

        public bool Finish => Time.time >= disposeTime;

        public GizmosHandler(Action drawMethod, float duration)
        {
            this.drawMethod = drawMethod;
            disposeTime = Time.time + duration;
        }

        public void Draw() => drawMethod();

        public void Dispose()
        {
            drawMethod = null;
        }
    }
}