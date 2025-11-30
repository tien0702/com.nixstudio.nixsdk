using UnityEngine;
using UnityEngine.UI;

namespace NIX.Packages
{
    /// <summary>
    /// Advanced vertical layout:
    /// - Uses RectTransform.rect size by default (Preferred optional).
    /// - Positions only by default (ResizeChildren optional).
    /// - Balances spacing under Y scale without changing pivot.
    /// - Honors Padding + Child Alignment.
    /// </summary>
    [ExecuteAlways]
    [AddComponentMenu("Layout/Advanced Vertical Layout Group (RectSize)")]
    public class AdvancedVerticalLayoutGroup : AdvancedLayoutGroupBase
    {
        protected override int PrimaryAxis => 1;
    }
}