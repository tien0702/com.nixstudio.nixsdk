using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PlayerEditor
{
    [MenuItem("Tools/Player/Clear Player Data")]
    private static void ClearUserData()
    {
        bool confirm = EditorUtility.DisplayDialog(
            "Clear Player Data and PlayerPrefs",
            "Do you want to clear all?",
            "Yes",
            "No"
        );
        if (confirm)
        {
            string path = $"{Application.persistentDataPath}/{"Player/player-data.json"}";
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError($"Unable to save data due to: {e.Message} {e.StackTrace}");
                }
            }

            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("Player data cleared");
        }
    }

    [MenuItem("Tools/Player/Open Saved File")]
    public static void OpenJsonFile()
    {
        string filePath = $"{Application.persistentDataPath}/{"Player/player-data.json"}";
        if (!File.Exists(filePath))
        {
            Debug.LogError("JSON file not found at: " + filePath);
            return;
        }

        if (TryOpenWithVSCode(filePath)) return;

        if (TryOpenWithNotepad(filePath)) return;

        if (EditorUtility.DisplayDialog("Open JSON",
                "Neither Visual Studio Code nor Notepad is available. Do you want to select an app manually?", "Yes",
                "No"))
        {
            TryOpenWithSystemDefault(filePath);
        }
    }

    [MenuItem("Tools/Player/Open Default Player File")]
    public static void OpenDefaultJsonFile()
    {
        string filePath = AssetDatabase.GetAssetPath(Resources.Load("player-data-default"));
        if (!File.Exists(filePath))
        {
            Debug.LogError("JSON file not found at: " + filePath);
            return;
        }

        if (TryOpenWithVSCode(filePath)) return;

        if (TryOpenWithNotepad(filePath)) return;

        if (EditorUtility.DisplayDialog("Open JSON",
                "Neither Visual Studio Code nor Notepad is available. Do you want to select an app manually?", "Yes",
                "No"))
        {
            TryOpenWithSystemDefault(filePath);
        }
    }

    private static bool TryOpenWithVSCode(string filePath)
    {
        try
        {
            System.Diagnostics.ProcessStartInfo processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "code",
                Arguments = "\"" + filePath + "\"",
                UseShellExecute = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
            };

            System.Diagnostics.Process.Start(processInfo);
            Debug.Log("Opened JSON file in Visual Studio Code.");
            return true;
        }
        catch
        {
            Debug.LogWarning("Visual Studio Code not found.");
            return false;
        }
    }

    private static bool TryOpenWithNotepad(string filePath)
    {
        try
        {
            System.Diagnostics.ProcessStartInfo processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "notepad.exe",
                Arguments = "\"" + filePath + "\"",
                UseShellExecute = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
            };

            System.Diagnostics.Process.Start(processInfo);
            Debug.Log("Opened JSON file in Notepad.");
            return true;
        }
        catch
        {
            Debug.LogWarning("Notepad not found.");
            return false;
        }
    }

    private static void TryOpenWithSystemDefault(string filePath)
    {
        try
        {
            System.Diagnostics.ProcessStartInfo processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
            };

            System.Diagnostics.Process.Start(processInfo);
            Debug.Log("Opened JSON file with the system default app.");
        }
        catch
        {
            Debug.LogError("Failed to open JSON file with the system default app.");
        }
    }
}