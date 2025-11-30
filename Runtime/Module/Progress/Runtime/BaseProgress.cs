using NIX.Core;
using UnityEngine;

namespace NIX.Module.Progress
{
    public abstract class BaseProgress : BaseMono
    {
        [SerializeField] protected float _MaxValue = 1f;
        [SerializeField] protected float _MinValue = 0f;
        [SerializeField] protected float _CurrentValue = 1f;

        public float MaxValue => _MaxValue;
        public float MinValue => _MinValue;
        public float CurrentValue => _CurrentValue;

        public virtual bool Init(float min, float max, float current)
        {
            _MinValue = min;
            _MaxValue = max;
            _CurrentValue = current;
            Display();

            return true;
        }

        public virtual void SetCurrentValue(float currentValue)
        {
            _CurrentValue = Mathf.Clamp(currentValue, _MinValue, _MaxValue);
            Display();
        }

        protected abstract void Display();

    }
}
