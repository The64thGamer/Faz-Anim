using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;

public class VersionWriter : IPreprocessBuildWithReport
{
    private const string targetFile = "GameVersion.cs";

    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        WriteVersion();
    }

    [DidReloadScripts]
    public static void WriteVersion()
    {
        string finalPath = Path.Combine(Application.dataPath + "/Scripts/VR/", targetFile);

        string newText = $"public static class GameVersion\r\n" +
            "{\r\n" +
            FormatVar("gameName", PlayerSettings.productName);
        if (PlayerSettings.bundleVersion.Length == 7)
        {
            newText += FormatVar("gameVersion", PlayerSettings.bundleVersion.Substring(0, PlayerSettings.bundleVersion.Length - 3));
            newText += FormatVar("isVR", "true");
        }
        else
        {
            newText += FormatVar("gameVersion", PlayerSettings.bundleVersion);
            newText += FormatVar("isVR", "false");
        }
        newText += "}";

       string currentText = File.ReadAllText(finalPath);

        if (currentText != newText)
        {
            Debug.Log("Updated GameVersion.cs");

            File.WriteAllText(finalPath, newText);
            AssetDatabase.Refresh();
        }
    }

    private static string FormatVar(string varName, string varValue)
    {
        return $"    public const string {varName} = \"{varValue}\";\r\n";
    }
}