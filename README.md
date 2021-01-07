## ADAPI-WATERMARK
This is a small utility that will watermark a PDF document.

### Requirements
- Visual Studio 2019 Community or later
- nuget

### Getting Started
- Open the `watermark-utility.sln` with Visual Studio 2019 Community
- Build and Run the project
    - Before running the project you should open and edit the `launchSettings.json` file and replace
    the `<placeholder>` text with real values.

#### Building from the Command Line
From the root of the repository
- `dotnet build .\watermark-utility\WatermarkUtility.csproj`

This will output a self-contained .net 5.0 command line application in `\watermark-utility\bin\Debug\net5.0\`.

#### Running tests
- In Visual Studio, select the _Test_ menu item
- Select **Run All Tests**

#### Running tests from the Command Line
From the root of the repository
- `dotnet test .\watermark-utility-tests\WatermarkUtilityTests.csproj`

### Project Structure
- adapi-watermark
    - watermark-utility
        - watermark-utility.sln
        - Properties
            - launchSettings.json
    - watermark-utility-tests

#### watermark-utility
This is the project that contains the watermark-utility command line application.

#### watermark-utility-tests
This is the project that contains the tests for the watermark-utility command line application.
