using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BaseDataSO), true)]
public class BaseDataSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        BaseDataSO dataSo = (BaseDataSO)target;

        GUILayout.Space(5);

        if (GUILayout.Button("Save", GUILayout.Height(25)))
        {
            dataSo.Save();
            EditorUtility.SetDirty(dataSo);
            Debug.Log("[BaseDataSO] Saved");
        }

        if (GUILayout.Button("Load", GUILayout.Height(25)))
        {
            dataSo.Load();
            EditorUtility.SetDirty(dataSo);
            Debug.Log("[BaseDataSO] Loaded");
        }

        if (GUILayout.Button("Delete", GUILayout.Height(25)))
        {
            if (EditorUtility.DisplayDialog(
                    "Delete Save Data",
                    "Are you sure you want to delete player save?",
                    "Yes",
                    "Cancel"))
            {
                dataSo.Delete();
                Debug.Log("[BaseDataSO] Deleted");
            }
        }

        if (GUILayout.Button("Open Save Folder", GUILayout.Height(25)))
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }

        GUILayout.Space(5);
        EditorGUILayout.HelpBox(
            "Persistent Path:\n" + Application.persistentDataPath,
            MessageType.Info);
    }
}