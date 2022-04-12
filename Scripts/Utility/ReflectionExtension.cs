#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

public static class ReflectionExtension
{
    // 출처: https://www.tangledrealitystudios.com/development-tips/flexible-editor-property-fields-unity/
    public static Type GetPropertyType(this SerializedProperty property)
    {
        string[] splitPropertyPath = property.propertyPath.Split('.');
        Type type = property.serializedObject.targetObject.GetType();

        for (int i = 0; i < splitPropertyPath.Length; i++)
        {
            if (type == null)
                return null;

            if (splitPropertyPath[i] == "Array")
            {
                type = type.GetEnumerableType();
                i++; //skip "data[x]"
            }
            else
                type = type.GetField(splitPropertyPath[i], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance)?.FieldType;
        }

        return type;
    }

    // 출처: https://www.tangledrealitystudios.com/development-tips/flexible-editor-property-fields-unity/
    public static Type GetEnumerableType(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException("type");

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            return type.GetGenericArguments()[0];

        var iface = (from i in type.GetInterfaces()
                     where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                     select i).FirstOrDefault();

        if (iface == null)
            throw new ArgumentException("Does not represent an enumerable type.", "type");

        return GetEnumerableType(iface);
    }
}
#endif