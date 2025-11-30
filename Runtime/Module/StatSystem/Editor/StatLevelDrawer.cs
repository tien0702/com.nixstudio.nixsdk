#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;

namespace NIX.Module.StatSystem
{
    [CustomPropertyDrawer(typeof(StatLevel))]
    public class StatLevelDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + 6f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var baseProp = property.FindPropertyRelative("BaseValue");
            var maxProp = property.FindPropertyRelative("MaxFinalValue");

            int index = GetIndexFromPath(property.propertyPath) + 1;

            var row = new Rect(position.x, position.y + 3f, position.width, EditorGUIUtility.singleLineHeight);

            float lvLabelW = 46f;
            float gap = 8f;

            var lvRect = new Rect(row.x, row.y, lvLabelW, row.height);
            EditorGUI.LabelField(lvRect, $"Lv {index}");

            float remain = row.width - lvLabelW - gap;
            float half = remain / 2f;

            var baseRect = new Rect(lvRect.xMax + gap, row.y, half - gap / 2f, row.height);
            var maxRect = new Rect(baseRect.xMax + gap, row.y, half - gap / 2f, row.height);

            float oldLabelW = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 90f;

            EditorGUI.PropertyField(baseRect, baseProp, new GUIContent("Base value"));
            EditorGUI.PropertyField(maxRect, maxProp, new GUIContent("Max Final Value"));

            EditorGUIUtility.labelWidth = oldLabelW;

            EditorGUI.EndProperty();
        }

        private static int GetIndexFromPath(string path)
        {
            var m = Regex.Match(path, @"Array\.data\[(\d+)\]");
            return m.Success ? int.Parse(m.Groups[1].Value) : -1;
        }
    }
#endif

    
}