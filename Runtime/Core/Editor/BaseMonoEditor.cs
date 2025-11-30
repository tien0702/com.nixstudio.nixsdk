using NIX.Core;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class BaseMonoEditor
{
    static BaseMonoEditor()
    {
        ObjectFactory.componentWasAdded += OnComponentAdded;
    }

    private static void OnComponentAdded(Component comp)
    {
        if (comp is BaseMono baseMono)
        {
            baseMono.OnComponentAdded();
        }
    }
}
