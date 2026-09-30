using System;
using System.IO;
using System.Linq;
using Unity.Pipeline.Commands;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildPipelineCommands
{
    // ==========================================
    // WINDOWS BUILD
    // ==========================================
    [CliCommand("build-windows", "Builds Standalone Windows 64-bit using scenes from Build Settings")]
    public static void BuildWindows(
        [CliArg("outputPath", "Target output path (exe)", Required = false)]
        string outputPath = "Builds/Windows/MyGame.exe")
    {
        ExecuteBuild(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64, outputPath);
    }

    // ==========================================
    // ANDROID BUILD
    // ==========================================
    [CliCommand("build-android", "Builds Android APK or AAB using scenes from Build Settings")]
    public static void BuildAndroid(
        [CliArg("outputPath", "Target output path (.apk or .aab)", Required = false)]
        string outputPath = "Builds/Android/MyGame.apk",
        [CliArg("buildAppBundle", "Set to true to generate an .aab instead of .apk", Required = false)]
        bool buildAppBundle = false)
    {
        EditorUserBuildSettings.buildAppBundle = buildAppBundle;
        ExecuteBuild(BuildTargetGroup.Android, BuildTarget.Android, outputPath);
    }

    public static void BuildAndroidCI()
    {
        EditorUserBuildSettings.buildAppBundle = false;

        ExecuteBuild(
            BuildTargetGroup.Android,
            BuildTarget.Android,
            "Builds/Android/MyGame.apk"
        );
    }

    // ==========================================
    // iOS BUILD (Outputs Xcode Project)
    // ==========================================
    [CliCommand("build-ios", "Builds iOS Xcode project directory using scenes from Build Settings")]
    public static void BuildIOS(
        [CliArg("outputPath", "Target directory for exported Xcode project", Required = false)]
        string outputPath = "Builds/iOS/XcodeProject")
    {
        ExecuteBuild(BuildTargetGroup.iOS, BuildTarget.iOS, outputPath);
    }

    // ==========================================
    // COMMON BUILD RUNNER
    // ==========================================
    // Failures throw: the Pipeline server turns an exception into a failure result,
    // so `unity run --command ...` exits non-zero instead of 0 with a failed build.
    private static void ExecuteBuild(BuildTargetGroup targetGroup, BuildTarget target, string outputPath)
    {
        EnsureEditorReadyFor(target);

        string[] scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            throw new InvalidOperationException(
                "[CLI Build] No enabled scenes found in File -> Build Settings.");
        }

        string directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            targetGroup = targetGroup,
            target = target,
            options = BuildOptions.None
        };

        Debug.Log($"[CLI Build] Starting build for {target} -> {outputPath}");
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result != BuildResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"[CLI Build] {target} build finished with result {summary.result} " +
                $"({summary.totalErrors} error(s)). See the Editor log above for details.");
        }

        Debug.Log($"[CLI Build] Success! Output generated at: {outputPath} " +
                  $"({summary.totalSize / (1024f * 1024f):F1} MB, {summary.totalTime.TotalSeconds:F0}s)");
    }

    // ==========================================
    // GUARD
    // ==========================================
    // The Editor must already be on the build target when the build command runs.
    // Switching inside this call is not enough: scripts recompile with the new
    // target's define symbols, but the domain only reloads after this method
    // returns, so BuildPlayer would still validate against the old editor
    // assemblies ("script class layout is incompatible between the editor and
    // the player"). Launch the Editor on the target first
    // (`unity run . -- -buildTarget iOS`), then run the build command.
    private static void EnsureEditorReadyFor(BuildTarget target)
    {
        BuildTarget active = EditorUserBuildSettings.activeBuildTarget;
        if (active != target)
        {
            throw new InvalidOperationException(
                $"[CLI Build] Active build target is {active}, expected {target}. " +
                $"Switch first in a separate Editor launch, e.g. `unity run . -- -buildTarget {target}`, " +
                "then run the build command.");
        }

        // Import anything that changed on disk since the Editor opened (e.g. files
        // written by earlier CI steps). This is synchronous.
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        // If that refresh picked up script changes, a compile + domain reload is now
        // pending and cannot finish inside this call. Fail fast instead of building
        // against stale assemblies.
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            throw new InvalidOperationException(
                "[CLI Build] Scripts changed during the pre-build refresh and a recompile is pending. " +
                "Re-run the platform switch step so the Editor compiles before building.");
        }

        if (EditorUtility.scriptCompilationFailed)
        {
            throw new InvalidOperationException(
                "[CLI Build] Scripts have compile errors for this target. Fix them before building.");
        }

        Debug.Log($"[CLI Build] Editor is on {active}, assets refreshed, scripts compiled. Continuing.");
    }
}
