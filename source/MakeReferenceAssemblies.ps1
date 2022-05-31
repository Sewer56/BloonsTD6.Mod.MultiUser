param ([string]$path='C:\Program Files (x86)\Steam\steamapps\common\BloonsTD6')

$paths = @( 
    ($path + '\MelonLoader\Managed\UnhollowerBaseLib.dll'),
    ($path + '\MelonLoader\Managed\Il2Cppmscorlib.dll'),
	($path + '\MelonLoader\Managed\NinjaKiwi.LiNK.dll'),
    ($path + '\MelonLoader\Managed\Il2CppNewtonsoft.Json.dll'),
    ($path + '\MelonLoader\MelonLoader.dll'),
    ($path + '\MelonLoader\0Harmony.dll'),
	($path + '\Mods\BloonsTD6 Mod Helper.dll')
)

function CopyAssembly {
    param ($Path, $OutFolder)
	Copy-Item -Path "$Path" -Destination "$OutFolder"
}

function MakeReferenceAssembly {
<#
.SYNOPSIS
    Creates a reference assembly for the given assembly and copies it to the given path.

.PARAMETER Path
    Path to the DLL to create a reference assembly for.
    
.PARAMETER OutFolder
    The output folder where the reference assembly should be placed.
#>

    param ($Path, $OutFolder)

    if (Test-Path -Path "$Path" -PathType Leaf) {
        Write-Output "[CREATING REFERENCE]: $Path"
        refasmer --all "$Path" -O "$OutFolder"
    }
    else {
        Write-Output "[DLL NOT FOUND]: $Path"
    }

    $xmlPath = [System.IO.Path]::ChangeExtension("$Path", '.xml');
    if (Test-Path -Path "$xmlPath" -PathType Leaf) {
        Write-Output "[COPYING DOCS]: $xmlPath"
        Copy-Item -Path "$xmlPath" -Destination "$OutFolder"
    }
}

# Install Reference Assembly Generator
dotnet tool install -g JetBrains.Refasmer.CliTool

$outFolder = $PSScriptRoot + '/Dependencies'
Write-Output "Base Path: $path"
Write-Output "Out Folder Path: $outFolder"

foreach ($assemblyPath in $paths) {
    MakeReferenceAssembly -Path $assemblyPath -OutFolder $outFolder
}