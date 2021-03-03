
| Package | Latest Release | Downloads |
| --- | --- | --- |
| BlueDotBrigade.Datenlokator | [![latest version](https://img.shields.io/nuget/v/BlueDotBrigade.Datenlokator)](https://www.nuget.org/packages/BlueDotBrigade.Datenlokator) | [![downloads](https://img.shields.io/nuget/dt/BlueDotBrigade.Datenlokator)](https://www.nuget.org/packages/BlueDotBrigade.Datenlokator) |

# For Developers

## Sample Usage

Step 1: Initialize the framework once by calling `Setup()`.

```CSharp
[TestClass]
public class TestEnvironment
{
  [AssemblyInitialize]
  public static void Setup(TestContext context)
  {
    // As part of the initialization process, 
    // compressed input files (`*.zip`) will be decompressed.
    InputData.Setup();
  }

  [AssemblyCleanup]
  public static void Teardown()
  {
    InputData.Teardown();
  }
}
```

Step 2: Call `InputData.GetFilePath()` as needed.

```CSharp
using BlueDotBrigade.Weevil;

[TestMethod]
public void Clear_BeforeSelected_Returns200()
{
	// The input file associated with this test
	// will be automatically located.
	var engine = Engine
		.UsingPath(InputData.GetFilePath())
		.Open();

	engine
		.Selector
		.Select(lineNumber: 56);

	engine.Clear(ClearRecordsOperation.BeforeSelected);

	Assert.AreEqual(200, engine.Records.Length);
}
```
