param([string]$UnityEditor = 'C:/Program Files/Unity/Hub/Editor/2021.3.45f2/Editor')
$ErrorActionPreference = 'Stop'
$projectRoot = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$validationOutput = Join-Path $projectRoot 'output/work-integration/validation'
New-Item -ItemType Directory -Path $validationOutput -Force | Out-Null
$enginePath = Join-Path $UnityEditor 'Data/Managed/UnityEngine'
$references = Get-ChildItem -LiteralPath $enginePath -Filter '*.dll' | ForEach-Object {
    '<Reference Include="' + $_.BaseName + '"><HintPath>' + $_.FullName + '</HintPath></Reference>'
}
foreach ($name in @('Unity.TextMeshPro', 'UnityEngine.UI', 'Assembly-CSharp')) {
    $references += '<Reference Include="' + $name + '"><HintPath>' + $projectRoot + '/Library/ScriptAssemblies/' + $name + '.dll</HintPath></Reference>'
}
$references += '<Reference Include="Newtonsoft.Json"><HintPath>' + $projectRoot + '/Library/PackageCache/com.unity.nuget.newtonsoft-json@3.2.1/Runtime/Newtonsoft.Json.dll</HintPath></Reference>'
# Compile every new host/view plus the real document remake and ADFGVX controllers.
# The existing runtime assembly supplies only unchanged game services/data dependencies.
$files = @('Assets/Scripts/GameSystem/GameService/Interface/IWorkExecutionService.cs',
    'Assets/Scripts/Encrypt/Game/ADFGVXStageData.cs', 'Tools/WorkIntegrationValidation/Program.cs')
foreach ($directory in @('Assets/Scripts/WorkIntegration', 'Assets/Scripts/Common_Documents', 'Assets/Scripts/Encrypt/Remake')) {
    $files += Get-ChildItem -LiteralPath (Join-Path $projectRoot $directory) -Filter '*.cs' | ForEach-Object { $_.FullName }
}
$sources = $files | ForEach-Object {
    $sourcePath = if ([System.IO.Path]::IsPathRooted($_)) { $_ } else { Join-Path $projectRoot $_ }
    '<Compile Include="' + $sourcePath + '" />'
}
$definition = '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net9.0</TargetFramework><LangVersion>9.0</LangVersion><EnableDefaultCompileItems>false</EnableDefaultCompileItems><EnableNETAnalyzers>false</EnableNETAnalyzers><NoWarn>0436</NoWarn><Nullable>disable</Nullable></PropertyGroup><ItemGroup>' + ($references -join "`n") + ($sources -join "`n") + '</ItemGroup></Project>'
$validationProject = Join-Path $validationOutput 'WorkIntegrationValidation.csproj'
Set-Content -LiteralPath $validationProject -Value $definition -Encoding UTF8
dotnet run --project $validationProject -- $projectRoot
if ($LASTEXITCODE -ne 0) { throw 'Work integration validation failed.' }
