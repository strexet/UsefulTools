using UnityEditor;
using UnityEngine;
using UsefulTools.Editor;

namespace UsefulTools.Runtime.DataStructures.Floats.Editor
{
    public class FloatMinMaxClassDrawer : PropertyDrawer
    {
        private const string MinPropertyName = nameof(FloatMinMax.min);
        private const string MaxPropertyName = nameof(FloatMinMax.max);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var minProp = property.FindPropertyRelative(MinPropertyName);
            var maxProp = property.FindPropertyRelative(MaxPropertyName);

            EditorUIHelper.DrawPropertiesInLine(position, minProp, maxProp);
        }
    }
    }
}