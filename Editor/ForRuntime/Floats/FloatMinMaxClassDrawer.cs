using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UsefulTools.Editor;

namespace UsefulTools.Runtime.DataStructures.Floats.Editor
{
    public class FloatMinMaxClassDrawer : OdinValueDrawer<FloatMinMax>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            FloatMinMax value = ValueEntry.SmartValue;

            var rect = EditorGUILayout.GetControlRect();

            // In Odin, labels are optional and can be null, so we have to account for that.
            if (label != null)
            {
                rect = EditorGUI.PrefixLabel(rect, label);
            }

            var prev = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 20;

            value.min = EditorGUI.FloatField(rect.AlignLeft(rect.width * 0.5f), "Min", value.min);
            value.max = EditorGUI.FloatField(rect.AlignRight(rect.width * 0.5f), "Max", value.max);

            EditorGUIUtility.labelWidth = prev;

            ValueEntry.SmartValue = value;
        }
    }
}