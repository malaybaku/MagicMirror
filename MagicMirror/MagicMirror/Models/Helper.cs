using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace Baku.MagicMirror.Models
{
    internal static class Helper
    {
        /// <summary>デザインモードかどうかを取得します。</summary>
        public static bool IsInDesignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject());

        /// <summary>UIスレッドのディスパッチャです。</summary>
        public static Dispatcher UIDispatcher { get; set; }
    }
}
