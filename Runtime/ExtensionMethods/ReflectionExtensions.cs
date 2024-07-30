using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;

namespace Z3.Utils.ExtensionMethods
{
    public static class ReflectionExtensions
    {
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
    }
}