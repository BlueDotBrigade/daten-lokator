# Daten Lokator

[![.NET](https://github.com/BlueDotBrigade/daten-lokator/actions/workflows/dotnet.yml/badge.svg)](https://github.com/BlueDotBrigade/daten-lokator/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/BlueDotBrigade/daten-lokator/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/BlueDotBrigade/daten-lokator/actions/workflows/github-code-scanning/codeql)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=BlueDotBrigade_daten-lokator&metric=sqale_rating)](https://sonarcloud.io/project/overview?id=BlueDotBrigade_daten-lokator)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=BlueDotBrigade_daten-lokator&metric=coverage)](https://sonarcloud.io/project/overview?id=BlueDotBrigade_daten-lokator)
[![Duplicated Lines](https://sonarcloud.io/api/project_badges/measure?project=BlueDotBrigade_daten-lokator&metric=duplicated_lines_density)](https://sonarcloud.io/project/overview?id=BlueDotBrigade_daten-lokator)


- [Overview](#overview)
- [Features](#features)
- [Getting Started](#getting-started)
   - [Compressed Input Data](#compressed-input-data)
   - [Sample Code](#sample-code)
- [Upgrading to 2.5.0](#upgrading-to-250)
- [Creating the Package](#creating-the-package)
- [Change Log][ChangeLog]

## Overview

[BlueDotBrigade.DatenLokator][NuGetPackage] is a .NET _NuGet_ package that streamlines automated testing by allowing you to load input data with a single line of code, thus reducing visual noise and improving test comprehension. This library supports multiple data formats, automatic decompression, global caching, and run-time customization.

## Features

1. Retrieve data in different formats:
   -	`AsBytes()`
   -	`AsFile()`
   -	`AsString()`
   -	`AsStream()`
   -	`AsStreamReader()`
   -	`AsJson()`
2. Automatic decompression of archives: `*.zip`, `*.7z`, `*.tar`
    -	Useful for saving disk space.
    -	See [Compressed Input Data](#compressed-input-data).
3. Global cache support.
    - Useful when multiple tests consume the same file as input.
4. Run-time customization.
    - Can be used to specify where the input data will be stored.
5. Extensible library.
    - Create [extension methods][ExtensionMethod] to support custom target formats (e.g. `AsRecord()`).
    - Implement the [ITestNamingStrategy][ITestNamingStrategy] interface to support custom test naming conventions.
    - Implement the [IFileManagementStrategy][IFileManagementStrategy] interface for proprietary file management (e.g. cloud based storage).

## Getting Started

Setup:

1. Create a .NET automated test project.
   - For example: Using _Visual Studio_ add a _MsTest Test Project_ to your solution.
   - _Daten Lokator_ will work with: _MsTest_, _NUnit_, _XUnit_, etc.
2. Add the latest _NuGet_ package to your test project:
   - [BlueDotBrigade.DatenLokator][NuGetPackage]
3. The _Daten Lokator_ library must be initialized only **once** when the automated tests start.
   - Example: `Lokator.Get().Setup()`
   - If you are using _MsTest_ then consider doing this where the `AssemblyInitialize` attribute is used.
4. Create a`.Daten` folder within the project (`*.csproj`) directory.
   - Example: `/DemoTests/.Daten`
   - Storing the input data within the project directory provides traceability, because now the input data can be committed to source control (e.g. *GitHub*).

Test data sources and mock endpoints:
- Instances of `Daten` can be used to load deterministic input fixtures from disk.
- `Lokator.Register()` can be used to setup lightweight sockets that serve static content — useful for testing HTTP clients without an external server.

Managing source files:

1. Create an automated test.
   - By default it is assumed that test method name follows the [Assert Act Arrange][AAA] naming convention.
2. Save the input data in a file directory structure that mirrors the namespace.
   - For example:
      - **Unit Tests:** `/DemoTests/Serialization/XmlSerializerTests.cs`
      - **Input Data:** `/DemoTests/.Daten/Serialization/XmlSerializerTests/*.*`
   - Where:
      - `.Daten` is _Daten Lokator_'s root directory for all input files.
      - `Serialization` is the namespace where the `XmlSerializerTests.cs` automated tests can be found.
3. When an input file is needed, simply call the appropriate `Daten` method from your automated test.
   - For example: `new Daten().AsString()`

### Compressed Input Data

Input data can be stored in an archive. Supported formats: `*.zip`, `*.7z`, `*.tar`

1. When `Lokator.Get().Setup()` runs, each archive under `.Daten` is decompressed into a sibling directory whose name ends with `~TMP`.
   - For example: `.Daten/Serialization/XmlSerializerTests.zip` is decompressed into `.Daten/Serialization/XmlSerializerTests~TMP/`
2. `Daten` then searches the `~TMP` directory as though it were the test's own directory.
   - Global data: when a test asks for `LargeFile.txt`, `.Daten/.Global/LargeFile~TMP/` is searched, which holds the content of `.Daten/.Global/LargeFile.zip`
3. The `~TMP` directory is kept between test runs, so an archive is only decompressed once.
   - After changing an archive, delete its `~TMP` directory so that it is decompressed again.
4. Add `*~TMP/` to your `.gitignore` file, so that decompressed files are not committed to source control.

### Sample Code

The [daten-lokator-demo][DemoRepository] repository contains a complete, runnable example: [unit tests][DemoTests], written for a trivial [application][DemoApp], that use the published [NuGet package][NuGetPackage] and demonstrate how:

1. easy it is to use _Daten Lokator_, and 
2. how the library reduces visual noise thus making the test easier to read

## Upgrading to 2.5.0

1. Your test project must target .NET 10 (`net10.0`) or later.
2. Archives are now decompressed into `<name>~TMP/` instead of `<name>~ZIP/`.
   - In your `.gitignore` file, replace `*~ZIP/` with `*~TMP/`.
   - Delete any `~ZIP` directories that are left under `.Daten`.

The [Change Log][ChangeLog] lists every change in this release.

## Creating the Package

From the repository's root folder, create the _NuGet_ package with:

```
dotnet pack -c Release
```

In Visual Studio, select the Release configuration, then right-click the project and choose __Pack__.

- The package is saved in `Src/BlueDotBrigade.DatenLokator.TestTools/bin/x64/Release`.
- Only a pack creates the package. A build does not, even in Release.
- Packing in Debug stops with an error, so a package built without optimization cannot be published by mistake.
- The debugging information (PDB) is embedded in the DLL, so stack traces still show file names and line numbers.

[DemoRepository]: https://github.com/BlueDotBrigade/daten-lokator-demo
[DemoApp]: https://github.com/BlueDotBrigade/daten-lokator-demo/tree/main/Src/Demo
[DemoTests]: https://github.com/BlueDotBrigade/daten-lokator-demo/blob/main/Tst/DemoTests/Serialization/XmlSerializerTests.cs

[NuGetPackage]: https://www.nuget.org/packages/BlueDotBrigade.DatenLokator
[ChangeLog]: https://github.com/BlueDotBrigade/daten-lokator/blob/main/ChangeLog.md

[AAA]: https://automationpanda.com/2020/07/07/arrange-act-assert-a-pattern-for-writing-good-tests/
[ExtensionMethod]: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods
[ITestNamingStrategy]: https://github.com/BlueDotBrigade/daten-lokator/blob/main/Src/BlueDotBrigade.DatenLokator.TestTools/NamingConventions/ITestNamingStrategy.cs
[IFileManagementStrategy]: https://github.com/BlueDotBrigade/daten-lokator/blob/main/Src/BlueDotBrigade.DatenLokator.TestTools/IO/IFileManagementStrategy.cs
