using System;
using System.Collections.Generic;
using UnityEngine;

namespace Z3.Utils
{
    // TODO: Review interface GetComponent<T> where T : class
    public class CachedComponents
    {
        public GameObject GameObject { get; }

        private readonly Dictionary<Type, Component> components;

        public CachedComponents(Component defaultComponent)
        {
            GameObject = defaultComponent.gameObject;
            components = new Dictionary<Type, Component>
            {
                { defaultComponent.GetType(), defaultComponent }
            };
        }

        public Component GetCachedComponent(Type componentType)
        {
            return GetCachedComponent<Component>(componentType);
        }

        public T GetCachedComponent<T>() where T : class
        {
            Type componentType = typeof(T);
            return GetCachedComponent<T>(componentType);
        }

        public T GetCachedComponent<T>(Type componentType) where T : class
        {
            // Optimizing get component 
            if (components.TryGetValue(componentType, out Component component))
            {
                // If the cached component is not null, return it
                if (component)
                {
                    return component as T;
                }

                components.Remove(componentType);
            }

            // Default get component and cache
            Component componentT = GameObject.GetComponent(componentType);
            components[componentType] = componentT;
            return componentT as T;
        }

        /// <summary>
        /// Little bit faster than GetCachedComponent if you have tho invoke many times
        /// </summary>
        public Func<Component> CreateGetter(Type componentType)
        {
            Component cachedComponent = null;

            return () =>
            {
                if (!cachedComponent)
                {
                    cachedComponent = GetCachedComponent(componentType);
                }

                return cachedComponent;
            };
        }
    }
}