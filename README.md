# Daten Lokator

[![.NET](https://github.com/BlueDotBrigade/daten-lokator/actions/workflows/dotnet.yml/badge.svg)](https://github.com/BlueDotBrigade/daten-lokator/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/BlueDotBrigade/daten-lokator/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/BlueDotBrigade/daten-lokator/actions/workflows/github-code-scanning/codeql)

- [Overview](#overview)
- [Features](#features)
- [Getting Started](#getting-started)
   - [Sample Code](#sample-code)

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
2. Automatic decompression of `*.zip` files.
    -	Useful for saving disk space.
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

### Sample Code

The following [unit tests][DemoTests], written for a trivial [application][DemoApp], demonstrate how:

1. easy it is to use _Daten Lokator_, and 
2. how the library reduces visual noise thus making the test easier to read

[DemoApp]: https://github.com/BlueDotBrigade/daten-lokator/tree/main/Src/Demo
[DemoTests]: https://github.com/BlueDotBrigade/daten-lokator/blob/main/Tst/DemoTests/Serialization/XmlSerializerTests.cs

[NuGetPackage]: https://www.nuget.org/packages/BlueDotBrigade.DatenLokator

[AAA]: https://automationpanda.com/2020/07/07/arrange-act-assert-a-pattern-for-writing-good-tests/
[ExtensionMethod]: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods
[ITestNamingStrategy]: https://github.com/BlueDotBrigade/daten-lokator/blob/main/Src/BlueDotBrigade.DatenLokator.TestTools/NamingConventions/ITestNamingStrategy.cs
[IFileManagementStrategy]: https://github.com/BlueDotBrigade/daten-lokator/blob/main/Src/BlueDotBrigade.DatenLokator.TestTools/IO/IFileManagementStrategy.cs
