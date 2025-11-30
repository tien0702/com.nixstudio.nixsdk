#if USE_SPINE
using Spine;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Animation = Spine.Animation;
using AnimationState = Spine.AnimationState;

namespace NIX.Utils
{
    public static class SpineUtils
    {
        public static void SetSkins(SkeletonGraphic skeleton, List<string> skinNames)
        {
            Skin newSkin = new Skin("new-skin");
            if (skinNames == null || skinNames.Count <= 0)
            {
                skeleton.Skeleton.SetSkin((Skin)null);
                return;
            }

            skinNames?.ForEach(skinName => { newSkin.AddSkin(skeleton.SkeletonData.FindSkin(skinName)); });
            skeleton.Skeleton.SetSkin(newSkin);
            skeleton.Skeleton.SetSlotsToSetupPose();
            skeleton.AnimationState.Apply(skeleton.Skeleton);
        }

        public static void SetSkins(SkeletonAnimation skeleton, List<string> skinNames)
        {
            Skin newSkin = new Skin("new-skin");

            skinNames?.ForEach(skinName =>
            {
                var foundSkin = skeleton.skeleton.Data.FindSkin(skinName);
                if (foundSkin == null)
                {
                    Debug.LogError($"Skin '{skinName}' not found in skeleton data!");
                }
                else
                {
                    newSkin.AddSkin(foundSkin);
                }
            });

            skeleton.Skeleton.SetSkin(newSkin);
            skeleton.Skeleton.SetSlotsToSetupPose();
            skeleton.AnimationState.Apply(skeleton.Skeleton);
        }

        public static bool UpdateImmediate(SkeletonGraphic skeleton, float time)
        {
            if (skeleton == null) return false;

            skeleton.AnimationState.GetCurrent(0).TrackTime = time;
            skeleton.Update(time);
            skeleton.LateUpdate();

            return true;
        }

        /// <summary>
        /// Plays the animation once. After that, it loops from the event marker to the end recursively.
        /// </summary>
        public static void PlayFromEventLoop(this AnimationState animationState, string animName, string eventName)
        {
            float loopStartTime = 0f;
            bool loopPointFound = false;

            // Step 1: Play full animation once
            var entry = animationState.SetAnimation(0, animName, false);
            entry.MixDuration = 0f;

            entry.Event += (trackEntry, spineEvent) =>
            {
                if (spineEvent.Data.Name == eventName && !loopPointFound)
                {
                    loopStartTime = trackEntry.AnimationTime;
                    loopPointFound = true;
                    Debug.Log($"[Spine] Event '{eventName}' found at {loopStartTime}s");
                }
            };

            entry.Complete += (trackEntry) =>
            {
                if (!loopPointFound)
                {
                    Debug.LogWarning($"[Spine] Event '{eventName}' not found in '{animName}'. No loop applied.");
                    return;
                }

                // Begin recursive looping from loopStartTime
                RecursiveLoop(animationState, animName, loopStartTime);
            };
        }

        /// <summary>
        /// Internal method that keeps looping from loopStartTime to end using recursive Complete callback.
        /// </summary>
        public static void RecursiveLoop(AnimationState state, string animName, float loopStartTime)
        {
            var loopEntry = state.SetAnimation(0, animName, false); // Play once again (manual loop)
            loopEntry.TrackTime = loopStartTime;
            loopEntry.MixDuration = 0f;

            loopEntry.Complete += (entry) =>
            {
                // Loop again recursively
                RecursiveLoop(state, animName, loopStartTime);
            };
        }

        public static bool HasAnimation(SkeletonGraphic graphic, string animationName)
        {
            if (graphic == null || graphic.Skeleton == null)
                return false;

            SkeletonData skeletonData = graphic.Skeleton.Data;

            var anim = skeletonData.FindAnimation(animationName);

            return anim != null;
        }

        /// <summary>
        /// Get the initial (first keyframe) local position (X,Y) of a bone in a given animation.
        /// </summary>
        public static Vector2? GetInitialBonePos(Animation animation, Skeleton skeleton, string boneName)
        {
            Bone bone = skeleton.FindBone(boneName);
            if (bone == null)
            {
                Debug.LogWarning("Bone not found: " + boneName);
                return null;
            }

            int boneIndex = bone.Data.Index;

            foreach (Timeline timeline in animation.Timelines)
            {
                if (timeline is TranslateTimeline translateTimeline)
                {
                    if (translateTimeline.BoneIndex == boneIndex)
                    {
                        float[] frames = translateTimeline.Frames;
                        if (frames.Length >= 3)
                        {
                            float x = frames[1];
                            float y = frames[2];
                            return new Vector2(x, y);
                        }
                    }
                }
            }

            return null;
        }

        public static Vector2 ScreenPointToSkeletonPosition(
            SkeletonGraphic skeletonGfx,
            PointerEventData eventData)
        {
            Camera cam = eventData.pressEventCamera ?? Camera.main;
            RectTransform rt = skeletonGfx.rectTransform;

            Vector3 screenPos = eventData.position;
            Vector3 worldPoint = cam.ScreenToWorldPoint(new Vector3(
                screenPos.x, screenPos.y, rt.position.z));

            Vector3 localPoint = rt.InverseTransformPoint(worldPoint);

            Vector3 noScale = new Vector3(
                localPoint.x / rt.localScale.x,
                localPoint.y / rt.localScale.y,
                0f);

            float importScale = skeletonGfx.skeletonDataAsset.scale;
            Vector2 skeletonSpace = new Vector2(
                noScale.x / importScale,
                noScale.y / importScale);

            if (skeletonGfx.Skeleton.ScaleY < 0f)
                skeletonSpace.y = -skeletonSpace.y;

            return skeletonSpace;
        }
    }
}
#endif