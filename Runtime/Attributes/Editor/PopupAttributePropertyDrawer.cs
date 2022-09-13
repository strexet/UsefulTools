using System.Collections.Generic;
using Khutor.Scripts.Attributes;
using UnityEditor;
using UnityEngine;

namespace Khutor.Editor.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(PopupAttribute))]
    public class PopupAttributePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var atb = attribute as PopupAttribute;
            var field = atb?.ContainingType.GetField(atb.PropertyName);

            if (field == null)
                return;

            var propertyType = property.serializedObject.targetObject.GetType();
            object value = field.GetValue(propertyType);
            bool didDisplayPopup = false;

            if (value is List<string> options
                && options.Count != 0)
            {
                int selectedIndex = options.IndexOf(property.stringValue);

                if (selectedIndex == -1 && string.IsNullOrEmpty(property.stringValue))
                    selectedIndex = 0;

                if (selectedIndex >= 0)
                {
                    selectedIndex = EditorGUI.Popup(position, property.name, selectedIndex, options.ToArray());
                    property.stringValue = options[selectedIndex];
                    didDisplayPopup = true;
                }
            }

            if (!didDisplayPopup)
                EditorGUI.PropertyField(position, property, label);
        }
    }
}