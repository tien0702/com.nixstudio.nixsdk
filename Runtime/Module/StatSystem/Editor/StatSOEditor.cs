#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NIX.Module.StatSystem
{
    [CustomEditor(typeof(StatSO))]
    [CanEditMultipleObjects]
    public class StatSOEditor : UnityEditor.Editor
    {
        private SerializedProperty _statId;
        private SerializedProperty _levels;
        private ReorderableList _list;

        // Tùy chỉnh bề rộng cột (tính theo pixel)
        private const float ColLevelWidth = 50f; // Cột "Level"
        private const float ColGap = 10f; // Khoảng cách giữa các cột

        private void OnEnable()
        {
            _statId = serializedObject.FindProperty("StatId");
            _levels = serializedObject.FindProperty("Levels");

            if (_levels == null || !_levels.isArray) return;

            _list = new ReorderableList(serializedObject, _levels, true, true, true, true);

            // Chiều cao mỗi hàng
            _list.elementHeight = EditorGUIUtility.singleLineHeight + 6f;

            // Header (tiêu đề cột)
            _list.drawHeaderCallback = rect =>
            {
                // Cột Level
                var levelRect = new Rect(rect.x, rect.y, ColLevelWidth, rect.height);
                EditorGUI.LabelField(levelRect, "Level", EditorStyles.boldLabel);

                // Cột Base + Max chia phần còn lại
                float remain = rect.width - ColLevelWidth;
                float half = (remain - ColGap) / 2f;

                var baseRect = new Rect(levelRect.xMax + ColGap, rect.y, half, rect.height);
                var maxRect = new Rect(baseRect.xMax + ColGap, rect.y, half, rect.height);

                EditorGUI.LabelField(baseRect, "Base Value", EditorStyles.boldLabel);
                EditorGUI.LabelField(maxRect, "Max Final Value", EditorStyles.boldLabel);
            };

            // Vẽ từng hàng
            _list.drawElementCallback = (rect, index, active, focused) =>
            {
                rect.y += 3f;
                rect.height = EditorGUIUtility.singleLineHeight;

                var element = _levels.GetArrayElementAtIndex(index);
                var baseProp = element.FindPropertyRelative("BaseValue");
                var maxProp = element.FindPropertyRelative("MaxFinalValue");

                // Cột Level (nhãn)
                var levelRect = new Rect(rect.x, rect.y, ColLevelWidth, rect.height);
                EditorGUI.LabelField(levelRect, $"Level {index + 1}");

                // Cột Base / Max
                float remain = rect.width - ColLevelWidth;
                float half = (remain - ColGap) / 2f;

                var baseRect = new Rect(levelRect.xMax + ColGap, rect.y, half, rect.height);
                var maxRect = new Rect(baseRect.xMax + ColGap, rect.y, half, rect.height);

                // Vẽ 2 ô input dạng field rút gọn (không label trong ô)
                EditorGUI.BeginChangeCheck();

                // Gắn label gọn trong placeholder bằng PrefixLabel (tuỳ thích); hoặc bỏ label trong field:
                var oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 0f; // không chiếm label phía trái

                baseProp.floatValue = EditorGUI.FloatField(baseRect, baseProp.floatValue);
                maxProp.floatValue = EditorGUI.FloatField(maxRect, maxProp.floatValue);

                EditorGUIUtility.labelWidth = oldLabelWidth;

                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            };

            // Tuỳ chọn thêm: set giá trị mặc định khi Add
            _list.onAddCallback = list =>
            {
                _levels.arraySize++;
                var el = _levels.GetArrayElementAtIndex(_levels.arraySize - 1);
                el.FindPropertyRelative("BaseValue").floatValue = 0f;
                el.FindPropertyRelative("MaxFinalValue").floatValue = 0f;
                serializedObject.ApplyModifiedProperties();
            };
        }

        public override void OnInspectorGUI()
        {
            if (_statId == null)
            {
                DrawDefaultInspector();
                return;
            }

            serializedObject.Update();

            // StatID ở trên
            EditorGUILayout.PropertyField(_statId);

            EditorGUILayout.Space(8);

            // Bảng Levels
            if (_list != null)
            {
                _list.DoLayoutList();
            }
            else
            {
                // fallback
                EditorGUILayout.PropertyField(_levels, new GUIContent("Levels"), true);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}