using UnityEditor;
using UnityEngine;
using System.Reflection;

public static class EditorAudioPreview
{
    static MethodInfo _playMethod;
    static MethodInfo _stopMethod;
    static PropertyInfo _timeProperty;

    static EditorAudioPreview()
    {
        var audioUtil = typeof(AudioImporter).Assembly
            .GetType("UnityEditor.AudioUtil");

        _playMethod = audioUtil.GetMethod(
            "PlayPreviewClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new[] { typeof(AudioClip), typeof(int), typeof(bool) },
            null
        );

        _stopMethod = audioUtil.GetMethod(
            "StopAllPreviewClips",
            BindingFlags.Static | BindingFlags.Public
        );

        _timeProperty = audioUtil.GetProperty(
            "previewClipTime",
            BindingFlags.Static | BindingFlags.Public
        );
    }

    public static void Play(AudioClip clip)
    {
        if (clip == null) return;
        _playMethod?.Invoke(null, new object[] { clip, 0, false });
    }

    public static void Stop()
    {
        _stopMethod?.Invoke(null, null);
    }

    public static float GetTime()
    {
        if (_timeProperty == null) return 0f;
        return (float)_timeProperty.GetValue(null);
    }
}