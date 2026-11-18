using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Z3.Utils.ExtensionMethods
{
    public static class ReflectionExtensions
    {
        public static bool IsAssignableFromAny(this MemberInfo member, params Type[] types)
        {
            foreach (Type type in types)
            {
                bool hasType = member.IsAssignableFrom(type);
                if (hasType)
                    return true;
            }

            return false;
        }

        public static bool IsAssignableFrom(this MemberInfo member, Type type)
        {
            if (member is PropertyInfo propertyInfo)
            {
                return propertyInfo.PropertyType.IsAssignableFrom(type);
            }
            else if (member is FieldInfo fieldInfo)
            {
                return fieldInfo.FieldType.IsAssignableFrom(type);
            }

            return false;
        }

        public static IEnumerable<T> GetAllFieldValuesTypeOf<T>(this object target)
        {
            return ReflectionUtils.GetAllFieldValuesTypeOf<T>(target);
        }

        public static void ForEach<T>(this IEnumerable<T> values, Action<T> action) // LINQ extensions?
        {
            foreach (T item in values)
            {
                action.Invoke(item);
            }
        }

        public static IEnumerable<T> GetValues<T>(this IEnumerable<FieldInfo> values, object target)
        {
            return values.Select(t => (T)t.GetValue(target)).ToList();
        }

        public static IEnumerable<T> GetValues<T>(this IEnumerable<PropertyInfo> values, object target)
        {
            return values.Select(t => (T)t.GetValue(target)).ToList();
        }

        public static T GetValue<T>(this FieldInfo fieldInfo, object target)
        {
            return (T)fieldInfo.GetValue(target);
        }

        public static bool IsNullable(this Type type)
        {
            return type.IsClass || Nullable.GetUnderlyingType(type) != null;
        }

        public static bool IsValidSubType(this Type type, object obj)
        {
            if (obj == null)
            {
                return type.IsNullable();
            }

            return type.IsAssignableFrom(obj.GetType());
        }

        public static object GetDefaultValueForType(this Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        public static IEnumerable<FieldInfo> GetSerializedFields(this Type type)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (FieldInfo field in fields)
            {
                if (field.IsPublic || Attribute.IsDefined(field, typeof(SerializeField)))
                {
                    yield return field;
                }
            }
        }

        public static bool TryGetBackingField(this PropertyInfo propertyInfo, out FieldInfo fieldInfo)
        {
            fieldInfo = GetBackingField(propertyInfo);
            return fieldInfo != null;
        }

        public static FieldInfo GetBackingField(this PropertyInfo propertyInfo)
        {
            return propertyInfo.DeclaringType.GetField($"<{propertyInfo.Name}>k__BackingField", ReflectionUtils.InstanceAccess);
        }

        public static bool TryGetBackingField(this Type parentType, string memberName, out FieldInfo fieldInfo)
        {
            fieldInfo = memberName.EndsWith(">k__BackingField") ? GetBackingField(parentType, memberName) : null;
            return fieldInfo != null;
        }

        public static FieldInfo GetBackingField(this Type parentType, string memberName)
        {
            // GetField/GetProperty don't return private members declared in base classes, so walk the hierarchy
            for (Type type = parentType; type != null; type = type.BaseType)
            {
                FieldInfo field = type.GetField(memberName, ReflectionUtils.InstanceDeclared);
                if (field != null)
                    return field;
            }

            return null;
        }
    }
}
