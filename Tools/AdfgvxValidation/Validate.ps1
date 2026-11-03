param([string]$UnityEditor = 'C:/Program Files/Unity/Hub/Editor/2021.3.45f2/Editor')
$ErrorActionPreference = 'Stop'
$projectRoot = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$validationOutput = Join-Path $projectRoot 'output/adfgvx/validation'
New-Item -ItemType Directory -Path $validationOutput -Force | Out-Null
$enginePath = Join-Path $UnityEditor 'Data/Managed/UnityEngine'
$references = Get-ChildItem -LiteralPath $enginePath -Filter '*.dll' | ForEach-Object {
    '<Reference Include="' + $_.BaseName + '"><HintPath>' + $_.FullName + '</HintPath></Reference>'
}
foreach ($name in @('Unity.TextMeshPro', 'UnityEngine.UI')) {
    $references += '<Reference Include="' + $name + '"><HintPath>' + $projectRoot + '/Library/ScriptAssemblies/' + $name + '.dll</HintPath></Reference>'
}
$references += '<Reference Include="Newtonsoft.Json"><HintPath>' + $projectRoot + '/Library/PackageCache/com.unity.nuget.newtonsoft-json@3.2.1/Runtime/Newtonsoft.Json.dll</HintPath></Reference>'
$sources = @('Assets/Scripts/Encrypt/Remake/AdfgvxPuzzle.cs', 'Assets/Scripts/Encrypt/Remake/AdfgvxTestController.cs',
    'Assets/Scripts/Encrypt/Remake/AdfgvxLesson.cs', 'Assets/Scripts/Encrypt/Remake/AdfgvxTestTutorial.cs',
    'Assets/Scripts/Encrypt/Game/ADFGVXStageData.cs', 'Tools/AdfgvxValidation/Program.cs') | ForEach-Object {
    '<Compile Include="' + $projectRoot + '/' + $_ + '" />'
}
$definition = '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net9.0</TargetFramework><LangVersion>9.0</LangVersion><EnableDefaultCompileItems>false</EnableDefaultCompileItems><EnableNETAnalyzers>false</EnableNETAnalyzers><Nullable>disable</Nullable></PropertyGroup><ItemGroup>' + ($references -join "`n") + ($sources -join "`n") + '</ItemGroup></Project>'
$validationProject = Join-Path $validationOutput 'AdfgvxValidation.csproj'
Set-Content -LiteralPath $validationProject -Value $definition -Encoding UTF8
dotnet run --project $validationProject -- $projectRoot
if ($LASTEXITCODE -ne 0) { throw 'ADFGVX validation failed.' }
