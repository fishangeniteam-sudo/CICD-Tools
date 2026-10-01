# CICD Tools

Editor-only Unity Pipeline CLI commands used by the CI workflows.

| Command | What it does |
|---|---|
| `build-ios` | Exports the Xcode project to `Builds/iOS/XcodeProject` |
| `build-android` | Builds an APK (or AAB with `buildAppBundle=true`) |
| `build-windows` | Builds Standalone Windows 64-bit |
| `fix-ios-pods` | Aligns conflicting `iosPod` versions in `*Dependencies.xml` to the highest one (`dryRun=true` only reports) |

The Editor must already be on the target platform when a build command runs
(launch once with `unity run . -- -buildTarget iOS` first).

## Install

Requires Unity 6.0+ and `com.unity.pipeline` (`unity pipeline install`).
Install the Pipeline package first, otherwise this package fails to compile.

Pin to a release tag with the `#v…` suffix. Without it, Unity takes the latest
commit on `main` and locks it in `Packages/packages-lock.json`.

### Option A: Unity Package Manager (UPM) window

1. In Unity, open **Window → Package Manager**.
2. Click the **+** button (top-left) and choose **Install package from git URL…**
3. Paste the URL and click **Install**:

   ```
   https://github.com/fishangeniteam-sudo/CICD-Tools.git#v1.0.0
   ```

4. The package appears as **CICD Tools** under **In Project**.

To update later, remove it and add it again with the new tag, or edit the tag
in `Packages/manifest.json` (Option B).

### Option B: Edit `Packages/manifest.json`

Add this line to `dependencies`:

```json
"com.fishan.cicd-tools": "https://github.com/fishangeniteam-sudo/CICD-Tools.git#v1.0.0"
```

Unity resolves it the next time the Editor opens or regains focus.

### Private repository

If the repo is private, Unity clones it with your system `git`, so `git` must be
able to authenticate to GitHub without a prompt (Git Credential Manager on
Windows, the macOS keychain, or `~/.git-credentials` on the CI runner).

### Replacing a copy in `Assets/`

If the project already has `Assets/Editor/BuildPipelineCommands.cs`, delete it
and its `.meta` file. The package uses the same GUID and class name, and having
both causes duplicate-class errors.

## Usage

```bash
unity run . -- -buildTarget iOS          # switch platform once
unity run . --command fix-ios-pods       # align iosPod versions (optional)
unity run . --command build-ios          # then build
```

## Releasing

1. Bump `version` in `package.json` and add a `CHANGELOG.md` entry.
2. Commit, then tag: `git tag v1.0.1 && git push origin v1.0.1`.
3. Update the `#v…` suffix in each project's manifest.
