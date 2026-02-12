using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using NIX.Core.DesignPatterns;
using UnityEngine;

namespace GrowAGarden.Module.Audio
{
    public class AudioRunner
    {
        protected static SimplePool<AudioRunner> POOL = new();

        public static AudioRunner Create() => POOL.Get();

        public static void Release(AudioRunner runner) => POOL.Release(runner);

        protected AudioBuild _AudioBuild = new();
        public bool IsPlaying { get; protected set; }
        public bool IsPause { get; protected set; }
        public float Duration { get; protected set; }
        public float ElapsedTime { get; protected set; }
        public bool IsRetain { get; protected set; }

        protected CancellationTokenSource _Cts;
        protected Dictionary<AudioMarker, bool> _MarkerDict = new();

        public virtual AudioRunner Play()
        {
            if (IsPlaying)
            {
                Debug.LogWarning("[Play] AudioRunner is already playing");
                return this;
            }

            IsPlaying = true;
            if (_AudioBuild.AudioSource == null) SetAudioSource(AudioManager.Instance.GetSfxSource());

            if (_AudioBuild.ChangePosition)
            {
                _AudioBuild.AudioSource.transform.position = _AudioBuild.Position;
            }

            _AudioBuild.AudioSource.clip = _AudioBuild.AudioSo.AudioClip;
            _AudioBuild.AudioSource.volume = _AudioBuild.Volume;
            PlayAsync().Forget();
            return this;
        }

        protected virtual async UniTaskVoid PlayAsync()
        {
            _Cts = new CancellationTokenSource();
            var keyList = _MarkerDict.Keys.ToList();

            int loopCount = 0;

            while (_AudioBuild.Loops == -1 || loopCount < _AudioBuild.Loops)
            {
                _AudioBuild.AudioSource.Play();
                _AudioBuild.LoopEvent?.Invoke(loopCount);
                while (ElapsedTime <= Duration)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, _Cts.Token);

                    if (IsPause) continue;
                    ElapsedTime += Time.deltaTime;

                    foreach (var marker in keyList)
                    {
                        if (ElapsedTime >= marker.Time && !_MarkerDict[marker])
                        {
                            _AudioBuild.MarkerEvent?.Invoke(marker);
                            _MarkerDict[marker] = true;
                        }
                    }
                }

                loopCount++;
                ElapsedTime = 0f;
            }

            _AudioBuild.CompleteEvent?.Invoke();
            Reset();
        }

        public virtual AudioRunner Replay()
        {
            if (IsPlaying)
            {
                _Cts.Cancel();
                IsPlaying = false;
            }

            return Play();
        }

        public virtual AudioRunner Pause()
        {
            if (!IsPlaying)
            {
                Debug.LogWarning("[Pause] AudioRunner is not playing");
                return this;
            }

            if (IsPause)
            {
                Debug.LogWarning("[Pause] AudioRunner is already paused");
                return this;
            }

            IsPause = true;
            _AudioBuild.AudioSource.Pause();
            _AudioBuild.PauseEvent?.Invoke();
            return this;
        }

        public virtual AudioRunner Resume()
        {
            if (!IsPlaying)
            {
                Debug.LogWarning("[Resume] AudioRunner is not playing");
                return this;
            }

            if (!IsPause)
            {
                Debug.LogWarning("[Resume] AudioRunner is not paused");
                return this;
            }

            IsPause = false;
            _AudioBuild.AudioSource.UnPause();
            _AudioBuild.ResumeEvent?.Invoke();
            return this;
        }

        public virtual AudioRunner Stop()
        {
            if (!IsPlaying)
            {
                Debug.LogWarning("[Stop] AudioRunner is not playing");
                return this;
            }

            _AudioBuild.StopEvent?.Invoke();
            _AudioBuild.AudioSource.Stop();
            Reset();
            return this;
        }

        public virtual AudioRunner Reset()
        {
            IsPlaying = false;
            IsPause = false;
            Duration = 0f;
            ElapsedTime = 0f;
            _Cts?.Cancel();
            if (!IsRetain)
            {
                _MarkerDict.Clear();
                _AudioBuild.Reset();
                AudioRunner.Release(this);
            }

            return this;
        }

        public virtual AudioRunner Retain()
        {
            IsRetain = true;
            return this;
        }

        public virtual AudioRunner UnRetain(bool immediate = false)
        {
            IsRetain = false;
            if (immediate && !IsPlaying) Reset();
            return this;
        }

        #region Builder

        public virtual AudioRunner SetAudioSo(AudioSO audioSo)
        {
            _AudioBuild.AudioSo = audioSo;
            Duration = audioSo.AudioClip.length;
            foreach (var marker in audioSo.Markers)
            {
                _MarkerDict.Add(marker, false);
            }

            return this;
        }

        public virtual AudioRunner SetAudioSource(AudioSource source)
        {
            _AudioBuild.AudioSource = source;
            return this;
        }

        public virtual AudioRunner SetPosition(Vector3 position)
        {
            _AudioBuild.Position = position;
            _AudioBuild.ChangePosition = true;
            return this;
        }

        public virtual AudioRunner SetLoops(int loops)
        {
            _AudioBuild.Loops = loops;
            return this;
        }

        public virtual AudioRunner SetVolume(float volume)
        {
            _AudioBuild.Volume = volume;
            return this;
        }

        public virtual AudioRunner OnMarkerEvent(Action<AudioMarker> callback)
        {
            _AudioBuild.MarkerEvent += callback;
            return this;
        }

        public virtual AudioRunner OnCompleteEvent(Action callback)
        {
            _AudioBuild.CompleteEvent += callback;
            return this;
        }

        public virtual AudioRunner OnPauseEvent(Action callback)
        {
            _AudioBuild.PauseEvent += callback;
            return this;
        }

        public virtual AudioRunner OnResumeEvent(Action callback)
        {
            _AudioBuild.ResumeEvent += callback;
            return this;
        }

        public virtual AudioRunner OnStopEvent(Action callback)
        {
            _AudioBuild.StopEvent += callback;
            return this;
        }

        public virtual AudioRunner OnLoopEvent(Action<int> callback)
        {
            _AudioBuild.LoopEvent += callback;
            return this;
        }

        #endregion
    }
}