using System.ComponentModel;
using System.Windows;

namespace Baku.MagicMirror.Models
{
    internal static class Helper
    {
        /// <summary>デザインモードかどうかを取得します。</summary>
        public static bool IsInDesignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject());
    }
}
