using System;
using UnityEditor;
using UnityEngine;

namespace UsefulTools.Runtime.DataStructures.Collections.Editor
{
    [CustomPropertyDrawer(typeof(DrawableDictionary), true)]
    public class DrawableDictionaryPropertyDrawer : PropertyDrawer
    {
        protected const string KeysPropertyName = "_Keys";
        protected const string ValuePropertyName = "_Values";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                var keysProp = property.FindPropertyRelative(KeysPropertyName);
                return (keysProp.arraySize + 2) * (EditorGUIUtility.singleLineHeight + 1f);
            }

            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool expanded = property.isExpanded;
            var rect = GetNextRect(ref position);
            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);

            if (expanded)
            {
                int lvl = EditorGUI.indentLevel;
                EditorGUI.indentLevel = lvl + 1;

                var keysProp = property.FindPropertyRelative(KeysPropertyName);
                var valuesProp = property.FindPropertyRelative(ValuePropertyName);

                int count = keysProp.arraySize;
                if (valuesProp.arraySize != count)
                {
                    valuesProp.arraySize = count;
                }

                for (int i = 0; i < count; i++)
                {
                    rect = GetNextRect(ref position);

                    float keyWidth = EditorGUIUtility.labelWidth;
                    float valueWidth = rect.width - keyWidth;

                    var keyRect = new Rect(rect.xMin, rect.yMin, keyWidth, rect.height);
                    var valueRect = new Rect(keyRect.xMax, rect.yMin, valueWidth, rect.height);

                    var keyProp = keysProp.GetArrayElementAtIndex(i);
                    var valueProp = valuesProp.GetArrayElementAtIndex(i);

                    DrawKey(keyRect, keyProp);
                    DrawValue(valueRect, valueProp);
                }

                EditorGUI.indentLevel = lvl;

                rect = GetNextRect(ref position);
                var plusButtonRect = new Rect(rect.xMax - 60f, rect.yMin, 30f, EditorGUIUtility.singleLineHeight);
                var minusButtonRect = new Rect(rect.xMax - 30f, rect.yMin, 30f, EditorGUIUtility.singleLineHeight);

                if (GUI.Button(plusButtonRect, "+"))
                {
                    AddKeyElement(keysProp);
                    valuesProp.arraySize = keysProp.arraySize;
                }

                if (GUI.Button(minusButtonRect, "-"))
                {
                    keysProp.arraySize = Mathf.Max(keysProp.arraySize - 1, 0);
                    valuesProp.arraySize = keysProp.arraySize;
                }
            }
        }

        protected virtual void DrawKey(Rect area, SerializedProperty keyProp) =>
            EditorGUI.PropertyField(area, keyProp, GUIContent.none, false);

        protected virtual void DrawValue(Rect area, SerializedProperty valueProp) =>
            EditorGUI.PropertyField(area, valueProp, GUIContent.none, false);

        private Rect GetNextRect(ref Rect position)
        {
            var nextRect = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
            float height = EditorGUIUtility.singleLineHeight + 1f;
            position = new Rect(position.xMin, position.yMin + height, position.width, position.height = height);
            return nextRect;
        }

        private static void AddKeyElement(SerializedProperty keysProp)
        {
            keysProp.arraySize++;
            var lastKeyProp = keysProp.GetArrayElementAtIndex(keysProp.arraySize - 1);

            switch (lastKeyProp.propertyType)
            {
                case SerializedPropertyType.Integer:
                {
                    int value = 0;
                    for (int i = 0; i < keysProp.arraySize - 1; i++)
                    {
                        if (keysProp.GetArrayElementAtIndex(i).intValue == value)
                        {
                            value++;
                            if (value == int.MaxValue)
                            {
                                break;
                            }

                            i = -1;
                        }
                    }

                    lastKeyProp.intValue = value;
                    break;
                }

                case SerializedPropertyType.Boolean:
                {
                    bool value = false;
                    for (int i = 0; i < keysProp.arraySize - 1; i++)
                    {
                        if (keysProp.GetArrayElementAtIndex(i).boolValue == value)
                        {
                            value = true;
                            break;
                        }
                    }

                    lastKeyProp.boolValue = value;
                    break;
                }

                case SerializedPropertyType.Float:
                {
                    float value = 0f;
                    for (int i = 0; i < keysProp.arraySize - 1; i++)
                    {
                        if (keysProp.GetArrayElementAtIndex(i).intValue == value)
                        {
                            value++;
                            if (value == int.MaxValue)
                            {
                                break;
                            }

                            i = -1;
                        }
                    }

                    lastKeyProp.floatValue = value;
                    break;
                }

                case SerializedPropertyType.String:
                {
                    lastKeyProp.stringValue = string.Empty;
                    break;
                }

                case SerializedPropertyType.Color:
                {
                    var value = Color.black;
                    for (int i = 0; i < keysProp.arraySize - 1; i++)
                    {
                        if (keysProp.GetArrayElementAtIndex(i).colorValue == value)
                        {
                            value = IntToColor(ColorToInt(value) + 1);
                            if (value == Color.white)
                            {
                                break;
                            }

                            i = -1;
                        }
                    }

                    lastKeyProp.colorValue = value;
                    break;
                }

                case SerializedPropertyType.ObjectReference:
                {
                    lastKeyProp.objectReferenceValue = null;
                    break;
                }

                case SerializedPropertyType.LayerMask:
                {
                    int value = -1;
                    for (int i = 0; i < keysProp.arraySize - 1; i++)
                    {
                        if (keysProp.GetArrayElementAtIndex(i).intValue == value)
                        {
                            value++;
                            if (value == int.MaxValue)
                            {
                                break;
                            }

                            i = -1;
                        }
                    }

                    lastKeyProp.intValue = value;
                    break;
                }

                case SerializedPropertyType.Enum:
                {
                    int value = 0;
                    if (keysProp.arraySize > 1)
                    {
                        var first = keysProp.GetArrayElementAtIndex(0);
                        int max = first.enumNames.Length - 1;

                        for (int i = 0; i < keysProp.arraySize - 1; i++)
                        {
                            if (keysProp.GetArrayElementAtIndex(i).enumValueIndex == value)
                            {
                                value++;
                                if (value >= max)
                                {
                                    break;
                                }

                                i = -1;
                            }
                        }
                    }

                    lastKeyProp.enumValueIndex = value;
                    break;
                }

                case SerializedPropertyType.Vector2:
                {
                    var value = Vector2.zero;
                    for (int i = 0; i < keysProp.arraySize - 1; i++)
                    {
                        if (keysProp.GetArrayElementAtIndex(i).vector2Value == value)
                        {
                            value.x++;
                            if (value.x == int.MaxValue)
                            {
                                break;
                            }

                            i = -1;
                        }
                    }

                    lastKeyProp.vector2Value = value;
                    break;
                }

                case SerializedPropertyType.Vector3:
                {
                    var value = Vector3.zero;
                    for (int i = 0; i < keysProp.arraySize - 1; i++)
                    {
                        if (keysProp.GetArrayElementAtIndex(i).vector3Value == value)
                        {
                            value.x++;
                            if (value.x == int.MaxValue)
                            {
                                break;
                            }

                            i = -1;
                        }
                    }

                    lastKeyProp.vector3Value = value;
                    break;
                }

                case SerializedPropertyType.Vector4:
                {
                    var value = Vector4.zero;
                    for (int i = 0; i < keysProp.arraySize - 1; i++)
                    {
                        if (keysProp.GetArrayElementAtIndex(i).vector4Value == value)
                        {
                            value.x++;
                            if (value.x == int.MaxValue)
                            {
                                break;
                            }

                            i = -1;
                        }
                    }

                    lastKeyProp.vector4Value = value;
                    break;
                }

                case SerializedPropertyType.Rect:
                {
                    lastKeyProp.rectValue = Rect.zero;
                    break;
                }

                case SerializedPropertyType.ArraySize:
                {
                    int value = 0;
                    for (int i = 0; i < keysProp.arraySize - 1; i++)
                    {
                        if (keysProp.GetArrayElementAtIndex(i).arraySize == value)
                        {
                            value++;
                            if (value == int.MaxValue)
                            {
                                break;
                            }

                            i = -1;
                        }
                    }

                    lastKeyProp.arraySize = value;
                    break;
                }

                case SerializedPropertyType.Character:
                {
                    int value = 0;
                    for (int i = 0; i < keysProp.arraySize - 1; i++)
                    {
                        if (keysProp.GetArrayElementAtIndex(i).intValue == value)
                        {
                            value++;
                            if (value == char.MaxValue)
                            {
                                break;
                            }

                            i = -1;
                        }
                    }

                    lastKeyProp.intValue = value;
                    break;
                }

                case SerializedPropertyType.AnimationCurve:
                {
                    lastKeyProp.animationCurveValue = null;
                    break;
                }

                case SerializedPropertyType.Bounds:
                {
                    lastKeyProp.boundsValue = default;
                    break;
                }

                default:
                    throw new InvalidOperationException("Can not handle Type as key.");
            }
        }

        private static void SetPropertyDefault(SerializedProperty prop)
        {
            if (prop == null)
            {
                throw new ArgumentNullException("prop");
            }

            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    prop.intValue = 0;
                    break;

                case SerializedPropertyType.Boolean:
                    prop.boolValue = false;
                    break;

                case SerializedPropertyType.Float:
                    prop.floatValue = 0f;
                    break;

                case SerializedPropertyType.String:
                    prop.stringValue = string.Empty;
                    break;

                case SerializedPropertyType.Color:
                    prop.colorValue = Color.black;
                    break;

                case SerializedPropertyType.ObjectReference:
                    prop.objectReferenceValue = null;
                    break;

                case SerializedPropertyType.LayerMask:
                    prop.intValue = -1;
                    break;

                case SerializedPropertyType.Enum:
                    prop.enumValueIndex = 0;
                    break;

                case SerializedPropertyType.Vector2:
                    prop.vector2Value = Vector2.zero;
                    break;

                case SerializedPropertyType.Vector3:
                    prop.vector3Value = Vector3.zero;
                    break;

                case SerializedPropertyType.Vector4:
                    prop.vector4Value = Vector4.zero;
                    break;

                case SerializedPropertyType.Rect:
                    prop.rectValue = Rect.zero;
                    break;

                case SerializedPropertyType.ArraySize:
                    prop.arraySize = 0;
                    break;

                case SerializedPropertyType.Character:
                    prop.intValue = 0;
                    break;

                case SerializedPropertyType.AnimationCurve:
                    prop.animationCurveValue = null;
                    break;

                case SerializedPropertyType.Bounds:
                    prop.boundsValue = default;
                    break;

                case SerializedPropertyType.Gradient:
                    throw new InvalidOperationException("Can not handle Gradient types.");
            }
        }

        private static int ColorToInt(Color color) =>
            (Mathf.RoundToInt(color.a * 255) << 24) +
            (Mathf.RoundToInt(color.r * 255) << 16) +
            (Mathf.RoundToInt(color.g * 255) << 8) +
            Mathf.RoundToInt(color.b * 255);

        private static Color IntToColor(int value)
        {
            float a = ((value >> 24) & 0xFF) / 255f;
            float r = ((value >> 16) & 0xFF) / 255f;
            float g = ((value >> 8) & 0xFF) / 255f;
            float b = (value & 0xFF) / 255f;
            return new Color(r, g, b, a);
        }
    }
}