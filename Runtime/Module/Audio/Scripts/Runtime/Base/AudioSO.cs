using System.Collections.Generic;
using UnityEngine;

namespace GrowAGarden.Module.Audio
{
    [CreateAssetMenu(fileName = "Audio", menuName = "SO/AudioSO")]
    public class AudioSO : ScriptableObject
    {
        public string Description;
        public AudioClip AudioClip;
        public List<AudioMarker> Markers;

        public void AddMarker(AudioMarker audioMarker)
        {
            Markers?.Add(audioMarker);
        }

        public void RemoveMarker(AudioMarker audioMarker)
        {
            Markers?.Remove(audioMarker);
        }
    }
}