using Metroidvania;
using UnityEditor;
using UnityEngine;

namespace MetroidvaniaEditor {
    [CustomPropertyDrawer(typeof(RangedValueAttribute))]
    public class RangedValueAttributeDrawer : PropertyDrawer {
        // Right/left number fields width
        private const float k_FieldWidth = 40;

        // Space between the right/left fields and the slider
        private const float k_FieldSpace = 5;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            RangedValueAttribute minMaxAttribute = (RangedValueAttribute)attribute;
            string propertyType = property.type;

            label.tooltip = $"{minMaxAttribute.min:F2} to {minMaxAttribute.max:F2}";

            Rect controlRect = EditorGUI.PrefixLabel(position, label);

            SplitRect(controlRect, out Rect leftField, out Rect sliderField, out Rect rightField);

            SerializedProperty min = property.FindPropertyRelative("min"), max = property.FindPropertyRelative("max");

            label = EditorGUI.BeginProperty(position, label, property);
            if (propertyType == nameof(RangedFloat)) {
                EditorGUI.BeginChangeCheck();
                float minVal = min.floatValue;
                float maxVal = max.floatValue;

                minVal = EditorGUI.FloatField(leftField, minVal);
                maxVal = EditorGUI.FloatField(rightField, maxVal);

                EditorGUI.MinMaxSlider(sliderField, ref minVal, ref maxVal, minMaxAttribute.min, minMaxAttribute.max);

                if (minVal < minMaxAttribute.min)
                    minVal = minMaxAttribute.min;

                if (maxVal > minMaxAttribute.max)
                    maxVal = minMaxAttribute.max;

                if (EditorGUI.EndChangeCheck()) {
                    min.floatValue = minVal;
                    max.floatValue = maxVal;
                }

            } else if (propertyType == nameof(RangedInt)) {

                EditorGUI.BeginChangeCheck();

                float minVal = min.intValue;
                float maxVal = max.intValue;

                minVal = EditorGUI.FloatField(leftField, minVal);
                maxVal = EditorGUI.FloatField(rightField, maxVal);

                EditorGUI.MinMaxSlider(sliderField, ref minVal, ref maxVal, minMaxAttribute.min, minMaxAttribute.max);

                if (minVal < minMaxAttribute.min)
                    maxVal = minMaxAttribute.min;

                if (minVal > minMaxAttribute.max)
                    maxVal = minMaxAttribute.max;

                if (EditorGUI.EndChangeCheck()) {
                    min.intValue = Mathf.FloorToInt(minVal > maxVal ? maxVal : minVal);
                    max.intValue = Mathf.FloorToInt(maxVal);
                }
            }
            EditorGUI.EndProperty();
        }

        private static void SplitRect(Rect rectToSplit, out Rect left, out Rect slider, out Rect right) {
            left = new Rect(rectToSplit.x, rectToSplit.y, k_FieldWidth, rectToSplit.height);
            right = new Rect(rectToSplit.x + rectToSplit.width - k_FieldWidth, rectToSplit.y, k_FieldWidth, rectToSplit.height);
            slider = new Rect(rectToSplit.x + k_FieldSpace + k_FieldWidth, rectToSplit.y, rectToSplit.width - ((k_FieldWidth + k_FieldSpace) * 2), rectToSplit.height);
        }
    }
}