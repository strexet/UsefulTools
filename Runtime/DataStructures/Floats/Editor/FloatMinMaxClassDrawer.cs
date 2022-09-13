using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace UsefulTools.Runtime.DataStructures.Floats.Editor
{
    public class FloatMinMaxClassDrawer : OdinValueDrawer<FloatMinMax>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var smartValue = ValueEntry.SmartValue;
            float min = smartValue.min;
            float max = smartValue.max;

            var rect = EditorGUILayout.GetControlRect();

            if (label != null)
            {
                rect = EditorGUI.PrefixLabel(rect, label);
            }

            GUIHelper.PushLabelWidth(40f);
            min = SirenixEditorFields.FloatField(rect.AlignLeft(rect.width * 0.5f), "Min", min);
            GUIHelper.PopLabelWidth();

            GUIHelper.PushLabelWidth(43f);
            max = SirenixEditorFields.FloatField(rect.AlignRight(rect.width * 0.5f), "Max", max);
            GUIHelper.PopLabelWidth();

            smartValue.min = min;
            smartValue.max = max;

            ValueEntry.SmartValue = smartValue;
        }
    }
}