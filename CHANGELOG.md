# Changelog

## [1.1.0] - 2026-09-30

### Added
- `fix-ios-pods` command: finds pods whose version requirements cannot all be met and rewrites the `*Dependencies.xml` entries to the highest required version. Checks direct `iosPod` entries and, when local CocoaPods spec repos exist (`~/.cocoapods/repos`), the dependencies declared in those pods' podspecs (e.g. `AppLovinMediationGoogleAdapter 13.10.0.0` → `Google-Mobile-Ads-SDK = 13.10.0`). Subspecs are grouped by root pod. Fails if a requirement that must change comes from a read-only package or a podspec. `dryRun=true` only reports.

## [1.0.0] - 2026-09-30

### Added
- `build-ios`, `build-android`, `build-windows` Unity Pipeline CLI commands.
- Pre-build guard: fails if the Editor is not on the target platform, a recompile is pending, or scripts have compile errors.
