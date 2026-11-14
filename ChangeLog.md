# Change Log

Notable changes to the [BlueDotBrigade.DatenLokator][NuGetPackage] NuGet package, newest first.

## 2.5.0

### Breaking Changes

- The package targets .NET 10 (`net10.0`) instead of .NET 9 (`net9.0`). #51
- Archives are decompressed into `<name>~TMP/` instead of `<name>~ZIP/`, because the name now suits every archive format. #49
  - In your `.gitignore` file, replace `*~ZIP/` with `*~TMP/`.
  - Delete any `~ZIP` directories that are left under `.Daten`. They are no longer used.

### Added

- `*.7z` and `*.tar` archives are decompressed, in addition to `*.zip`. #49
  - `*.7z` support adds a dependency on the [SharpCompress][SharpCompress] NuGet package (MIT).
- `Setup()` stops with an error when two archives in one directory share a name (e.g. `Sample.zip` and `Sample.7z`), because both would be decompressed into the same `Sample~TMP/` directory. #49

### Fixed

- The package contains an optimized Release build, so [BenchmarkDotNet][BenchmarkDotNet] no longer refuses to run in projects that reference it. #52
  - The debugging information (PDB) is embedded in the DLL, so stack traces still show file names and line numbers.
- An archive is recognized by its exact extension: `Sample.zipx` is no longer treated as a `*.zip` file. #49
- Only a directory's own name marks it as decompressed. Previously, `~ZIP` anywhere in a path, including a parent of `.Daten`, stopped archives from being decompressed. #49
- A failed decompression removes its partial directory, so that the next test run does not use incomplete data. #49

### Changed

- Dependencies were upgraded: `MSTest.TestFramework` 4.5.2 and `System.Configuration.ConfigurationManager` 10.0.14. `System.Collections.Immutable` is no longer referenced. #51
- The Demo sample moved to the [daten-lokator-demo][DemoRepository] repository. #48

[NuGetPackage]: https://www.nuget.org/packages/BlueDotBrigade.DatenLokator
[SharpCompress]: https://www.nuget.org/packages/SharpCompress
[BenchmarkDotNet]: https://benchmarkdotnet.org
[DemoRepository]: https://github.com/BlueDotBrigade/daten-lokator-demo
