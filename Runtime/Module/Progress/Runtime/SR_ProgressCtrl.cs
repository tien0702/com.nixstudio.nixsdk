using NIX.Module;
using UnityEngine;

namespace NIX.Module.Progress
{
    public class SR_ProgressCtrl : BaseProgress
    {
        [SerializeField] private string _fillProp = "_FillAmount";
        private SpriteRenderer _sr;
        private MaterialPropertyBlock _mpb;

        protected override void Display()
        {
            _sr.GetPropertyBlock(_mpb);
            _mpb.SetFloat(_fillProp, CurrentValue / MaxValue);
            _sr.SetPropertyBlock(_mpb);
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (_sr == null) _sr = transform.Find("Fill").GetComponent<SpriteRenderer>();
            _mpb ??= new MaterialPropertyBlock();
            Display();
        }
#endif
    }
}