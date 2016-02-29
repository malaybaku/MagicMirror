#実行ディレクトリが散らかるの嫌いなので依存ライブラリを"DLLs"以下に押し込む。
# - NOTE: 参照ディレクトリはApp.configのprobing要素として追加されてるので
#         以下の移動先ディレクトリとApp.configは逐一対応を取る事

param(
    [string]$TargetDir
)

#Livet
New-Item DLLs\Livet -ItemType Directory
Move-Item ($TargetDir + "Livet*") DLLs\Livet\ -Force

#MetroRadiance
New-Item DLLs\MetroRadiance -ItemType Directory
Move-Item ($TargetDir + "MetroRadiance*") DLLs\MetroRadiance\ -Force

#FirstFloor ModernUI
New-Item DLLs\FirstFloor -ItemType Directory
Move-Item ($TargetDir + "FirstFloor.ModernUI.*") DLLs\FirstFloor\ -Force

#AvalonDock(VS2013 Theme)
#This theme is NOT official one, source is at https://github.com/4ux-nbIx/AvalonDock.Themes.VS2013
New-Item DLLs\AvalonDockTheme -ItemType Directory
Move-Item ($TargetDir + "Xceed.Wpf.AvalonDock.Themes.VS2013.*") DLLs\AvalonDockTheme\ -Force

#AvalonDock(Xceed, official package)
New-Item DLLs\Xceed -ItemType Directory
Move-Item ($TargetDir + "Xceed.Wpf.AvalonDock.*") DLLs\Xceed\ -Force

#UwpDesktop (*.winmd, Windows Matadata)
New-Item DLLs\winmds -ItemType Directory
Move-Item ($TargetDir + "*.winmd") DLLs\winmds\ -Force

#Other System|Microsoft dlls
New-Item DLLs\System -ItemType Directory
Move-Item ($TargetDir + "System*") DLLs\System\ -Force
Move-Item ($TargetDir + "Microsoft*") DLLs\System\ -Force

#Satellite Assemblies
New-Item DLLs\Satellites -ItemType Directory
Move-Item ($TargetDir + "de") DLLs\Satellites\ -Force
Move-Item ($TargetDir + "es") DLLs\Satellites\ -Force
Move-Item ($TargetDir + "fr") DLLs\Satellites\ -Force
Move-Item ($TargetDir + "hu") DLLs\Satellites\ -Force
Move-Item ($TargetDir + "it") DLLs\Satellites\ -Force
Move-Item ($TargetDir + "pt-BR") DLLs\Satellites\ -Force
Move-Item ($TargetDir + "ro") DLLs\Satellites\ -Force
Move-Item ($TargetDir + "ru") DLLs\Satellites\ -Force
Move-Item ($TargetDir + "sv") DLLs\Satellites\ -Force
Move-Item ($TargetDir + "zh-Hans") DLLs\Satellites\ -Force
