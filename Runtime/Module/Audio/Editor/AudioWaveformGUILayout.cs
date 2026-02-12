using GrowAGarden.Module.Audio;
using UnityEditor;
using UnityEngine;

public static class AudioWaveformGUILayout
{
    private static float[] _samplesCache;
    private static AudioSO _audioSo;

    private static bool _isPlaying;
    private static EditorWindow _ownerWindow;

    // ================= PUBLIC API =================
    public static void DrawWaveform(AudioSO so, float height = 80f)
    {
        if (so == null)
        {
            EditorGUILayout.HelpBox("No AudioClip assigned.", MessageType.Info);
            return;
        }

        // window đang vẽ GUI thực sự (Inspector / CustomEditor)
        if (_ownerWindow == null)
            _ownerWindow = EditorWindow.mouseOverWindow;

        DrawPlaybackControls(so.AudioClip);

        Rect rect = GUILayoutUtility.GetRect(
            0f,
            height,
            GUILayout.ExpandWidth(true)
        );

        DrawWaveformInternal(rect, so.AudioClip);
    }

    // ================= WAVEFORM =================
    private static void DrawWaveformInternal(Rect rect, AudioClip clip)
    {
        if (Event.current.type != EventType.Repaint)
            return;

        EditorGUI.DrawRect(rect, new Color(0.18f, 0.18f, 0.18f));

        CacheSamples(_audioSo);

        int width = Mathf.FloorToInt(rect.width);
        int height = Mathf.FloorToInt(rect.height);
        float midY = rect.y + height * 0.5f;

        int packSize = Mathf.Max(1, _samplesCache.Length / width);
        Color waveColor = new Color(1f, 0.6f, 0.1f);

        for (int x = 0; x < width; x++)
        {
            int start = x * packSize;
            float max = 0f;

            for (int i = 0; i < packSize && start + i < _samplesCache.Length; i++)
                max = Mathf.Max(max, Mathf.Abs(_samplesCache[start + i]));

            float waveHeight = max * height * 0.5f;

            EditorGUI.DrawRect(
                new Rect(rect.x + x, midY - waveHeight, 1f, waveHeight * 2f),
                waveColor
            );
        }

        DrawPlayhead(rect, clip);
    }

    // ================= PLAYHEAD =================
    private static void DrawPlayhead(Rect rect, AudioClip clip)
    {
        if (!_isPlaying || clip.length <= 0f)
            return;

        float time = EditorAudioPreview.GetTime();
        if (time <= 0f) return;

        float x = rect.x + (time / clip.length) * rect.width;

        EditorGUI.DrawRect(
            new Rect(x, rect.y, 2f, rect.height),
            Color.red
        );
    }

    // ================= PLAY / STOP (TOGGLE) =================
    private static void DrawPlaybackControls(AudioClip clip)
    {
        bool actuallyPlaying = IsActuallyPlaying(clip);
        _isPlaying = actuallyPlaying;

        GUILayout.BeginHorizontal();

        string label = actuallyPlaying ? "⏹ Stop" : "▶ Play";

        if (GUILayout.Button(label, GUILayout.Width(100)))
        {
            if (actuallyPlaying)
            {
                StopPlayback();
            }
            else
            {
                StartPlayback(clip);
            }
        }

        GUILayout.EndHorizontal();
    }

    private static void StartPlayback(AudioClip clip)
    {
        EditorAudioPreview.Play(clip);
        _isPlaying = true;

        EditorApplication.update -= RepaintOwner;
        EditorApplication.update += RepaintOwner;
    }

    private static void StopPlayback()
    {
        EditorAudioPreview.Stop();
        _isPlaying = false;

        EditorApplication.update -= RepaintOwner;
        _ownerWindow?.Repaint();
    }

    // ================= STATE CHECK =================
    private static bool IsActuallyPlaying(AudioClip clip)
    {
        if (clip == null) return false;

        float t = EditorAudioPreview.GetTime();
        return t > 0f && t < clip.length;
    }

    // ================= REPAINT DRIVER =================
    private static void RepaintOwner()
    {
        if (_ownerWindow == null)
        {
            EditorApplication.update -= RepaintOwner;
            return;
        }

        if (_isPlaying && !IsActuallyPlaying(_audioSo.AudioClip))
        {
            _isPlaying = false;
            EditorApplication.update -= RepaintOwner;
            _ownerWindow.Repaint();
            return;
        }

        if (_isPlaying)
            _ownerWindow.Repaint();
    }

    // ================= CACHE =================
    private static void CacheSamples(AudioSO so)
    {
        if (_audioSo == so && _samplesCache != null)
            return;

        _audioSo = so;
        _samplesCache = new float[so.AudioClip.samples * so.AudioClip.channels];
        so.AudioClip.GetData(_samplesCache, 0);
    }
}
