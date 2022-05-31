param ([string]$Path='C:\Program Files (x86)\Steam\steamapps\common\BloonsTD6')
[System.Environment]::SetEnvironmentVariable('BLOONSTD6_PATH',"$path", [System.EnvironmentVariableTarget]::User)
Write-Output $env:BLOONSTD6_PATH