# CICD Tools

Editor-only Unity Pipeline CLI commands used by the CI workflows.

| Command | What it does |
|---|---|
| `build-ios` | Exports the Xcode project to `Builds/iOS/XcodeProject` |
| `build-android` | Builds an APK (or AAB with `buildAppBundle=true`) |
| `build-windows` | Builds Standalone Windows 64-bit |

The Editor must already be on the target platform when a build command runs
(launch once with `unity run . -- -buildTarget iOS` first).

## Install

Requires Unity 6.0+ and `com.unity.pipeline` (`unity pipeline install`).
The assembly only compiles when `com.unity.pipeline` is present, so projects
without it are not affected.

Add to `Packages/manifest.json`, pinned to a tag:

```json
"com.fishan.cicd-tools": "https://github.com/fishangeniteam-sudo/CICD-Tools.git#v1.0.0"
```

Or from the command line in CI:

```bash
unity run . --command build-ios
```

## Releasing

1. Bump `version` in `package.json` and add a `CHANGELOG.md` entry.
2. Commit, then tag: `git tag v1.0.1 && git push origin v1.0.1`.
3. Update the `#v…` suffix in each project's manifest.
