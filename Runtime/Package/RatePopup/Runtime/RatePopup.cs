using System.Collections;
using System.Collections.Generic;
using NIX.Module.Popup;
using UnityEngine;
using UnityEngine.UI;

namespace NIX.Packages
{
    public class RatePopup : BasePopup
    {
        public const string RATE_US_KEY = "RATE_US_KEY";

        [SerializeField] protected Button _RateBtn, _NoBtn;
        [SerializeField] protected List<ImageUnlockable> _Stars;
        [SerializeField] protected string _URL = "";
        [SerializeField] protected float _DelayShowStar = 0.1f;

        protected int _RateStar;

        protected virtual void Awake()
        {
            _RateBtn.onClick.AddListener(OnRate);
            _NoBtn?.onClick.AddListener(() => { Close(); });

            for (int i = 0; i < _Stars.Count; i++)
            {
                int index = i;
                _Stars[i].GetComponent<Button>().onClick.AddListener(() => { OnSelectStar(index); });
            }
        }

        protected virtual void OnEnable()
        {
            _Stars.ForEach(star => star.SetState(LockState.Unlock));

            StartCoroutine(ShowStars(_DelayShowStar));
        }

        protected virtual void OnDisable()
        {
            StopAllCoroutines();
        }

        protected virtual IEnumerator ShowStars(float delay)
        {
            int index = 0;
            while (index < _Stars.Count)
            {
                _Stars[index].SetState(LockState.Unlock);
                index++;
                yield return new WaitForSeconds(delay);
            }
        }

        protected virtual void OnRate()
        {
            if (_RateStar < 4)
            {
                Close();
            }
            else
            {
                Application.OpenURL(_URL);
                PlayerPrefs.SetInt(RATE_US_KEY, 0);
                PlayerPrefs.Save();
            }
        }

        protected virtual void OnSelectStar(int index)
        {
            _RateStar = index + 1;
            for (int i = 0; i < _Stars.Count; i++)
            {
                _Stars[i].SetState(!(i <= index) ? LockState.Lock : LockState.Unlock);
            }
        }
    }
}