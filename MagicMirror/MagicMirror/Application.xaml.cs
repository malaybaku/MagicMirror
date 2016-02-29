using System;
using System.IO;
using System.Linq;
using System.Windows;

using Baku.MagicMirror.Views;

namespace Baku.MagicMirror
{
    public partial class Application
    {
        //実質エントリポイント
        protected override void OnStartup(StartupEventArgs e)
        {
            PathModifier.AddEnvironmentPaths(
                Path.Combine(Environment.CurrentDirectory, "qi_framework_dlls")
                );

            //MetroRadiance標準だとMainWindow閉じるだけではアプリケーション終了しない気がするんだけど仕様？
            MainWindow = new MagicMirrorDockWindow();
            MainWindow.Closed += (_, __) => Shutdown();
            MainWindow.Show();

            base.OnStartup(e);
        }


        static class PathModifier
        {
            public static void AddEnvironmentPaths(params string[] paths)
            {
                var path = new[] { Environment.GetEnvironmentVariable("PATH") ?? "" };

                string newPath = string.Join(Path.PathSeparator.ToString(), path.Concat(paths));

                Environment.SetEnvironmentVariable("PATH", newPath);
            }
        }

    }


}
