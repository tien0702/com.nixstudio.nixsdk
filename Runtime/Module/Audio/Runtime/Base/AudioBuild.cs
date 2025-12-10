using System;
using UnityEngine;

namespace GrowAGarden.Module.Audio
{
    public class AudioBuild
    {
        public AudioSO AudioSo;
        public AudioSource AudioSource;
        public Vector3 Position;
        public bool ChangePosition = false;
        public int Loops = 1;
        public float Volume = 1;

        public Action<AudioMarker> MarkerEvent;
        public Action CompleteEvent;
        public Action PauseEvent;
        public Action ResumeEvent;
        public Action StopEvent;
        public Action<int> LoopEvent;

        public void Reset()
        {

            if (AudioSource != null)
            {
                AudioSource.Stop();
                AudioSource.gameObject.SetActive(false);
                AudioSource = null;
            }
            AudioSo = null;
            AudioSource = null;
            ChangePosition = false;

            Position = Vector3.zero;
            Loops = 1;
            Volume = 1;

            MarkerEvent = null;
            CompleteEvent = null;
            PauseEvent = null;
            ResumeEvent = null;
            StopEvent = null;
        }
    }
}