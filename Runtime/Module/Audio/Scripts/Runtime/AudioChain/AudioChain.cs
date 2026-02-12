using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GrowAGarden.Module.Audio
{
    public class AudioChain : AudioRunner
    {
        private AudioChainSO _so;
        private CancellationTokenSource _counterCts;
        private int _index = 0;

        public override AudioRunner Reset()
        {
            _so = null;
            _index = 0;
            return base.Reset();
        }

        public AudioChain SetSo(AudioChainSO so)
        {
            _so = so;
            return this;
        }

        protected override async UniTaskVoid PlayAsync()
        {
            _Cts = new CancellationTokenSource();
            var keyList = _MarkerDict.Keys.ToList();

            int loopCount = 0;

            while (_AudioBuild.Loops == -1 || loopCount < _AudioBuild.Loops)
            {
                _AudioBuild.AudioSource.Play();
                AudioManager.Instance.PlaySfx(GetAudioClip().AudioClip);
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

        private AudioSO GetAudioClip()
        {
            _counterCts = new CancellationTokenSource();
            _index = Mathf.Clamp(_index + 1, 0, _so.AudioList.Count - 1);
            return _so.AudioList[_index];
        }
    }
}