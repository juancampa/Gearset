using System;
using System.Collections.Generic;
using System.Reflection;

namespace Gearset.Helpers
{
    public static class ReflectionExtensionMethods
    {
        #region Get Instance Field/Property

        /// <summary>
        /// Return public Instance fields.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="skipPrivate"></param>
        /// <returns></returns>
        public static FieldInfo[] GetInstanceFields(this Type t, bool skipPrivate = true)
        {
            if (skipPrivate)
                return t.GetFields(BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance);
            else
                return t.GetFields(BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static List<MemberInfo> GetFieldsAndProperties(this Type t, BindingFlags bindingAttr)
        {
            var targetMembers = new List<MemberInfo>();

            targetMembers.AddRange(t.GetFields(bindingAttr));
            targetMembers.AddRange(t.GetProperties(bindingAttr));

            return targetMembers;
        }

        /// <summary>
        /// Return public Instance properties.
        /// </summary>
        public static PropertyInfo[] GetInstanceProperties(this Type t, bool skipPrivate = true)
        {
            if (skipPrivate)
                return t.GetProperties(BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance);
            else
                return t.GetProperties(BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Return public Instance methods.
        /// </summary>
        public static MethodInfo[] GetInstanceMethods(this Type t)
        {
            return t.GetMethods(BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// Return public Static methods.
        /// </summary>
        public static MethodInfo[] GetStaticMethods(this Type t)
        {
            return t.GetMethods(BindingFlags.Default | BindingFlags.Public | BindingFlags.Static);
        } 
        #endregion
    }
}
