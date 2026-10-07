# Changelog

There are no release tags; CI uses the latest `main`.

## 2026-10-07

### Added
- `CocoaPodsConflictResolver` adds a Podfile `post_install` hook that raises pod deployment targets below the app's minimum iOS version. Fixes Xcode 27 "deployment target ... supported range is 15.0 to 27.0" errors. Turn off with `disableDeploymentTargetFix` in `PodfileOverrides.json`.

## 2026-10-01

### Added
- `CocoaPodsConflictResolver`: runs `pod install` during iOS builds and fixes Podfile version conflicts between `*Dependencies.xml` pods and the pods that depend on them. Optional config in `ProjectSettings/PodfileOverrides.json`.

## 2026-09-30

### Added
- `build-ios`, `build-android`, `build-windows` Unity Pipeline CLI commands.
- Pre-build check: fails if the Editor is not on the target platform, a recompile is pending, or scripts have compile errors.
