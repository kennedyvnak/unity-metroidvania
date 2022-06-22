using Metroidvania;
using UnityEditor;
using UnityEngine;

namespace MetroidvaniaEditor
{
    [CustomPropertyDrawer(typeof(RangedValueAttribute))]
    public class RangedValueAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            RangedValueAttribute minMaxAttribute = (RangedValueAttribute)attribute;
            string propertyType = property.type;

            label.tooltip = minMaxAttribute.min.ToString("F2") + " to " + minMaxAttribute.max.ToString("F2");

            Rect controlRect = EditorGUI.PrefixLabel(position, label);

            Rect[] splitRect = SplitRect(controlRect, 3);

            SerializedProperty min = property.FindPropertyRelative("min"), max = property.FindPropertyRelative("max");

            if (propertyType == nameof(RangedFloat))
            {
                EditorGUI.BeginChangeCheck();
                float minVal = min.floatValue;
                float maxVal = max.floatValue;

                //F2 limits the float to two decimal places (0.00).
                minVal = EditorGUI.FloatField(splitRect[0], float.Parse(minVal.ToString("F2")));
                maxVal = EditorGUI.FloatField(splitRect[2], float.Parse(maxVal.ToString("F2")));

                EditorGUI.MinMaxSlider(splitRect[1], ref minVal, ref maxVal, minMaxAttribute.min, minMaxAttribute.max);

                if (minVal < minMaxAttribute.min)
                    minVal = minMaxAttribute.min;

                if (maxVal > minMaxAttribute.max)
                    maxVal = minMaxAttribute.max;

                if (EditorGUI.EndChangeCheck())
                {
                    min.floatValue = minVal;
                    max.floatValue = maxVal;
                }

            }
            else if (propertyType == nameof(RangedInt))
            {

                EditorGUI.BeginChangeCheck();

                float minVal = min.intValue;
                float maxVal = max.intValue;

                minVal = EditorGUI.FloatField(splitRect[0], minVal);
                maxVal = EditorGUI.FloatField(splitRect[2], maxVal);

                EditorGUI.MinMaxSlider(splitRect[1], ref minVal, ref maxVal, minMaxAttribute.min, minMaxAttribute.max);

                if (minVal < minMaxAttribute.min)
                    maxVal = minMaxAttribute.min;

                if (minVal > minMaxAttribute.max)
                    maxVal = minMaxAttribute.max;

                if (EditorGUI.EndChangeCheck())
                {
                    min.intValue = Mathf.FloorToInt(minVal > maxVal ? maxVal : minVal);
                    max.intValue = Mathf.FloorToInt(maxVal);
                }
            }
        }

        private static Rect[] SplitRect(Rect rectToSplit, int n)
        {
            Rect[] rects = new Rect[n];

            for (int i = 0; i < n; i++)
                rects[i] = new Rect(rectToSplit.position.x + (i * rectToSplit.width / n), rectToSplit.position.y, rectToSplit.width / n, rectToSplit.height);

            int padding = (int)rects[0].width - 40;
            int space = 5;

            rects[0].width -= padding + space;
            rects[2].width -= padding + space;

            rects[1].x -= padding;
            rects[1].width += padding * 2;

            rects[2].x += padding + space;

            return rects;
        }
    }
}