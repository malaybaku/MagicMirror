#プラグインのうちソリューションに入ってるやつを該当する配置場所に移す

param(
    [string]$SolutionDir,
    [string]$TargetDir
)

#Plugin directories
New-Item Plugins -ItemType Directory
New-Item PluginDependency -ItemType Directory

$PluginDir = $TargetDir + "Plugins\"
$PluginDependencyDir = $TargetDir + "PluginDependency\"

#MagicMirror.SoundStream
$SStreamTargetDir = $SolutionDir + "MagicMirror.SoundStream\bin\x86\Debug\"

Copy-Item ($SStreamTargetDir + "MagicMirror.SoundStream*") $PluginDir -Force
Copy-Item ($SStreamTargetDir + "NAudio*") $PluginDependencyDir -Force

