using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using Z3.Utils.ExtensionMethods;

namespace Z3.Utils
{
    public static class ReflectionUtils
    {
        /// <summary> All instance members, including public and private </summary>
        /// <remarks> Inherited private members excluded </remarks>
        public const BindingFlags InstanceAccess = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        /// <summary> All declared instance members, including public and private </summary>
        /// <remarks> Excluding inherited members and static </remarks>
        public const BindingFlags InstanceDeclared = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly;

        /// <summary> All declared in scope </summary>
        /// <remarks> All inherited members excluded </remarks>
        public const BindingFlags AllDeclared = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static;

        public static IEnumerable<Type> GetDeriveredConcreteTypes<T>()
        {
             return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract && t != typeof(T));
        }

        
        public static IEnumerable<Type> GetDerivedConcreteTypesInAssembly<T>()
        {
            return Assembly.GetAssembly(typeof(T))
                .GetTypes()
                .Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract && t != typeof(T));
        }

        public static bool HasAttribute<T>(object target) where T : Attribute
        {
            return target.GetType().IsDefined(typeof(T));
        }

        public static FieldInfo GetField(object target, string name)
        {
            return GetMember(target, type => type.GetField(name, InstanceDeclared));
        }

        public static List<FieldInfo> GetAllFields(object target)
        {
            return GetAllMembers(target, (t) => t.GetFields(InstanceDeclared));
        }

        public static List<MemberInfo> GetAllMembers(object target, BindingFlags bindingFlags = InstanceDeclared, string ignoreNamespace = null)
        {
            return GetAllMembers(target, (t) => t.GetMembers(bindingFlags));
        }

        public static IEnumerable<FieldInfo> GetAllFieldsTypeOf<T>(object target)
        {
            return GetAllFields(target).Where(f => typeof(T).IsAssignableFrom(f.FieldType));
        }

        public static IEnumerable<T> GetAllFieldValuesTypeOf<T>(object target)
        {
            return GetAllFieldsTypeOf<T>(target).GetValues<T>(target);
        }

        public static Type GetGenericArgumentFromBaseType(Type type, Type genericType)
        {
            Type baseType = type.BaseType;

            while (baseType != null)
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == genericType)
                {
                    return baseType.GetGenericArguments()[0];
                }

                baseType = baseType.BaseType;
            }

            return null;
        }

        public static IEnumerable<TMember> GetAllMembersByAttribute<TMember>(object target, Type attribute, string ignoreNamespace = null) where TMember : MemberInfo
        {
            List<MemberInfo> fields = new List<MemberInfo>();

            // Pega a classe atual e a classe base recursivamente até chegar em object
            Type currentType = target.GetType();

            bool hasReadOnlyAttribute = false;
            while (currentType is not null && (ignoreNamespace == null || !currentType.Namespace.Contains(ignoreNamespace)))
            {
                // Verifica se a classe atual tem o atributo ReadOnly
                if (hasReadOnlyAttribute || currentType.GetCustomAttributes(attribute, false).Length > 0)
                {
                    // Pega todos os campos da classe atual
                    hasReadOnlyAttribute = true;
                    fields.AddRange(currentType.GetMembers(AllDeclared));
                }
                else
                {
                    // Pega apenas os campos que têm o atributo ReadOnly
                    var list = currentType.GetMembers(AllDeclared).Where(m => m.IsDefined(attribute));
                    fields.AddRange(list);
                }

                // Vai para a classe base
                currentType = currentType.BaseType;
            }

            return fields.OfType<TMember>();
        }

        public static Dictionary<FieldInfo, IEnumerable<TAttribute>> GetAllFieldsWithAttributes2<TAttribute>(object target, string ignoreNamespace = null) where TAttribute : Attribute
        {
            Dictionary<FieldInfo, IEnumerable<TAttribute>> members = new();

            Type currentType = target.GetType();
            Type attributeType = typeof(TAttribute);

            // Get all field from all inherit classes
            while (currentType is not null && (ignoreNamespace == null || !currentType.Namespace.Contains(ignoreNamespace)))
            {
                // Get all field with attribute and into the dictionary
                currentType.GetFields(AllDeclared)
                    .ToDictionary(
                        fieldInfo => fieldInfo,
                        fieldInfo => fieldInfo.GetCustomAttributes<TAttribute>())
                    .Where(pair => pair.Value.Any())
                    .ForEach(pair => members[pair.Key] = pair.Value);

                // Vai para a classe base
                currentType = currentType.BaseType;
            }

            return members;
        }

        #region GetAllWithAttribute
        /// <summary> Single Attribute from <see cref="FieldInfo"/></summary>
        public static List<(FieldInfo, TAttribute)> GetAllFieldsWithAttribute<TAttribute>(object target, string ignoreNamespace = null) where TAttribute : Attribute
        {
            return GetAllMembersWithAttribute<FieldInfo, TAttribute>(target, type => type.GetFields(AllDeclared), ignoreNamespace);
        }

        /// <summary> Multiple Attribute from <see cref="FieldInfo"/></summary>
        public static List<(FieldInfo, IEnumerable<TAttribute>)> GetAllFieldsWithAttributes<TAttribute>(object target, string ignoreNamespace = null, BindingFlags flags = AllDeclared) where TAttribute : Attribute
        {
            return GetAllMembersWithAttributes<FieldInfo, TAttribute>(target, type => type.GetFields(flags), ignoreNamespace);
        }

        /// <summary> Single Attribute from <see cref="PropertyInfo"/></summary>
        public static List<(PropertyInfo, TAttribute)> GetAllPropertyWithAttribute<TAttribute>(object target, string ignoreNamespace = null, BindingFlags flags = AllDeclared) where TAttribute : Attribute
        {
            return GetAllMembersWithAttribute<PropertyInfo, TAttribute>(target, type => type.GetProperties(flags), ignoreNamespace);
        }

        /// <summary> Multiple Attribute from <see cref="PropertyInfo"/></summary>
        public static List<(PropertyInfo, IEnumerable<TAttribute>)> GetAllPropertyWithAttributes<TAttribute>(object target, string ignoreNamespace = null, BindingFlags flags = AllDeclared) where TAttribute : Attribute
        {
            return GetAllMembersWithAttributes<PropertyInfo, TAttribute>(target, type => type.GetProperties(flags), ignoreNamespace);
        }

        /// <summary> Single Attribute from <see cref="MethodInfo"/></summary>
        public static List<(MethodInfo, TAttribute)> GetAllMethodsWithAttribute<TAttribute>(object target, string ignoreNamespace = null, BindingFlags flags = AllDeclared) where TAttribute : Attribute
        {
            return GetAllMembersWithAttribute<MethodInfo, TAttribute>(target, type => type.GetMethods(flags), ignoreNamespace);
        }

        /// <summary> Multiple Attribute from <see cref="MethodInfo"/></summary>
        public static List<(MethodInfo, IEnumerable<TAttribute>)> GetAllMethodsWithAttributes<TAttribute>(object target, string ignoreNamespace = null, BindingFlags flags = AllDeclared) where TAttribute : Attribute
        {
            return GetAllMembersWithAttributes<MethodInfo, TAttribute>(target, type => type.GetMethods(flags), ignoreNamespace);

        }

        /// <summary> Single Attribute from <see cref="MemberInfo"/></summary>
        public static List<(MemberInfo, TAttribute)> GetAllMembersWithAttribute<TAttribute>(object target, string ignoreNamespace = null) where TAttribute : Attribute
        {
            return GetAllMembersWithAttribute<MemberInfo, TAttribute>(target, type => type.GetMembers(AllDeclared), ignoreNamespace);
        }

        /// <summary> Multiple Attribute from <see cref="MemberInfo"/></summary>
        public static List<(MemberInfo, IEnumerable<TAttribute>)> GetAllMembersWithAttributes<TAttribute>(object target, string ignoreNamespace = null) where TAttribute : Attribute
        {
            return GetAllMembersWithAttributes<MemberInfo, TAttribute>(target, type => type.GetMembers(AllDeclared), ignoreNamespace);
        }

        /// <summary> Single Attribute from generic member </summary>
        private static List<(TMember, TAttribute)> GetAllMembersWithAttribute<TMember, TAttribute>(object target, Func<Type, TMember[]> getMembers, string ignoreNamespace = null) where TAttribute : Attribute where TMember : MemberInfo
        {
            return GetAllMembersWithAttributes<TMember, TAttribute>(target, getMembers, ignoreNamespace)
                .Select(d => (d.Item1, d.Item2.First())).ToList();
        }

        /// <summary> Multiple Attribute from generic member </summary>
        private static List<(TMember, IEnumerable<TAttribute>)> GetAllMembersWithAttributes<TMember, TAttribute>(object target, Func<Type, TMember[]> getMembers, string ignoreNamespace = null) where TAttribute : Attribute where TMember : MemberInfo
        {
            return GetAllMembers(target, getMembers, ignoreNamespace)
                .Select(member => (member, member.GetCustomAttributes<TAttribute>()))
                .Where(tupla => tupla.Item2.Any())
                .ToList();
        }
        #endregion

        public static TMember GetMember<TMember>(object target, Func<Type, TMember> getMember) where TMember : MemberInfo
        {
            Type currentType = target.GetType();

            // Get all members from all inherit classes
            while (currentType is not null)
            {
                // Get all members with attribute and into the dictionary
                TMember member = getMember(currentType);
                if (member != null)
                    return member;

                // Go to base class
                currentType = currentType.BaseType;
            }

            return null;
        }

        public static List<TMember> GetAllMembers<TMember>(object target, Func<Type, TMember[]> getMembers, string ignoreNamespace = null) where TMember : MemberInfo
        {
            List<TMember> memberList = new();

            Type currentType = target.GetType();

            // Small optimization
            Func<Type, bool> checkWhile;
            if (ignoreNamespace == null)
            {
                checkWhile = (type) => type is not null;
            }
            else
            {
                // TODO: Fix it when namespace is empty
                checkWhile = (type) => type is not null && type.Namespace != null && !type.Namespace.Contains(ignoreNamespace);
            }

            // Get all members from all inherit classes
            while (checkWhile(currentType))
            {
                // Get all members with attribute and into the dictionary
                TMember[] newMembers = getMembers(currentType);
                memberList.AddRange(newMembers);

                // Go to base class
                currentType = currentType.BaseType;
            }

            return memberList;
        }
    }
}