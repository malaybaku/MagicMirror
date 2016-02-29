using System.Windows;
using System.Windows.Controls;

using Baku.MagicMirror.ViewModels;

namespace Baku.MagicMirror.Views
{
    internal class PaneStyleSelector : StyleSelector
    {
        public Style ToolPaneStyle { get; set; }
        public Style DocumentPaneStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
            => (item is ToolPaneViewModel) ? ToolPaneStyle : 
               (item is DocumentPaneViewModel) ? DocumentPaneStyle : 
               base.SelectStyle(item, container);

    }

}
