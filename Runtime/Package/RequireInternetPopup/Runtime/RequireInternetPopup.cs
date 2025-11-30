using NIX.Module.Popup;
using UnityEngine;
using UnityEngine.UI;

namespace NIX.Packages
{
    public class RequireInternetPopup : BasePopup
    {
        [SerializeField] protected Button _ConnectBtn;

        protected virtual void Awake()
        {
            _ConnectBtn.onClick.AddListener(OpenNetworkSettings);
        }

        protected void OpenNetworkSettings()
        {
#if UNITY_ANDROID
            // Only on Android devices
            if (Application.platform == RuntimePlatform.Android)
            {
                // Get the current Android Activity from UnityPlayer
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                // Get SDK version to decide whether to use Panel API or fallback
                AndroidJavaClass versionClass = new AndroidJavaClass("android.os.Build$VERSION");
                int sdkInt = versionClass.GetStatic<int>("SDK_INT");

                string action;
                if (sdkInt >= 29)
                {
                    // Android 10+ → open Internet Connectivity Panel (Wi-Fi + Mobile Data)
                    action = "android.settings.panel.action.INTERNET_CONNECTIVITY";
                }
                else
                {
                    // Android <10 → fallback to Wireless Settings
                    action = "android.settings.WIRELESS_SETTINGS";
                }

                // Create Intent with the corresponding action
                AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", action);

                // If you want this Intent to start in a new task:
                intent.Call<AndroidJavaObject>("addFlags", new AndroidJavaClass("android.content.Intent")
                    .GetStatic<int>("FLAG_ACTIVITY_NEW_TASK"));

                // Start the activity
                currentActivity.Call("startActivity", intent);
            }
            else
            {
                Debug.Log("This feature is only available on Android devices.");
            }
            

            // Only on iOS devices
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                // Open the Wi-Fi settings page on iOS
                // Note: "App-Prefs:root=WIFI" works on iOS 10-12, but may be restricted on newer versions.
                // If it doesn't work, it will simply open the general Settings.
                Application.OpenURL("App-Prefs:root=WIFI");
            }
            else
            {
                Debug.Log("This feature is only available on iOS devices.");
            }
#else
            // On other platforms (e.g., Editor, Windows, macOS), just log a message
            Debug.Log("Network settings cannot be opened on this platform.");
#endif
        }
    }
}
    
