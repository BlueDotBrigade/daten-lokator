# Daten Lokator

- [Overview](#overview)
- [Features](#features)
- [Examples](#examples)

## Overview

If your automated tests are verifying more than just value types (`string`, `int`, `float`, etc), then consider using this *NuGet* package to automatically locate and load your test data (`*.log`, `*.xml`, `*.json`, `*.jpg`, etc). All you need to do is call:

- `new Daten().AsStream()`

Where the method name represents the target format:

-	`AsFile()`
-	`AsString()`
-	`AsStream()`
-	`AsBytes()`
-	etc.

## Features

1. Automatic decompression of compressed (`*.zip`) files.
    -	Useful for saving disk space.
2. Global cache support.
    - Useful when multiple rests require the same file as input.
3. Run-time customization.
    - Provide a root directory that is organization specific.
4. Extensible library.
    - Create [extension methods][ExtensionMethod] to support custom target formats (e.g. `AsRecord()`).
    - Implement the `ITestNamingStrategy` interface to support custom test naming conventions.
    - Implement the `IFileManagementStrategy` interface for proprietary file management (e.g. cloud file storage).

[ExtensionMethod]: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods

## Examples

The following [sample code][DemoTests] written for a trivial [application][DemoApp] demonstrates how:

1. easy it is to use *Daten Lokator*, and 
2. how the library reduces visual noise thus making the test easier to read

[DemoApp]: https://github.com/BlueDotBrigade/daten-lokator/tree/Features/6-EasierToExtend/Src/BlueDotBrigade.DatenLokator.Demo
[DemoTests]: https://github.com/BlueDotBrigade/daten-lokator/blob/Features/6-EasierToExtend/Tst/BlueDotBrigade.DatenLokator.DemoTests/Serialization/XmlSerializerTests.cs