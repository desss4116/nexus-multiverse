using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class BlacksiteProjectSetup
{
    static BlacksiteProjectSetup()
    {
        EditorApplication.delayCall += EnsureProject;
    }

    [MenuItem("BLACKSITE/Prepare Android Project")]
    public static void EnsureProject()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
        if (!AssetDatabase.IsValidFolder("Assets/Editor")) return;
        if (!AssetDatabase.IsValidFolder("Assets/Scenes")) AssetDatabase.CreateFolder("Assets", "Scenes");

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var root = new GameObject("BLACKSITE OPERATOR");
        root.AddComponent<BlacksiteGame>();
        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Main.unity");
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/Scenes/Main.unity", true) };

        PlayerSettings.productName = "BLACKSITE: OPERATOR";
        PlayerSettings.companyName = "Blacksite Studio";
        PlayerSettings.applicationIdentifier = "com.blacksite.operator";
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.bundleVersion = "0.1.0";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.blacksite.operator");
        AssetDatabase.SaveAssets();
    }
}
