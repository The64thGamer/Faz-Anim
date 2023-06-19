using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.XR.Management;
using UnityEditor.XR.Management;

[CustomEditor(typeof(SimulatorManager))]
public class VersionManager : Editor
{
    [HideInInspector]
    public bool version;
    [HideInInspector]
    public bool VR;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SimulatorManager exam = (SimulatorManager)target;
        ///////////////////////////////////////////////////////////////////////////////////////////
        GUILayout.Space(20);
        if (VR)
        {
            GUI.backgroundColor = Color.green;
        }
        else
        {
            GUI.backgroundColor = Color.grey;
        }
        if (GUILayout.Button("Toggle VR Build"))
        {
            SetVR();
        }
        GUILayout.Space(20);
        ///////////////////////////////////////////////////////////////////////////////////////////
        if (version)
        {
            GUI.backgroundColor = Color.green;
        }
        else
        {
            GUI.backgroundColor = Color.grey;
        }
        if (GUILayout.Button("Set Faz-Anim"))
        {
            SetFazAnim();
        }
        if (EditorGUI.EndChangeCheck() && version)
        {
            if (VR)
            {
                PlayerSettings.bundleVersion = exam.fazAnimVersion + " VR";
            }
            else
            {
                PlayerSettings.bundleVersion = exam.fazAnimVersion;
            }

        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        GUILayout.Space(20);
        if (version)
        {
            GUI.backgroundColor = Color.grey;
        }
        else
        {
            GUI.backgroundColor = Color.green;
        }
        if (GUILayout.Button("Set Rock-afire Replay"))
        {
            SetSimulator();
        }
        if (EditorGUI.EndChangeCheck() && !version)
        {
            if (VR)
            {
                PlayerSettings.bundleVersion = exam.rockafireReplayVersion + " VR";
            }
            else
            {
                PlayerSettings.bundleVersion = exam.rockafireReplayVersion;
            }
        }
           }
    public void SetFazAnim()
    {
        SimulatorManager exam = (SimulatorManager)target;
        version = true;
        // Find valid Scene paths and make a list of EditorBuildSettingsScene
        List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
        foreach (string scenePath in exam.fazAnimScenes)
        {
            if (!string.IsNullOrEmpty(scenePath))
                editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));
        }
        // Set the Build Settings window Scene list
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();

        PlayerSettings.productName = "Faz-Anim";
        Texture2D[] tex = new Texture2D[8];
        for (int i = 0; i < 8; i++)
        {
            tex[i] = exam.fazTexture as Texture2D;
        }
        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Standalone, tex);
    }
    public void SetSimulator()
    {
        SimulatorManager exam = (SimulatorManager)target;
        version = false;
        // Find valid Scene paths and make a list of EditorBuildSettingsScene
        List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
        foreach (string scenePath in exam.simulatorScenes)
        {
            if (!string.IsNullOrEmpty(scenePath))
                editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));
        }

        // Set the Build Settings window Scene list
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();

        PlayerSettings.productName = "Reel to Real";
        Texture2D[] tex = new Texture2D[8];
        for (int i = 0; i < 8; i++)
        {
            tex[i] = exam.rockTexture as Texture2D;
        }
        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Standalone, tex);
    }

    public void SetVR()
    {
        XRGeneralSettingsPerBuildTarget generalSettings = null;
        EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out generalSettings);
        if (generalSettings == null)
        {
            EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out generalSettings);
            if (generalSettings == null)
            {
                string searchText = "t:XRGeneralSettings";
                string[] assets = AssetDatabase.FindAssets(searchText);
                if (assets.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(assets[0]);
                    generalSettings = AssetDatabase.LoadAssetAtPath(path, typeof(XRGeneralSettingsPerBuildTarget)) as XRGeneralSettingsPerBuildTarget;
                }
            }
            EditorBuildSettings.AddConfigObject(XRGeneralSettings.k_SettingsKey, generalSettings, true);
        }

        BuildTargetGroup buildTargetGroup = EditorGUILayout.BeginBuildTargetSelectionGrouping();
        XRGeneralSettings settings = generalSettings.SettingsForBuildTarget(buildTargetGroup);

        var serializedSettingsObject = new SerializedObject(settings);
        serializedSettingsObject.Update();
        SerializedProperty initOnStart = serializedSettingsObject.FindProperty("m_InitManagerOnStart");
        if (VR)
        {
            initOnStart.boolValue = false;
        }
        else
        {
            initOnStart.boolValue = true;
        }
        serializedSettingsObject.ApplyModifiedProperties();
        VR = !VR;
        UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
    }
}
