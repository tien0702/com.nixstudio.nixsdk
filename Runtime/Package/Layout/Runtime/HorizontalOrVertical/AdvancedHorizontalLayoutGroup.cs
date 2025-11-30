using UnityEngine;
using UnityEngine.UI;

namespace NIX.Packages
{
    /// <summary>
    /// Advanced horizontal layout:
    /// - Uses RectTransform.rect size by default (Preferred optional).
    /// - Positions only by default (ResizeChildren optional).
    /// - Balances spacing under X scale without changing pivot.
    /// - Honors Padding + Child Alignment.
    /// </summary>
    [ExecuteAlways]
    [AddComponentMenu("Layout/Advanced Horizontal Layout Group (RectSize)")]
    public class AdvancedHorizontalLayoutGroup : AdvancedLayoutGroupBase
    {
        protected override int PrimaryAxis => 0;
    }
}