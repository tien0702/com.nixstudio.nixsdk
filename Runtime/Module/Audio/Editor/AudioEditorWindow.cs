using System;
using System.Collections.Generic;
using GrowAGarden.Module.Audio;
using UnityEditor;
using UnityEngine;

public class AudioEditorWindow : EditorWindow
{
    [MenuItem("Tools/Audio/Audio Config")]
    public static void SelectAudioConfig()
    {
        Selection.activeObject = AudioConfigSO.Instance;
        EditorGUIUtility.PingObject(AudioConfigSO.Instance);
    }

    [MenuItem("Tools/Audio/Audio Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<AudioEditorWindow>("Audio Editor");
        window.minSize = new Vector2(700, 450);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Create AudioSO by Config"))
        {
            Debug.Log("Create SFX by Config");
            {
                string[] sfxPaths = new string[AudioConfigSO.Instance.SfxSources.Count];
                for (var i = 0; i < AudioConfigSO.Instance.SfxSources.Count; i++)
                {
                    var folder = AudioConfigSO.Instance.SfxSources[i];
                    sfxPaths[i] = AssetDatabase.GetAssetPath(folder);
                }

                string saveSfxPath = AssetDatabase.GetAssetPath(AudioConfigSO.Instance.SfxExportFolder);
                List<AudioClip> sfxClips = LoadAllAudioClips(sfxPaths);

                foreach (var audioClip in sfxClips)
                {
                    /*AssetDatabaseUtils.TryCreateSo<AudioSO>($"{saveSfxPath}/{audioClip.name}.asset", () =>
                    {
                        var asset = ScriptableObject.CreateInstance<AudioSO>();
                        asset.AudioClip = audioClip;
                        return asset;
                    });*/
                }
            }
            Debug.Log("Create BGM by Config");
            {
                string[] musicPaths = new string[AudioConfigSO.Instance.MusicSources.Count];
                for (var i = 0; i < AudioConfigSO.Instance.MusicSources.Count; i++)
                {
                    var folder = AudioConfigSO.Instance.MusicSources[i];
                    musicPaths[i] = AssetDatabase.GetAssetPath(folder);
                }

                string saveMusicPath = AssetDatabase.GetAssetPath(AudioConfigSO.Instance.MusicExportFolder);
                List<AudioClip> musicClips = LoadAllAudioClips(musicPaths);

                foreach (var audioClip in musicClips)
                {
                    /*AssetDatabaseUtils.TryCreateSo<AudioSO>($"{saveMusicPath}/{audioClip.name}.asset", () =>
                    {
                        var asset = ScriptableObject.CreateInstance<AudioSO>();
                        asset.AudioClip = audioClip;
                        return asset;
                    });*/
                }
            }
        }
    }

    private List<AudioClip> LoadAllAudioClips(string[] folderPaths)
    {
        List<AudioClip> audioClips = new List<AudioClip>();
        string[] guids = AssetDatabase.FindAssets("t:AudioClip", folderPaths);
        foreach (var guid in guids)
        {
            string clipPath = AssetDatabase.GUIDToAssetPath(guid);
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(clipPath);

            if (clip != null)
                audioClips.Add(clip);
        }

        return audioClips;
    }
}