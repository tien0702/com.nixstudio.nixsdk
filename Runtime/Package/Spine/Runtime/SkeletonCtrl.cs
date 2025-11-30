/*using System;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
using AnimationState = Spine.AnimationState;

namespace NIX.Module.SpineSkeleton
{
    /// <summary>
    /// Spine controller that works with both SkeletonAnimation (mesh) and SkeletonGraphic (UGUI).
    /// </summary>
    public class SkeletonCtrl : MonoBehaviour
    {
        public event Action onSkinChanged;

        // Assign either SkeletonAnimation or SkeletonGraphic here (optional).
        [SerializeField] protected Component _Target; // SkeletonAnimation or SkeletonGraphic
        [SerializeField] [SpineAnimation] protected List<string> _AnimSeq;

        // Cached interface refs (work for both Animation & Graphic).
        private ISkeletonComponent _skeletonComp;
        private IAnimationStateComponent _stateComp;

        // Optional concrete caches (for convenience / checks).
        private SkeletonAnimation _animation;
        private SkeletonGraphic _graphic;

        // Runtime skins list
        protected List<string> _Skins = new();

        #region Component Accessors

        public SkeletonAnimation Model => _animation ??= GetComponent<SkeletonAnimation>();

        /// <summary>Spine Skeleton (works for both SkeletonAnimation & SkeletonGraphic).</summary>
        protected Skeleton Skeleton => (_skeletonComp ??= ResolveSkeletonComp())?.Skeleton;

        /// <summary>Spine AnimationState (works for both SkeletonAnimation & SkeletonGraphic).</summary>
        protected AnimationState State => (_stateComp ??= ResolveStateComp())?.AnimationState;

        protected SkeletonAnimation AnimationRenderer =>
            _animation ??= (_Target as SkeletonAnimation) ?? GetComponent<SkeletonAnimation>();

        protected SkeletonGraphic GraphicRenderer =>
            _graphic ??= (_Target as SkeletonGraphic) ?? GetComponent<SkeletonGraphic>();

        private ISkeletonComponent ResolveSkeletonComp()
        {
            if (_skeletonComp != null) return _skeletonComp;
            if (_Target is ISkeletonComponent sc) return sc;
            return GetComponent<ISkeletonComponent>();
        }

        private IAnimationStateComponent ResolveStateComp()
        {
            if (_stateComp != null) return _stateComp;
            if (_Target is IAnimationStateComponent asc) return asc;
            return GetComponent<IAnimationStateComponent>();
        }

        #endregion

        private void Reset()
        {
            // Try to auto-assign a suitable target on add.
            var anim = GetComponent<SkeletonAnimation>();
            if (anim != null)
            {
                _Target = anim;
                return;
            }

            var graphic = GetComponent<SkeletonGraphic>();
            if (graphic != null)
            {
                _Target = graphic;
            }
        }

        private void OnEnable()
        {
            if (_AnimSeq is { Count: > 0 })
            {
                PlayAnims(_AnimSeq, true);
            }
        }

        #region Skin API

        public virtual void SetSkin(string skin)
        {
            _Skins = new() { skin };
            ChangeSkins(_Skins);
        }

        public virtual void SetSkins(List<string> skins)
        {
            _Skins = skins ?? new List<string>();
            ChangeSkins(_Skins);
        }

        public virtual void AddSkins(List<string> skins)
        {
            if (skins != null && skins.Count > 0) _Skins.AddRange(skins);
            ChangeSkins(_Skins);
        }

        public virtual void AddSkin(string skin)
        {
            if (!string.IsNullOrEmpty(skin)) _Skins.Add(skin);
            ChangeSkins(_Skins);
        }

        public virtual bool ContainsSkin(string skin)
        {
            var skeleton = Skeleton;
            return skeleton != null && skeleton.Data.FindSkin(skin) != null;
        }

        public virtual bool ContainsAnim(string animName)
        {
            var skeleton = Skeleton;
            return skeleton != null && skeleton.Data.FindAnimation(animName) != null;
        }

        public virtual void RemoveSkins(List<string> skins)
        {
            if (skins != null && skins.Count > 0)
                _Skins.RemoveAll(skins.Contains);
            ChangeSkins(_Skins);
        }

        public virtual void RemoveSkin(string skin)
        {
            _Skins.Remove(skin);
            ChangeSkins(_Skins);
        }

        public virtual void ReplaceSkin(string oldSkin, string newSkin)
        {
            _Skins.Remove(oldSkin);
            if (!string.IsNullOrEmpty(newSkin)) _Skins.Add(newSkin);
            ChangeSkins(_Skins);
        }

        public virtual void ClearSkins()
        {
            _Skins.Clear();
            ChangeSkins(_Skins);
        }

        public List<string> GetSkins() => _Skins;

        /// <summary>
        /// Apply combined skins to the skeleton (Animation or Graphic).
        /// </summary>
        protected virtual void ChangeSkins(List<string> skins)
        {
            var skeleton = Skeleton;
            var state = State;

            if (skeleton == null || state == null)
            {
                Debug.LogWarning("[SkeletonCtrl] Missing Skeleton / State component.");
                return;
            }

            // Build combined skin or clear skin
            if (skins == null || skins.Count == 0)
            {
                skeleton.SetSkin((Skin)null);
            }
            else
            {
                var combined = new Skin("combined");
                foreach (var s in skins)
                {
                    if (string.IsNullOrEmpty(s)) continue;
                    var found = skeleton.Data.FindSkin(s);
                    if (found != null) combined.AddSkin(found);
                    else Debug.LogWarning($"[SkeletonCtrl] Skin '{s}' not found in SkeletonData.");
                }

                skeleton.SetSkin(combined);
            }

            // Reset slots to setup so attachments from skin take effect
            skeleton.SetSlotsToSetupPose();

            // Re-apply current state immediately
            state.Apply(skeleton);
            skeleton.UpdateWorldTransform(Skeleton.Physics.None);

            // For UI, mesh will rebuild next frame. (No public SetAllDirty from here)
            onSkinChanged?.Invoke();
        }

        #endregion

        #region Play APIs

        public virtual TrackEntry PlayAnim(string animName, bool loop = false, Action<EventData> onEvent = null)
        {
            var state = State;
            if (state == null)
            {
                Debug.LogWarning("[PlayAnim] No AnimationState.");
                return null;
            }

            var track = state.SetAnimation(0, animName, loop);
            if (track != null && onEvent != null)
                track.Event += (_, ev) => onEvent(ev.Data);
            return track;
        }

        public virtual TrackEntry PlayAnimAt(string animName, float duration, bool loop = false,
            Action<EventData> onEvent = null)
        {
            var state = State;
            if (state == null)
            {
                Debug.LogWarning("[PlayAnim] No AnimationState.");
                return null;
            }

            var track = state.SetAnimation(0, animName, loop);
            if (track != null && onEvent != null)
                track.Event += (_, ev) => onEvent(ev.Data);
            if (track != null) track.TrackTime = duration;
            return track;
        }

        public virtual TrackEntry CleanAndPlay(string animName, bool loop = false, Action<EventData> onEvent = null)
        {
            var skeleton = Skeleton;
            var state = State;
            if (Model != null) Model.enabled = true;
            if (GraphicRenderer != null) GraphicRenderer.enabled = true;
            if (skeleton == null || state == null)
            {
                Debug.LogWarning("[CleanAndPlay] Missing Skeleton / State.");
                return null;
            }

            // 1) Clear all tracks & flush
            state.ClearTracks();
            state.Update(0f);

            // 2) Reset to setup pose
            skeleton.SetToSetupPose();
            skeleton.UpdateCache();

            // 3) Play new animation with no mixing
            var entry = state.SetAnimation(0, animName, loop);
            if (entry != null)
            {
                entry.MixDuration = 0f;
                entry.HoldPrevious = false;
                entry.MixBlend = MixBlend.Replace;
                entry.TrackTime = 0f;
                if (onEvent != null)
                    entry.Event += (_, ev) => onEvent(ev.Data);
            }

            state.Data.DefaultMix = 0f;

            // 4) Apply immediately to avoid one-frame flicker
            state.Apply(skeleton);
            skeleton.UpdateWorldTransform(Skeleton.Physics.None);
            return entry;
        }

        public virtual TrackEntry PlayOnce(string animName, bool disableAtEnd = true, Action<EventData> onEvent = null)
        {
            Model.enabled = true;
            var entry = CleanAndPlay(animName, false, onEvent);
            entry.Complete += t => Model.enabled = false;
            return entry;
        }

        // -------- PlayRepeat Logic --------

        private TrackEntry _repeatEntry;
        private bool _isRepeating;
        private Action _onLoopStart;
        private Action _onLoopEnd;

        /// <summary>
        /// Loop an animation and invoke callbacks on each loop.
        /// </summary>
        public virtual void PlayRepeat(string animName, Action onRepeat = null, Action onEndRepeat = null)
        {
            var state = State;
            if (state == null || string.IsNullOrEmpty(animName))
            {
                Debug.LogWarning("[PlayRepeat] Invalid state or animation name.");
                return;
            }

            StopRepeat();

            _onLoopStart = onRepeat;
            _onLoopEnd = onEndRepeat;
            _isRepeating = true;

            _repeatEntry = state.SetAnimation(0, animName, true);
            if (_repeatEntry == null)
            {
                Debug.LogWarning($"[PlayRepeat] Cannot set animation '{animName}'.");
                _isRepeating = false;
                return;
            }

            _repeatEntry.Start += OnTrackStart;
            _repeatEntry.Complete += OnTrackComplete;
            _repeatEntry.End += OnTrackEnd;
            _repeatEntry.Interrupt += OnTrackEnd;
            _repeatEntry.Dispose += OnTrackDispose;
        }

        public virtual void StopRepeat(bool clearTrack = true)
        {
            _isRepeating = false;

            if (_repeatEntry != null)
            {
                _repeatEntry.Start -= OnTrackStart;
                _repeatEntry.Complete -= OnTrackComplete;
                _repeatEntry.End -= OnTrackEnd;
                _repeatEntry.Interrupt -= OnTrackEnd;
                _repeatEntry.Dispose -= OnTrackDispose;
                _repeatEntry = null;
            }

            if (clearTrack && State != null)
                State.ClearTrack(0);

            _onLoopStart = null;
            _onLoopEnd = null;
        }

        private void OnTrackStart(TrackEntry entry)
        {
            if (!_isRepeating || entry != _repeatEntry) return;
            _onLoopStart?.Invoke();
        }

        private void OnTrackComplete(TrackEntry entry)
        {
            if (!_isRepeating || entry != _repeatEntry) return;
            _onLoopEnd?.Invoke(); // just finished a loop
            _onLoopStart?.Invoke(); // consider next loop started
        }

        private void OnTrackEnd(TrackEntry entry)
        {
            if (entry != _repeatEntry) return;
            StopRepeat(clearTrack: false);
        }

        private void OnTrackDispose(TrackEntry entry)
        {
            if (entry != _repeatEntry) return;
            StopRepeat(clearTrack: false);
        }

        public virtual TrackEntry PlayAnimOnce(string animName, Action onComplete)
        {
            var state = State;
            if (state == null)
            {
                Debug.LogWarning("[PlayAnimOnce] No AnimationState.");
                return null;
            }

            var track = state.SetAnimation(0, animName, false);
            if (track != null)
                track.Complete += _ => onComplete?.Invoke();
            return track;
        }

        public virtual void ClearSkeleton()
        {
            var skeleton = Skeleton;
            var state = State;
            if (skeleton == null || state == null) return;

            state.ClearTracks();
            skeleton.SetBonesToSetupPose();
            skeleton.SetSlotsToSetupPose();
            skeleton.UpdateWorldTransform(Skeleton.Physics.None);
        }

        public virtual TrackEntry PlayAnims(
            List<string> animNames,
            bool endLoop = false,
            Action onCompleteAll = null,
            Action<string> onCompleteAnim = null)
        {
            var state = State;
            if (state == null)
            {
                Debug.LogWarning("[PlayAnims] No AnimationState.");
                return null;
            }

            Debug.Log("PlayAnims: " + string.Join(", ", animNames ?? new List<string>()));
            state.ClearTrack(0);

            if (animNames == null || animNames.Count == 0)
            {
                onCompleteAll?.Invoke();
                return null;
            }

            TrackEntry PlayNext(int index)
            {
                var entry = state.SetAnimation(0, animNames[index], false);
                if (entry != null)
                {
                    entry.Complete += _ =>
                    {
                        onCompleteAnim?.Invoke(animNames[index]);

                        if (index + 1 < animNames.Count)
                        {
                            PlayNext(index + 1);
                        }
                        else
                        {
                            if (endLoop)
                                state.SetAnimation(0, animNames[^1], true);

                            onCompleteAll?.Invoke();
                        }
                    };
                }

                return entry;
            }

            return PlayNext(0);
        }

        #endregion
    }
}*/