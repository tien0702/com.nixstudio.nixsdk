using UnityEditor;
using UnityEngine;

public static class AudioMarkerSerializedGUILayout
{
    public static void DrawMarkers(
        SerializedObject audioSO,
        ref Vector2 scroll,
        float height = 280f
    )
    {
        SerializedProperty markersProp = audioSO.FindProperty("Markers");
        if (markersProp == null) return;

        audioSO.Update();

        // ===== OUTER FRAME =====
        GUILayout.BeginVertical(EditorStyles.helpBox);

        // ===== HEADER =====
        GUILayout.Label("Markers", EditorStyles.boldLabel);
        GUILayout.Space(4);

        // ===== LIST (SCROLL) =====
        scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(height));

        for (int i = 0; i < markersProp.arraySize; i++)
        {
            SerializedProperty markerProp =
                markersProp.GetArrayElementAtIndex(i);

            GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();
            GUILayout.Label($"#{i}", GUILayout.Width(30));

            EditorGUILayout.PropertyField(
                markerProp.FindPropertyRelative("Name"),
                GUIContent.none
            );

            if (GUILayout.Button("✕", GUILayout.Width(22)))
            {
                Undo.RecordObject(audioSO.targetObject, "Remove Audio Marker");

                markersProp.DeleteArrayElementAtIndex(i);

                if (i < markersProp.arraySize &&
                    markersProp.GetArrayElementAtIndex(i).objectReferenceValue != null)
                {
                    markersProp.DeleteArrayElementAtIndex(i);
                }

                audioSO.ApplyModifiedProperties();
                GUIUtility.ExitGUI();
            }

            GUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(
                markerProp.FindPropertyRelative("Description")
            );

            EditorGUILayout.PropertyField(
                markerProp.FindPropertyRelative("Time"),
                new GUIContent("Time (s)")
            );

            GUILayout.EndVertical();
            GUILayout.Space(4);
        }

        GUILayout.EndScrollView();

        GUILayout.Space(6);

        // ===== ADD BUTTON (BOTTOM - CENTER) =====
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("+ Add Marker", GUILayout.Width(120)))
        {
            Undo.RecordObject(audioSO.targetObject, "Add Audio Marker");

            int index = markersProp.arraySize;
            markersProp.InsertArrayElementAtIndex(index);

            SerializedProperty element = markersProp.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("Name").stringValue = "New Marker";
            element.FindPropertyRelative("Description").stringValue = "";
            element.FindPropertyRelative("Time").floatValue = 0f;

            // Optional: auto scroll xuống cuối
            scroll.y = float.MaxValue;
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        audioSO.ApplyModifiedProperties();
    }
}
