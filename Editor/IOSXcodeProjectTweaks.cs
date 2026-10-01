#if UNITY_EDITOR && UNITY_IOS
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;
using UnityEngine;

/// <summary>
/// Sets ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES = NO on the project, Unity-iPhone and UnityFramework.
///
/// Order: EDM4U installs pods at 50 and CocoaPodsConflictResolver runs at 45, so 100 runs after
/// both and pod install can't overwrite the setting.
/// </summary>
public class IOSXcodeProjectTweaks : IPostprocessBuildWithReport
{
    private const string Tag = "[XcodeTweaks]";

    public int callbackOrder => 100;

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.iOS)
            return;

        string pbxPath = PBXProject.GetPBXProjectPath(report.summary.outputPath);
        var project = new PBXProject();
        project.ReadFromFile(pbxPath);

        string[] targets =
        {
            project.ProjectGuid(),
            project.GetUnityMainTargetGuid(),
            project.GetUnityFrameworkTargetGuid(),
        };

        foreach (string guid in targets)
            project.SetBuildProperty(guid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");

        project.WriteToFile(pbxPath);
        Debug.Log($"{Tag} ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES = NO (project, Unity-iPhone, UnityFramework)");
    }
}
#endif
