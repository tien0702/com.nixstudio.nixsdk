using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NIX.Packages
{
    [ExecuteAlways]
    public class ResponsiveLayoutGroup : LayoutGroup
    {
        [SerializeField] protected float _SpacingX = 10f;
        [SerializeField] protected float _SpacingY = 10f;
        [SerializeField] protected int _MaxColumnCount = 2;

        protected override void OnEnable()
        {
            base.OnEnable();
            CalculateLayoutInputHorizontal();
            CalculateLayoutInputVertical();
        }

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            ArrangeResponsiveChildren();
        }

        public override void CalculateLayoutInputVertical()
        {
            ArrangeResponsiveChildren();
        }

        public override void SetLayoutHorizontal()
        {
        }

        public override void SetLayoutVertical()
        {
        }

        protected virtual void ArrangeResponsiveChildren()
        {
            float containerWidth = rectTransform.rect.width - padding.left - padding.right;

            float maxChildWidth = 0f;
            foreach (var child in rectChildren)
            {
                float width = LayoutUtility.GetPreferredSize(child, 0);
                maxChildWidth = Mathf.Max(maxChildWidth, width);
            }

            int columnCount = Mathf.Max(1,
                Mathf.Min(_MaxColumnCount,
                    Mathf.FloorToInt((containerWidth + _SpacingX) / (maxChildWidth + _SpacingX))));

            List<List<RectTransform>> lines = new List<List<RectTransform>>();
            List<float> lineHeights = new List<float>();
            List<float> lineWidths = new List<float>();

            List<RectTransform> currentLine = new List<RectTransform>();
            float currentLineWidth = 0f;
            float rowMaxHeight = 0f;
            int colIndex = 0;

            foreach (var child in rectChildren)
            {
                float width = LayoutUtility.GetPreferredSize(child, 0);
                float height = LayoutUtility.GetPreferredSize(child, 1);

                if (colIndex >= columnCount)
                {
                    lines.Add(currentLine);
                    lineHeights.Add(rowMaxHeight);
                    lineWidths.Add(currentLineWidth - _SpacingX);

                    currentLine = new List<RectTransform>();
                    colIndex = 0;
                    currentLineWidth = 0f;
                    rowMaxHeight = 0f;
                }

                currentLine.Add(child);
                currentLineWidth += width + _SpacingX;
                rowMaxHeight = Mathf.Max(rowMaxHeight, height);
                colIndex++;
            }

            if (currentLine.Count > 0)
            {
                lines.Add(currentLine);
                lineHeights.Add(rowMaxHeight);
                lineWidths.Add(currentLineWidth - _SpacingX);
            }

            float y = GetStartOffsetY(lineHeights);

            for (int row = 0; row < lines.Count; row++)
            {
                List<RectTransform> line = lines[row];
                float rowHeight = lineHeights[row];
                float rowWidth = lineWidths[row];
                float xStart = GetStartOffsetX(rowWidth);

                float xCursor = xStart;
                foreach (var child in line)
                {
                    float width = LayoutUtility.GetPreferredSize(child, 0);
                    float height = LayoutUtility.GetPreferredSize(child, 1);

                    float yOffset = GetVerticalAlignOffset(rowHeight, height);

                    SetChildAlongAxis(child, 0, xCursor, width);
                    SetChildAlongAxis(child, 1, y + yOffset, height);

                    xCursor += width + _SpacingX;
                }

                y += rowHeight + _SpacingY;
            }

            float totalHeight = y - _SpacingY + padding.bottom;
            SetLayoutInputForAxis(totalHeight, totalHeight, -1, 1);
        }

        protected virtual float GetStartOffsetX(float contentWidth)
        {
            float containerWidth = rectTransform.rect.width - padding.left - padding.right;
            float offset = padding.left;

            switch (childAlignment)
            {
                case TextAnchor.UpperLeft:
                case TextAnchor.MiddleLeft:
                case TextAnchor.LowerLeft:
                    offset = padding.left;
                    break;
                case TextAnchor.UpperCenter:
                case TextAnchor.MiddleCenter:
                case TextAnchor.LowerCenter:
                    offset = padding.left + (containerWidth - contentWidth) / 2f;
                    break;
                case TextAnchor.UpperRight:
                case TextAnchor.MiddleRight:
                case TextAnchor.LowerRight:
                    offset = rectTransform.rect.width - padding.right - contentWidth;
                    break;
            }

            return offset;
        }

        protected virtual float GetStartOffsetY(List<float> rowHeights)
        {
            float totalHeight = 0f;
            foreach (var h in rowHeights)
                totalHeight += h + _SpacingY;

            totalHeight -= _SpacingY;

            float containerHeight = rectTransform.rect.height - padding.top - padding.bottom;
            float offset = padding.top;

            switch (childAlignment)
            {
                case TextAnchor.UpperLeft:
                case TextAnchor.UpperCenter:
                case TextAnchor.UpperRight:
                    offset = padding.top;
                    break;
                case TextAnchor.MiddleLeft:
                case TextAnchor.MiddleCenter:
                case TextAnchor.MiddleRight:
                    offset = padding.top + (containerHeight - totalHeight) / 2f;
                    break;
                case TextAnchor.LowerLeft:
                case TextAnchor.LowerCenter:
                case TextAnchor.LowerRight:
                    offset = rectTransform.rect.height - padding.bottom - totalHeight;
                    break;
            }

            return offset;
        }

        protected virtual float GetVerticalAlignOffset(float rowHeight, float elementHeight)
        {
            switch (childAlignment)
            {
                case TextAnchor.UpperLeft:
                case TextAnchor.UpperCenter:
                case TextAnchor.UpperRight:
                    return 0f;
                case TextAnchor.MiddleLeft:
                case TextAnchor.MiddleCenter:
                case TextAnchor.MiddleRight:
                    return (rowHeight - elementHeight) / 2f;
                case TextAnchor.LowerLeft:
                case TextAnchor.LowerCenter:
                case TextAnchor.LowerRight:
                    return rowHeight - elementHeight;
            }

            return 0f;
        }
    }
}