#if USE_SPINE
using System.Linq;
using Spine.Unity;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasSkeletonFade : MonoBehaviour
{
    [Header("Fade Settings")]
    [Tooltip("Duration of the fade animation using PrimeTween")]
    public float FadeDuration = 0.5f;

    protected CanvasGroup _CanvasGroup;
    protected SkeletonGraphic[] _Skeletons;
    protected Renderer[] _Renderers;
    protected MaterialPropertyBlock[] _Mpbs;
    protected bool _Initialized = false;

    protected virtual void Awake()
    {
        InitializeIfNeeded();
    }

    protected virtual void InitializeIfNeeded()
    {
        if (_Initialized) return;

        _CanvasGroup = GetComponent<CanvasGroup>();
        _Skeletons = GetComponentsInChildren<SkeletonGraphic>(true);

        _Renderers = _Skeletons
            .Where(s => s != null)
            .Select(s => s.GetComponent<Renderer>())
            .ToArray();

        _Mpbs = _Renderers
            .Select(_ => new MaterialPropertyBlock())
            .ToArray();

        _Initialized = true;
    }

    public virtual void Reload()
    {
        _Initialized = false;
        InitializeIfNeeded();
    }

    public virtual void SetAlpha(float alpha)
    {
        InitializeIfNeeded();

        if (_CanvasGroup != null)
            _CanvasGroup.alpha = alpha;

        for (int i = 0; i < _Renderers.Length; i++)
        {
            var rend = _Renderers[i];
            if (rend == null) continue;

            var mpb = _Mpbs[i];
            rend.GetPropertyBlock(mpb);

            Color tint = mpb.GetColor("_Color");
            if (tint.Equals(default(Color)))
                tint = rend.sharedMaterial.color;

            tint.a = alpha;
            mpb.SetColor("_Color", tint);
            rend.SetPropertyBlock(mpb);
        }
    }

    public virtual void SetInteractable(bool interactable)
    {
        InitializeIfNeeded();

        if (_CanvasGroup != null)
        {
            _CanvasGroup.interactable = interactable;
            _CanvasGroup.blocksRaycasts = interactable;
        }
    }

    public virtual void SetBlockRaycast(bool shouldBlock)
    {
        InitializeIfNeeded();

        if (_CanvasGroup != null)
            _CanvasGroup.blocksRaycasts = shouldBlock;
    }
}

#endif