function Pause()
{
    [System.Console]::Write("Press Any Key To Continue...")
    [System.Console]::ReadKey()
}

function RemoveDirectory([string]$dir)
{
    if([System.IO.Directory]::Exists($dir))
    {
        [System.IO.Directory]::Delete($dir,[bool]1)
    }
}

$inno = 'C:\Program Files (x86)\Inno Setup 6\ISCC.exe'
if(!([System.IO.File]::Exists($inno)))
{
    [System.Console]::ForegroundColor = [System.ConsoleColor]::Red
    [System.Console]::WriteLine('Inno Setup 6 Not Found !');
    [System.Console]::WriteLine('Download Here: https://jrsoftware.org/download.php/is.exe');
    Pause
    exit
}
$msbuild = &"${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe
RemoveDirectory("bin")
cd Source
$buildargs = 'Mosa.sln','/t:Build','/p:Configuration=Debug;Platform="Mixed Platforms"'
&$msbuild $buildargs
cd..
$innoargs = '/DMyAppVersion=2.0.0','Source\Inno-Setup-Script\Mosa-Installer.iss'
&$inno $innoargs
$output = 'bin\MOSA-Installer.exe'
$installargs = '/silent'
&$output $installargs