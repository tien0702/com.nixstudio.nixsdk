////////////////////////////////////////////////////////////////////////////////
//
// @author Benoît Freslon @benoitfreslon
// https://github.com/BenoitFreslon/Vibration
// https://benoitfreslon.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

namespace NIX.Core
{
    public static class Vibration
    {
#if UNITY_IOS
        [DllImport("__Internal")] private static extern bool _HasVibrator();
        [DllImport("__Internal")] private static extern void _Vibrate();
        [DllImport("__Internal")] private static extern void _VibratePop();
        [DllImport("__Internal")] private static extern void _VibratePeek();
        [DllImport("__Internal")] private static extern void _VibrateNope();
        [DllImport("__Internal")] private static extern void _impactOccurred(string style);
        [DllImport("__Internal")] private static extern void _notificationOccurred(string style);
        [DllImport("__Internal")] private static extern void _selectionChanged();
#endif

#if UNITY_ANDROID
        private static AndroidJavaClass _unityPlayer;
        private static AndroidJavaObject _currentActivity;
        private static AndroidJavaObject _vibrator;
        private static AndroidJavaObject _context;
        private static AndroidJavaClass _vibrationEffect;
#endif

        private static bool _initialized = false;

        public static void Init()
        {
            if (_initialized) return;

#if UNITY_ANDROID
            if (Application.isMobilePlatform)
            {
                _unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                _currentActivity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                _vibrator = _currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                _context = _currentActivity.Call<AndroidJavaObject>("getApplicationContext");

                if (AndroidVersion >= 26)
                {
                    _vibrationEffect = new AndroidJavaClass("android.os.VibrationEffect");
                }
            }
#endif
            _initialized = true;
        }

        public static void Vibrate(ImpactFeedbackStyle style)
        {
            if (!Application.isMobilePlatform) return;

            if (!_initialized) Init();

#if UNITY_IOS
            _impactOccurred(style.ToString());
#elif UNITY_ANDROID
            VibrateAndroid(GetMillisecondsByImpactFeedback(style));
#endif
        }

        private static long GetMillisecondsByImpactFeedback(ImpactFeedbackStyle style)
        {
            return style switch
            {
                ImpactFeedbackStyle.Heavy => 100,
                ImpactFeedbackStyle.Medium => 70,
                ImpactFeedbackStyle.Light => 50,
                ImpactFeedbackStyle.Rigid => 30,
                ImpactFeedbackStyle.Soft => 10,
                _ => 100
            };
        }

        public static void VibrateIOS(NotificationFeedbackStyle style)
        {
#if UNITY_IOS
            _notificationOccurred(style.ToString());
#endif
        }

        public static void VibrateIOS_SelectionChanged()
        {
#if UNITY_IOS
            _selectionChanged();
#endif
        }

        public static void VibratePop()
        {
            if (!Application.isMobilePlatform) return;
            if (!_initialized) Init();

#if UNITY_IOS
            _VibratePop();
#elif UNITY_ANDROID
            VibrateAndroid(50);
#endif
        }

        public static void VibratePeek()
        {
            if (!Application.isMobilePlatform) return;
            if (!_initialized) Init();

#if UNITY_IOS
            _VibratePeek();
#elif UNITY_ANDROID
            VibrateAndroid(100);
#endif
        }

        public static void VibrateNope()
        {
            if (!Application.isMobilePlatform) return;
            if (!_initialized) Init();

#if UNITY_IOS
            _VibrateNope();
#elif UNITY_ANDROID
            long[] pattern = { 0, 50, 50, 50 };
            VibrateAndroid(pattern, -1);
#endif
        }

#if UNITY_ANDROID
        public static void VibrateAndroid(long milliseconds)
        {
            if (!Application.isMobilePlatform || !_initialized) return;

            if (_vibrator == null) Init();
            if (_vibrator == null) return;

            if (AndroidVersion >= 26 && _vibrationEffect != null)
            {
                AndroidJavaObject createOneShot =
                    _vibrationEffect.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, -1);
                _vibrator.Call("vibrate", createOneShot);
            }
            else
            {
                _vibrator.Call("vibrate", milliseconds);
            }
        }

        /// <summary>
        /// Start a continuous vibration loop.
        /// On Android, uses a repeating pattern. On iOS, falls back to a single vibration.
        /// </summary>
        /// <param name="millisecondsPerPulse">
        /// Duration of each vibration pulse in milliseconds (default 1000 ms).
        /// </param>
        public static void VibrateLoop(long millisecondsPerPulse = 1000)
        {
            if (!Application.isMobilePlatform) return;
            if (!_initialized) Init();

#if UNITY_ANDROID
            // Pattern: [wait 0 ms, vibrate for millisecondsPerPulse]
            // Repeat index = 0 → loop forever
            long[] pattern = { 0, millisecondsPerPulse };
            VibrateAndroid(pattern, 0);
#elif UNITY_IOS
            // iOS does not support continuous vibration via Handheld
            Handheld.Vibrate();
#endif
        }

        /// <summary>
        /// Stops any ongoing vibration.
        /// Only effective on Android; iOS has no cancellation API.
        /// </summary>
        public static void StopVibration()
        {
            if (!Application.isMobilePlatform) return;
#if UNITY_ANDROID
            CancelAndroid();
#elif UNITY_IOS
            // No-op: iOS vibrations cannot be programmatically stopped
#endif
        }

        public static void VibrateAndroid(long[] pattern, int repeat)
        {
            if (!Application.isMobilePlatform || !_initialized) return;

            if (_vibrator == null) Init();
            if (_vibrator == null) return;

            if (AndroidVersion >= 26 && _vibrationEffect != null)
            {
                AndroidJavaObject createWaveform =
                    _vibrationEffect.CallStatic<AndroidJavaObject>("createWaveform", pattern, repeat);
                _vibrator.Call("vibrate", createWaveform);
            }
            else
            {
                _vibrator.Call("vibrate", pattern, repeat);
            }
        }
#endif

        public static void CancelAndroid()
        {
#if UNITY_ANDROID
            if (!Application.isMobilePlatform) return;
            if (!_initialized) Init();
            _vibrator?.Call("cancel");
#endif
        }

        public static bool HasVibrator()
        {
            if (!Application.isMobilePlatform) return false;

#if UNITY_ANDROID
            if (!_initialized) Init();

            AndroidJavaClass contextClass = new AndroidJavaClass("android.content.Context");
            string vibratorService = contextClass.GetStatic<string>("VIBRATOR_SERVICE");

            AndroidJavaObject systemService = _context.Call<AndroidJavaObject>("getSystemService", vibratorService);
            return systemService?.Call<bool>("hasVibrator") ?? false;
#elif UNITY_IOS
            return _HasVibrator();
#else
            return false;
#endif
        }

        public static void Vibrate()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (Application.isMobilePlatform)
            {
                if (!_initialized) Init();
                Handheld.Vibrate();
            }
#endif
        }

        public static int AndroidVersion
        {
            get
            {
#if UNITY_ANDROID
                if (Application.platform == RuntimePlatform.Android)
                {
                    string androidVersion = SystemInfo.operatingSystem;
                    int sdkPos = androidVersion.IndexOf("API-");
                    if (sdkPos != -1 && sdkPos + 5 <= androidVersion.Length)
                    {
                        if (int.TryParse(androidVersion.Substring(sdkPos + 4, 2), out int version))
                        {
                            return version;
                        }
                    }
                }
#endif
                return 0;
            }
        }
    }

    public enum ImpactFeedbackStyle
    {
        Heavy,
        Medium,
        Light,
        Rigid,
        Soft
    }

    public enum NotificationFeedbackStyle
    {
        Error,
        Success,
        Warning
    }
}