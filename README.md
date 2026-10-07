# CICD Tools

Editor scripts for building Unity projects from the command line in CI.
Requires Unity 6.0+ and the `com.unity.pipeline` package (`unity pipeline install`).

## What's inside

**`BuildPipelineCommands`**: Unity Pipeline CLI commands.

| Command | Output |
|---|---|
| `build-ios` | Xcode project in `Builds/iOS/XcodeProject` |
| `build-android` | APK, or AAB with `buildAppBundle=true` |
| `build-windows` | Standalone Windows 64-bit |

Each command builds the enabled scenes from Build Settings and fails if the
Editor isn't already on the target platform, so switch first in a separate launch:

```bash
unity run . -- -buildTarget iOS      # switch platform
unity run . --command build-ios      # build
```

**`CocoaPodsConflictResolver`**: runs automatically during iOS builds, after
EDM4U writes the Podfile. It runs `pod install`. If a pod listed in a
`*Dependencies.xml` conflicts with the version another pod (e.g. a mediation
adapter) needs, it removes the direct version and retries. If a conflict can't be
fixed safely, the build fails with a report naming the XML files involved.

Optional per-project config in `ProjectSettings/PodfileOverrides.json`:

```json
{
  "disableAutoResolve": false,
  "overrides": [ { "pod": "Some-SDK", "version": "1.2.3" } ]
}
```

A pod with a version here is never changed by the resolver. `"version": ""` removes the constraint.

## How CI uses it

The Gitea iOS workflow downloads the `.cs` files from `main` into
`Assets/Editor/CI` before opening the Editor, so a push to `main` is used by the
next CI run. To use something other than `main`, set the Actions variable
`CI_TOOLS_REF` to a branch or commit SHA.

Projects must not also contain these classes (as a package or as a copy in
`Assets/`); the workflow fails if they do, because duplicate classes don't compile.

## Installing in a project (optional)

To use the commands locally, add this to `dependencies` in `Packages/manifest.json`:

```json
"com.fishan.cicd-tools": "https://github.com/fishangeniteam-sudo/CICD-Tools.git"
```

Unity locks the current commit in `Packages/packages-lock.json`. To update,
delete that entry from the lock file (or remove and re-add the package).
Don't install it in projects built by the CI workflow (see above).
