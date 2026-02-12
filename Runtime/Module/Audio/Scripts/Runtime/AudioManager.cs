using NIX.Core.DesignPatterns;
using UnityEngine;
using UnityEngine.Audio;

namespace GrowAGarden.Module.Audio
{
    public class AudioManager : SingletonBehaviour<AudioManager>
    {
        public const string KEY_BGM = "BGM";
        public const string KEY_SFX = "SFX";

        [SerializeField] protected Pooler _SfxSourcePool;
        [SerializeField] private AudioMixer _audioMixer;

        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioSource _sfxSource;

        private SimplePool<AudioRunner> _configBuilderPool;

        protected override void Awake()
        {
            _configBuilderPool = new();
            _configBuilderPool.Fill(10);
            base.Awake();
        }

        public AudioSource GetSfxSource() => _SfxSourcePool.GetObject<AudioSource>();

        public AudioRunner CreateAudioRunner() => AudioRunner.Create().SetVolume(GetSfxVolume());

        public AudioChain CreateAudioChain()
        {
            return null;
        }

        public void SetMusicVolume(float volume)
        {
            PlayerPrefs.SetFloat(KEY_BGM, volume);
            PlayerPrefs.Save();
            _audioMixer.SetFloat(KEY_BGM, Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1)) * 20);
        }

        public void SetSfxVolume(float volume)
        {
            PlayerPrefs.SetFloat(KEY_SFX, volume);
            PlayerPrefs.Save();
            _audioMixer.SetFloat(KEY_SFX, Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1)) * 20);
        }

        public float GetSfxVolume() => PlayerPrefs.GetFloat(KEY_SFX, 1f);

        public float GetMusicVolume() => PlayerPrefs.GetFloat(KEY_BGM, 1f);

        public AudioRunner PlaySfx(AudioSO so, Vector3 position)
        {
            return CreateAudioRunner().SetAudioSo(so).SetPosition(position).Play();
        }

        public void PlaySfx(AudioClip clip)
        {
            _sfxSource.PlayOneShot(clip);
        }
    }
}