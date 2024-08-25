using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Editor.Utility
{
    public static class EditorHelpers
    {
        public static List<Type> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()).ToList();
        }
        public static string[] ConvertTypeListToArray<T>(List<T> list)
        {
            return list.Select(type => type.GetType().ToString()).ToArray();
        }

        public static string[] ConvertListToArray(List<string> list)
        {
            return list.ToArray();
        }

        public static List<T> GenerateTypeInstances<T>(List<Type> typeList)
        {
            return typeList.Where(type => typeof(T).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
               .Select(type => (T)Activator.CreateInstance(type))
               .ToList();
        }

        public static List<string> FindTypeNamesByInterface<T>(List<Type> typeList)
        {
            return typeList.Where(type => typeof(T).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
               .Select(type => type.FullName)
               .ToList();
        }

        public static void DrawPropertiesAutomatically(SerializedObject so) 
        {
            var serializedProperty = so.GetIterator();
            serializedProperty.NextVisible(true);
            while (serializedProperty.NextVisible(false))
            {
                EditorGUILayout.PropertyField(serializedProperty, true);
            }
        }

        public static int FindIndexInArray(string[] array, object field)
        {
            if (field == null)
                return 0;

            return array.ToList().FindIndex(type => type == field.ToString());
        }

        public static int FindTypeIndexInArray<T>(List<T> array, T field)
        {
            if (field == null)
                return 0;

            return array.FindIndex(type => type.GetType().Equals(field.GetType()));
        }
    }
}