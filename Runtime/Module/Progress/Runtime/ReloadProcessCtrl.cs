/*
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NIX.Module.UI
{
    public class ReloadProcessCtrl : BaseProgress
    {
        [Header("References")]
        [SerializeField] protected Slider _Fill;
        [SerializeField] protected TextMeshProUGUI _AmountTxt;

        protected int _MaxAmount;
        protected int _MinAmount = 0;
        protected int _CurrentAmount;

        protected bool _IsReload = false;

        public bool IsFull => _CurrentAmount >= _MaxAmount;

        public virtual bool Init(int minAmount, int maxAmount, int currentValue, float reloadTime)
        {
            this._MaxAmount = maxAmount;
            this._MinAmount = minAmount;
            this._CurrentAmount = currentValue;

            _MaxValue = reloadTime;
            _MinValue = 0;
            _CurrentValue = 0;

            Display();
            RefreshAmountInfo();
            return true;
        }

        public virtual bool TakeAmount(int amount)
        {
            if (_CurrentAmount - amount < _MinAmount) return false;
            _CurrentAmount -= amount;
            RefreshAmountInfo();

            if (!_IsReload)
            {
                _IsReload = true;
                DOVirtual.Float(_MinValue, _MaxValue, _MaxValue, (float val) =>
                {
                    SetCurrentValue(val);
                }).OnComplete(() =>
                {
                    SetCurrentValue(_MinValue);
                    Reload();
                });
            }
            return true;
        }

        public virtual void Reload()
        {
            _CurrentAmount = Math.Min(_CurrentAmount + 1, _MaxAmount);
            _IsReload = false;
            RefreshAmountInfo();
            if (!IsFull && !_IsReload)
            {
                _IsReload = true;
                DOVirtual.Float(_MinValue, _MaxValue, _MaxValue, (float val) =>
                {
                    SetCurrentValue(val);
                }).OnComplete(() =>
                {
                    SetCurrentValue(_MinValue);
                    Reload();
                });
            }
        }

        protected override void Display()
        {
            _Fill.value = _CurrentValue / _MaxValue;
        }

        protected virtual void RefreshAmountInfo()
        {
            _AmountTxt.text = _CurrentAmount.ToString();
        }

    }
}
*/
