
using Baku.MagicMirror.Plugin;

namespace Baku.MagicMirror.ViewModels
{

    internal class ToolPaneViewModel : PaneViewModel
    {
        public ToolPaneViewModel(IMagicMirrorPlugin plugin)
        {
            Plugin = plugin;
            Title = Plugin.Name;
            ContentId = Plugin.Guid.ToString();
        }

        public IMagicMirrorPlugin Plugin { get; }

        #region IsVisible

        private bool _isVisible = true;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetAndRaise(ref _isVisible, value); }
        }

        #endregion

    }
}
