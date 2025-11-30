using NIX.Core.UI;
using UnityEngine;
using UnityEngine.UI;

namespace NIX.Module.UI
{
    [RequireComponent(typeof(Button))]
    public class CloseUIVisualBtn : MonoBehaviour
    {
        [SerializeField] protected BaseUIVisual _UIVisual;

        protected Button _Btn;

        protected virtual void Awake()
        {
            _Btn ??= GetComponent<Button>();
            _Btn.onClick.AddListener(OnPressBtn);
        }

        protected virtual void OnPressBtn()
        {
            _UIVisual.Close();
        }
    }
}