using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using System.Linq;
using UnityEngine;
using EditorScripts;

namespace SpriteCanvasSystem.Editor
{
    [CustomEditor(typeof(UIElement), true)]
    [CanEditMultipleObjects]
    public class UISpriteDrawer : UnityEditor.Editor
    {
        private List<Type> _types;
        private void OnEnable()
        {
            _types = EditorHelpers.GetAssemblies();
        }
        public override void OnInspectorGUI()
        {
            UIElement script = (UIElement)target;
            var serializedObject = new SerializedObject(script);
            EditorHelpers.DrawPropertiesAutomatically(serializedObject);

            var implementingTypes = EditorHelpers.GenerateTypeInstances<IResponsiveOperation>(_types);
            var typeNames = EditorHelpers.ConvertTypeListToArray(implementingTypes);
            var currentIndex = EditorHelpers.FindTypeIndexInArray(implementingTypes, script.ResponsiveOperation);

            EditorGUI.BeginChangeCheck();
            var selectedIndex = EditorGUILayout.Popup("Responsive", currentIndex, typeNames);
            if (EditorGUI.EndChangeCheck())
            {
                ReflectSpriteOperation(script, script.SpriteOperationName, implementingTypes[selectedIndex]);
                EditorUtility.SetDirty(script);
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void ReflectSpriteOperation(UIElement sprite, string targetFieldName, IResponsiveOperation spriteUIOperationHandler)
        {
            const BindingFlags instanceNonPublicFlags = BindingFlags.Instance | BindingFlags.NonPublic;

            var spriteFields = sprite.GetType().GetFields(instanceNonPublicFlags).ToList();
            var targetField = spriteFields.Find(field => field.Name == targetFieldName);

            // todo: Bad method. It needs fixing.
            if (sprite is UITextMeshPro || sprite is UISprite || sprite is UIButton)
            {
                object spriteObject = sprite switch
                {
                    UITextMeshPro => sprite,
                    UISprite => sprite,
                    UIButton => sprite,
                    _ => null
                };

                if (targetField.GetValue(spriteObject) == null)
                    targetField.SetValue(spriteObject, spriteUIOperationHandler);

                if (targetField.GetValue(spriteObject)?.GetType() == spriteUIOperationHandler.GetType())
                    return;
            }

            targetField.SetValue(sprite, spriteUIOperationHandler);
        }

        private List<IResponsiveOperation> GetImplementingTypes(Type interfaceType)
        {
            return _types.Where(type => interfaceType.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                .Select(type => Activator.CreateInstance(type) as IResponsiveOperation)
                .ToList();
        }
    }
}