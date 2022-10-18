using UnityEditor;
using UnityEngine;
using UsefulTools.Editor;

namespace UsefulTools.Runtime.DataStructures.Floats.Editor
{
    [CustomPropertyDrawer(typeof(FloatWithSpread), true)]
    public class FloatWithSpreadClassDrawer : PropertyDrawer
    {
        private const string ValuePropertyName = nameof(FloatWithSpread.value);
        private const string SpreadPropertyName = nameof(FloatWithSpread.spread);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProp = property.FindPropertyRelative(ValuePropertyName);
            var spreadProp = property.FindPropertyRelative(SpreadPropertyName);

            EditorUIHelper.DrawPropertiesInLine(position, valueProp, spreadProp);
        }
    }
}