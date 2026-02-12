using UnityEngine;
using UnityEditor;
using System.Linq;

namespace NIX.Core.Extend
{
    [CustomPropertyDrawer(typeof(AnimationAttribute))]
    public class AnimatorAnimationDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var target = property.serializedObject.targetObject as MonoBehaviour;

            if (target == null)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            Animator animator = target.GetComponent<Animator>();
            if (animator == null)
                animator = target.GetComponentInChildren<Animator>();

            if (animator == null || animator.runtimeAnimatorController == null)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            var clips = animator.runtimeAnimatorController.animationClips;
            var clipNames = clips.Select(c => c.name).Distinct().ToArray();

            if (clipNames.Length == 0)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            int index = Mathf.Max(0, System.Array.IndexOf(clipNames, property.stringValue));

            int newIndex = EditorGUI.Popup(position, label.text, index, clipNames);

            if (newIndex != index && newIndex >= 0 && newIndex < clipNames.Length)
            {
                property.stringValue = clipNames[newIndex];
            }
        }
    }
}