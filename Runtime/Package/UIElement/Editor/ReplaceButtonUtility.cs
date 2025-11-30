using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace NIX.Packages
{
    public static class ReplaceButtonUtility
    {
        [MenuItem("CONTEXT/Button/Replace With AdvancedButton")]
        private static void ReplaceWithAdvancedButton(MenuCommand command)
        {
            Button oldButton = command.context as Button;
            if (oldButton == null) return;

            GameObject go = oldButton.gameObject;

            // Cache old properties
            var targetGraphic = oldButton.targetGraphic;
            var interactable = oldButton.interactable;
            var transition = oldButton.transition;
            var colors = oldButton.colors;
            var spriteState = oldButton.spriteState;
            var animationTriggers = oldButton.animationTriggers;
            var navigation = oldButton.navigation;
            var onClick = oldButton.onClick;

            // Remove old component
            Undo.DestroyObjectImmediate(oldButton);

            // Add new component
            AdvancedButton newButton = Undo.AddComponent<AdvancedButton>(go);

            // Restore properties
            newButton.targetGraphic = targetGraphic;
            newButton.interactable = interactable;
            newButton.transition = transition;
            newButton.colors = colors;
            newButton.spriteState = spriteState;
            newButton.animationTriggers = animationTriggers;
            newButton.navigation = navigation;

            // Copy events
            newButton.onClick = onClick;

            // Mark dirty
            EditorUtility.SetDirty(go);
            Debug.Log($"✅ Replaced Button with AdvancedButton on '{go.name}'", go);
        }
    }
}