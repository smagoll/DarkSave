using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace DarkSave.Runtime
{
    public static class SaveSystemUtils
    {
        public static bool IsSerializableRecursively(Type type, HashSet<Type> visited = null)
        {
            if (type == null)
                return false;

            visited ??= new HashSet<Type>();

            if (!visited.Add(type))
                return true;

            if (type.IsPrimitive || type == typeof(string) || type.IsEnum)
                return true;

            if (Nullable.GetUnderlyingType(type) != null)
                return IsSerializableRecursively(Nullable.GetUnderlyingType(type), visited);

            if (typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType)
            {
                var elementType = type.GetGenericArguments()[0];
                return IsSerializableRecursively(elementType, visited);
            }

            if (!type.IsSerializable && !typeof(UnityEngine.Object).IsAssignableFrom(type))
                return false;

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (field.IsNotSerialized)
                    continue;

                if (!IsSerializableRecursively(field.FieldType, visited))
                    return false;
            }

            return true;
        }
    }
}