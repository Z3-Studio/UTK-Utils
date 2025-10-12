using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Z3.Utils
{
    public class DebugDrawer : Singleton<DebugDrawer>
    {
        public static bool Active { get; set; } = true;

        private static Dictionary<string, GizmosHandler> gizmosHandler = new();

        private void OnDestroy()
        {
            gizmosHandler.Clear();
        }

        private static void Add(Action drawMethod, float duration, string key)
        {
            if (!Application.isPlaying) // TODO: Review it
                return;

            duration = Mathf.Max(duration, Time.deltaTime);

            key ??= Guid.NewGuid().ToString();

            if (gizmosHandler.TryGetValue(key, out GizmosHandler existing))
            {
                existing.Dispose();
            }

            gizmosHandler[key] = new GizmosHandler(drawMethod, duration);

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

        public static void DrawSphere(Vector3 position, float size = .5f, float duration = 1f, string key = null) => DrawSphere(position, size, Color.magenta, duration, key);

        public static void DrawSphere(Vector3 position, float size, Color color, float duration = 1f, string key = null)
        {
            Add(Draw, duration, key);

            void Draw()
            {
                Gizmos.color = color;
                Gizmos.DrawWireSphere(position, size);
            }
        }

        public static void DrawRaycast(Transform reference, RaycastHit raycastHit, float distance, float duration = 0f, string key = null)
        {
            DrawRaycast(reference.position, reference.forward, raycastHit, distance, Color.green, Color.red, duration, key);
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

        public static void DrawLine(Vector3 from, Vector3 to, float duration = 1f, string key = null) => DrawLine(from, to, Color.magenta, duration, key);

        public static void DrawLine(Vector3 from, Vector3 to, Color color, float duration = 1f, string key = null)
        {
            Add(Draw, duration, key);

            void Draw()
            {
                Gizmos.color = color;
                Gizmos.DrawLine(from, to);
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