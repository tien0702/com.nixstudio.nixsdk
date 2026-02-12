using UnityEngine;
using UnityEngine.UI;

namespace NIX.Core.Extend
{
    public static class ImageExtend
    {
        public static bool CopyBySpriteRenderer(
            this Image target,
            SpriteRenderer src,
            RectTransform uiParent,
            Canvas canvas,
            Camera cam = null,
            string nameIfCreate = null,
            bool reparentIfExisting = true)
        {
            if (src == null || src.sprite == null || uiParent == null || canvas == null)
            {
                Debug.LogWarning("[UIConvertUtils] Missing src/uiParent/canvas/sprite.");
                return false;
            }

            // --- 1) Local quad of the sprite in sprite space (relative to its pivot)
            var sp = src.sprite;
            Vector2 sizeLocal = sp.rect.size / sp.pixelsPerUnit; // sprite size in local units (no scale)
            Vector2 pivot01 = sp.pivot / sp.rect.size; // 0..1
            float left = -pivot01.x * sizeLocal.x;
            float right = (1f - pivot01.x) * sizeLocal.x;
            float bottom = -pivot01.y * sizeLocal.y;
            float top = (1f - pivot01.y) * sizeLocal.y;

            Vector3[] local = new Vector3[4];
            // BL, TL, TR, BR
            local[0] = new Vector3(left, bottom, 0f);
            local[1] = new Vector3(left, top, 0f);
            local[2] = new Vector3(right, top, 0f);
            local[3] = new Vector3(right, bottom, 0f);

            // --- 2) World → Screen
            if (cam == null) cam = Camera.main;
            Vector3[] world = new Vector3[4];
            Vector2[] screen = new Vector2[4];
            for (int i = 0; i < 4; i++)
            {
                world[i] = src.transform.TransformPoint(local[i]); // applies TRS of SpriteRenderer
                screen[i] = RectTransformUtility.WorldToScreenPoint(cam, world[i]);
            }

            // --- 3) Screen → Canvas local
            var canvasRT = (RectTransform)canvas.transform;
            var ui = new Vector2[4];
            Camera eventCam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            for (int i = 0; i < 4; i++)
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, screen[i], eventCam, out ui[i]);

            // --- 4) Derive oriented rect in canvas space
            Vector2 bl = ui[0], tl = ui[1], tr = ui[2], br = ui[3];
            Vector2 xAxis = br - bl; // width direction
            Vector2 yAxis = tl - bl; // height direction
            float width = xAxis.magnitude;
            float height = yAxis.magnitude;
            float angleDeg = Mathf.Atan2(xAxis.y, xAxis.x) * Mathf.Rad2Deg;
            Vector2 center = (bl + tr) * 0.5f;

            // --- 5) Create or reuse Image
            if (target == null)
            {
                var go = new GameObject(nameIfCreate ?? $"UI_{sp.name}", typeof(RectTransform), typeof(Image));
                go.transform.SetParent(uiParent, false);
                target = go.GetComponent<Image>();
            }
            else if (reparentIfExisting && target.transform.parent != uiParent)
            {
                target.transform.SetParent(uiParent, false);
            }

            // --- 6) Apply sprite + color, and fit transform
            target.sprite = sp;
            target.preserveAspect = false; // we set exact size
            // Optional: carry color from SpriteRenderer
            target.color = src.color;

            var rt = target.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = center;
            rt.sizeDelta = new Vector2(width, height);
            rt.localRotation = Quaternion.Euler(0f, 0f, angleDeg);
            rt.localScale = Vector3.one;

            return true;
        }

        public static bool CopyBySpriteRendererWithScale(
            this Image target,
            SpriteRenderer src,
            Canvas canvas,
            Camera cam = null,
            string nameIfCreate = null,
            bool reparentIfExisting = true,
            bool forceRebuildLayoutBeforeMeasure = false,
            bool uniformScale = false // true: dùng 1 scale cho cả X/Y (theo min để fit)
        )
        {
            if (src == null || src.sprite == null || canvas == null)
            {
                Debug.LogWarning("[UIConvertUtils] Missing src/uiParent/canvas/sprite.");
                return false;
            }

            var uiParent = target.transform.parent.GetComponent<RectTransform>();
            // --- 1) Build local quad in sprite space (relative to pivot)
            var sp = src.sprite;
            Vector2 sizeLocal = sp.rect.size / sp.pixelsPerUnit;
            Vector2 pivot01 = sp.pivot / sp.rect.size;
            float left = -pivot01.x * sizeLocal.x;
            float right = (1f - pivot01.x) * sizeLocal.x;
            float bottom = -pivot01.y * sizeLocal.y;
            float top = (1f - pivot01.y) * sizeLocal.y;

            Vector3[] local = new Vector3[4];
            // BL, TL, TR, BR (giữ thứ tự này để trục x/y ổn định)
            local[0] = new Vector3(left, bottom, 0f);
            local[1] = new Vector3(left, top, 0f);
            local[2] = new Vector3(right, top, 0f);
            local[3] = new Vector3(right, bottom, 0f);

            // --- 2) World -> Screen
            if (cam == null) cam = Camera.main;
            Vector3[] world = new Vector3[4];
            Vector2[] screen = new Vector2[4];
            for (int i = 0; i < 4; i++)
            {
                world[i] = src.transform.TransformPoint(local[i]);
                screen[i] = RectTransformUtility.WorldToScreenPoint(cam, world[i]);
            }

            // --- 3) Screen -> Canvas local
            var canvasRT = (RectTransform)canvas.transform;
            var ui = new Vector2[4];
            Camera eventCam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            for (int i = 0; i < 4; i++)
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, screen[i], eventCam, out ui[i]);

            // --- 4) Oriented rect in canvas space
            Vector2 bl = ui[0], tl = ui[1], tr = ui[2], br = ui[3];
            Vector2 xAxis = br - bl; // width direction
            Vector2 yAxis = tl - bl; // height direction
            float targetWidth = xAxis.magnitude; // desired width in canvas space
            float targetHeight = yAxis.magnitude; // desired height in canvas space
            float angleDeg = Mathf.Atan2(xAxis.y, xAxis.x) * Mathf.Rad2Deg;
            Vector2 center = (bl + tr) * 0.5f;

            // --- 5) Create or reuse Image (không đổi sizeDelta)
            if (target == null)
            {
                var go = new GameObject(nameIfCreate ?? $"UI_{sp.name}", typeof(RectTransform), typeof(Image));
                go.transform.SetParent(uiParent, false);
                target = go.GetComponent<Image>();
            }
            else if (reparentIfExisting && target.transform.parent != uiParent)
            {
                target.transform.SetParent(uiParent, false);
            }

            // --- 6) Apply sprite + color + transform (position/rotation). Giữ sizeDelta hiện có.
            target.sprite = sp;
            target.preserveAspect = false; // size/scale sẽ quyết định, không cần giữ tỉ lệ ở Image
            target.color = src.color;

            var rt = target.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = center;
            rt.localRotation = Quaternion.Euler(0f, 0f, angleDeg);

            // Quan trọng: KHÔNG đụng vào rt.sizeDelta ở đây.
            // Nếu bạn muốn base size là "native size" của sprite khi target chưa có layout, có thể mở dòng dưới:
            // if (rt.rect.size.x <= 0f || rt.rect.size.y <= 0f) { rt.sizeDelta = sp.rect.size / sp.pixelsPerUnit; }

            // Option: ép rebuild layout để đảm bảo rt.rect.size đã cập nhật trước khi đo
            if (forceRebuildLayoutBeforeMeasure)
            {
                // Force rebuild trên cây cha gần nhất có thể ảnh hưởng tới rt
                var rootForLayout = uiParent ?? rt;
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rootForLayout);
            }

            // Lấy "base" size hiện tại của RectTransform (đơn vị local, chưa tính scale)
            Vector2 baseSize = rt.rect.size;

            // Fallback: nếu baseSize bằng 0 (chưa layout), dùng native size của sprite để tránh chia 0
            if (baseSize.x <= 0f || baseSize.y <= 0f)
            {
                baseSize = sp.rect.size / sp.pixelsPerUnit;
                if (baseSize.x <= 0f) baseSize.x = 1f;
                if (baseSize.y <= 0f) baseSize.y = 1f;
            }

            // --- 7) TÍNH SCALE sau khi convert
            float scaleX = targetWidth / baseSize.x;
            float scaleY = targetHeight / baseSize.y;

            if (uniformScale)
            {
                // Dùng 1 scale để giữ tỉ lệ (fit theo chiều nhỏ hơn)
                float s = Mathf.Min(scaleX, scaleY);
                scaleX = scaleY = s;
            }

            rt.localScale = new Vector3(scaleX, scaleY, 1f);

            return true;
        }
    }
}