using System;
using System.Collections.Generic;
using GrowAGarden.Module.Audio;
using UnityEditor;
using UnityEngine;

public class AudioEditorWindow : EditorWindow
{
    private const int width = 500;
    private const int height = 100;
    private Texture2D waveformTexture;
    private AudioSource previewAudioSource;
    private float previewTime;
    private AudioSO audioSo;
    private SerializedObject _serializedSO;
    private Vector2 _scroll;

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


    private void OnEnable()
    {
        if (previewAudioSource == null)
        {
            // Create an audio source for previewing the sound
            GameObject audioPreviewer = new GameObject("AudioPreviewer");
            previewAudioSource = audioPreviewer.AddComponent<AudioSource>();
            previewAudioSource.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    private void OnGUI()
    {
        audioSo = (AudioSO)EditorGUILayout.ObjectField("Audio So", audioSo, typeof(AudioSO), false);
        _serializedSO = new SerializedObject(audioSo);

        if (audioSo != null)
        {
            AudioWaveformGUILayout.DrawWaveform(
                audioSo,
                height: 100f
            );
        }

        // Display waveform texture
        if (waveformTexture != null)
        {
            GUILayout.Label("Waveform Preview");

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Box(waveformTexture,
                GUILayout.Width(waveformTexture.width),
                GUILayout.Height(waveformTexture.height));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            //Display the playhead
            Rect waveformRect = GUILayoutUtility.GetLastRect();
            previewAudioSource.clip = audioSo.AudioClip;
            float playheadPosition =
                Mathf.Min(((previewAudioSource.time) / (previewAudioSource.clip.length / 2f)) * waveformRect.width,
                    waveformRect.width);
            Rect playheadRect = new Rect(playheadPosition, GUILayoutUtility.GetLastRect().y, 2, height);
            EditorGUI.DrawRect(playheadRect, Color.red);

            /*if (isPlaying)
            {
            }*/
        }

        AudioMarkerSerializedGUILayout.DrawMarkers(
            _serializedSO,
            ref _scroll
        );

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

    private Texture2D DrawWaveform(AudioClip clip, Color waveformColor)
    {
        Texture2D texture = new Texture2D(width, height);
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        Color[] colors = new Color[width * height];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(0.2f, 0.2f, 0.2f); // Background color
        }

        // Calculate the range of samples based on trim
        int trimSamples = Mathf.FloorToInt(clip.length * clip.frequency * clip.channels);

        int packSize = (trimSamples / width) + 1; // Calculate packSize based on the trimmed range

        for (int i = 0; i < width; i++)
        {
            float max = 0;
            for (int j = 0; j < packSize; j++)
            {
                int index = (i * packSize) + j;
                if (index < samples.Length)
                {
                    float wavePeak = Mathf.Abs(samples[index]);
                    if (wavePeak > max) max = wavePeak;
                }
            }

            int heightPos = Mathf.FloorToInt(max * (height / 2f));
            for (int j = 0; j < heightPos; j++)
            {
                colors[(height / 2 + j) * width + i] = waveformColor;
                colors[(height / 2 - j) * width + i] = waveformColor;
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        return texture;
    }
}