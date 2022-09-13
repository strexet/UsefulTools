using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace UsefulTools.Runtime.DataStructures.Floats.Editor
{
    public class FloatWithSpreadClassDrawer : OdinValueDrawer<FloatWithSpread>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var smartValue = ValueEntry.SmartValue;
            float value = smartValue.value;
            float spread = smartValue.spread;

            var rect = EditorGUILayout.GetControlRect();

            if (label != null)
            {
                rect = EditorGUI.PrefixLabel(rect, label);
            }

            GUIHelper.PushLabelWidth(66f);
            value = SirenixEditorFields.FloatField(rect.AlignLeft(rect.width * 0.5f), "Value", value);
            GUIHelper.PopLabelWidth();

            GUIHelper.PushLabelWidth(74f);
            spread = SirenixEditorFields.FloatField(rect.AlignRight(rect.width * 0.5f), "Spread", spread);
            GUIHelper.PopLabelWidth();

            smartValue.value = value;
            smartValue.spread = spread;

            ValueEntry.SmartValue = smartValue;
        }
    }
}