using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor
{
    public static class EditorUIHelper
    {
        public static Rect GetNextRect(ref Rect position)
        {
            var nextRect = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
            float height = EditorGUIUtility.singleLineHeight + 1f;
            position = new Rect(position.xMin, position.yMin + height, position.width, position.height = height);
            return nextRect;
        }
        
        public static void DrawPropertiesInLine(Rect position, params SerializedProperty[] properties)
        {
            if (properties.Length == 0)
            {
                return;
            }
            
            const float spaceRatio = 0.025f;
            var propertiesCount = properties.Length;

            var rect = GetNextRect(ref position);
            var space = spaceRatio * rect.width;
            var sectionWidth = (rect.width - space * propertiesCount - 1) / (2 * propertiesCount);
            
            var startRect = new Rect(rect.xMin, rect.yMin, 0, rect.height);
            var lastRect = startRect;
            
            DrawProperty(properties[0], sectionWidth, 0, ref lastRect);
            
            for (int i = 1; i < propertiesCount; i++)
            {
                DrawProperty(properties[i], sectionWidth, space, ref lastRect);
            }
        }

        private static void DrawProperty(SerializedProperty property, float sectionWidth, float spaceWidth, ref Rect lastRect)
        {
            var labelRect = new Rect(lastRect.xMax + spaceWidth, lastRect.yMin, sectionWidth, lastRect.height);
            var valueRect = new Rect(labelRect.xMax, lastRect.yMin, sectionWidth, lastRect.height);

            EditorGUI.LabelField(labelRect, property.name);
            EditorGUI.PropertyField(valueRect, property, GUIContent.none, false);

            lastRect = valueRect;
        }
    }
}