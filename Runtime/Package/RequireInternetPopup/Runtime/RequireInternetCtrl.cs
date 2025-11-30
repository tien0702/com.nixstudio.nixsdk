using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace NIX.Packages
{
    public class RequireInternetCtrl : MonoBehaviour
    {
        public const string REQUIRE_INTERNET_KEY = "REQUIRE_INTERNET";

        [SerializeField] protected RequireInternetPopup _Popup;

        [Tooltip("Millisecond")] 
        [SerializeField] protected int _TimeFrequency = 7000;

        protected bool _IsShowing = false;

        public static bool REQUIRE_INTERNET
        {
            get => PlayerPrefs.HasKey(REQUIRE_INTERNET_KEY);
            set
            {
                if (value)
                {
                    PlayerPrefs.SetInt(REQUIRE_INTERNET_KEY, 1);
                    PlayerPrefs.Save();
                }
                else
                {
                    PlayerPrefs.DeleteKey(REQUIRE_INTERNET_KEY);
                }
            }
        }

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            _ = StartCheckingNetwork(_TimeFrequency);
        }

        private async Task StartCheckingNetwork(int timeFrequency)
        {
            while (true)
            {
                if (REQUIRE_INTERNET)
                {
                    bool hasInternet = await CheckInternet();
                    if (!hasInternet)
                    {
                        if (!_IsShowing)
                        {
                            _Popup.Open();
                            _IsShowing = true;
                        }
                    }
                    else
                    {
                        if (_IsShowing)
                        {
                            _Popup.Close();
                            _IsShowing = false;
                        }
                    }
                }

                await Task.Delay(timeFrequency);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private async void OnApplicationFocus(bool focus)
        {
            if (!focus || !_IsShowing) return;

            bool hasInternet = await CheckInternet();

            if (hasInternet)
            {
                _Popup.Close();
                _IsShowing = false;
            }
        }

        /// <summary>
        /// Returns true if actual internet is reachable (not just local).
        /// </summary>
        protected async Task<bool> CheckInternet(int timeout = 3000)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return false;

            using UnityWebRequest request = new UnityWebRequest("https://www.google.com");
            request.method = UnityWebRequest.kHttpVerbHEAD;
            request.timeout = Mathf.CeilToInt(timeout / 1000f);

            try
            {
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                return request.result != UnityWebRequest.Result.ConnectionError &&
                       request.result != UnityWebRequest.Result.ProtocolError;
            }
            catch
            {
                return false;
            }
        }
    }
}