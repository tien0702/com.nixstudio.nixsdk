using NIX.Core.Utils;
using UnityEngine;

namespace NIX.Packages
{
    public class SpringFeedback : BaseButtonFeedback
    {
        [SerializeField] private bool _loadDefaultScale = true;
        float _defaultScale = 1f;
        private SpringUtils.tDampedSpringMotionParams _springPram = new SpringUtils.tDampedSpringMotionParams();

        private float _frequency = 15f;
        private float _dampingRatio = 0.5f;
        private float _offsetSpringFeel = 0.15f;
        private float _currentScale;
        private float _velScale;

        protected override void Awake()
        {
            base.Awake();
            if (_loadDefaultScale) _defaultScale = transform.localScale.x;
        }

        private void Update()
        {
            SpringUtils.CalcDampedSpringMotionParams(ref _springPram, Time.deltaTime, _frequency, _dampingRatio);
            SpringUtils.UpdateDampedSpringMotion(ref _currentScale, ref _velScale, 0, in _springPram);
            Vector3 newLocalScale = new Vector3(_defaultScale - _currentScale, _defaultScale + _currentScale,
                transform.localScale.z);
            transform.localScale = newLocalScale;
        }

        protected override void OnEvent()
        {
            _currentScale = _offsetSpringFeel;
        }
    }
}