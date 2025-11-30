#if UNITY_EDITOR
using NIX.Packages;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class ImageReplacerEditor
{
    [MenuItem("CONTEXT/Image/Replace With AlphaCheckImage")]
    private static void ReplaceWithAlphaCheckImage(MenuCommand command)
    {
        Image oldImage = command.context as Image;
        GameObject go = oldImage.gameObject;

        SerializedObject oldSerialized = new SerializedObject(oldImage);

        var sprite = oldImage.sprite;
        var color = oldImage.color;
        var material = oldImage.material;
        var raycastTarget = oldImage.raycastTarget;
        var preserveAspect = oldImage.preserveAspect;
        var type = oldImage.type;
        var fillMethod = oldImage.fillMethod;
        var fillOrigin = oldImage.fillOrigin;
        var fillClockwise = oldImage.fillClockwise;
        var fillAmount = oldImage.fillAmount;

        Undo.DestroyObjectImmediate(oldImage);

        AlphaCheckImage newImage = Undo.AddComponent<AlphaCheckImage>(go);

        newImage.sprite = sprite;
        newImage.color = color;
        newImage.material = material;
        newImage.raycastTarget = raycastTarget;
        newImage.preserveAspect = preserveAspect;
        newImage.type = type;
        newImage.fillMethod = fillMethod;
        newImage.fillOrigin = fillOrigin;
        newImage.fillClockwise = fillClockwise;
        newImage.fillAmount = fillAmount;

        EditorUtility.SetDirty(go);
        Debug.Log($"✅ Replaced Image with AlphaCheckImage on '{go.name}'", go);
    }
}
#endif