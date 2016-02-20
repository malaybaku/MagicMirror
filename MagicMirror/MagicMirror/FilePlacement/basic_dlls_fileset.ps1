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

#Other System|Microsoft dlls
New-Item DLLs\System -ItemType Directory
Move-Item ($TargetDir + "System*") DLLs\System\ -Force
Move-Item ($TargetDir + "Microsoft*") DLLs\System\ -Force

