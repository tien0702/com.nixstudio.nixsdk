using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AudioConfigSO : ScriptableObject
{
    public static AudioConfigSO Instance
    {
        get
        {
            var config = AssetDatabase.LoadAssetAtPath<AudioConfigSO>("Assets/Editor/AudioConfigSO.asset");
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<AudioConfigSO>();
                AssetDatabase.CreateAsset(config, "Assets/Editor/AudioConfigSO.asset");
                
                config.MusicSources = new ();
                config.SfxSources = new ();

                config.SfxExportFolder = AssetDatabase.LoadAssetAtPath<DefaultAsset>("Assets/Resources/SO/Audio/SFX");
                config.MusicExportFolder = AssetDatabase.LoadAssetAtPath<DefaultAsset>("Assets/Resources/SO/Audio/BGM");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return config;
        }
    }

    public List<DefaultAsset> SfxSources;
    public List<DefaultAsset> MusicSources;

    public DefaultAsset SfxExportFolder;
    public DefaultAsset MusicExportFolder;
}