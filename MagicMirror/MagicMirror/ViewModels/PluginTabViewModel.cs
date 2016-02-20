using MagicMirror.Plugin;

namespace Baku.MagicMirror.ViewModels
{
    internal class PluginTabViewModel : TabItemViewModel
    {
        public PluginTabViewModel(IMagicMirrorPlugin plugin) : base(plugin.Name)
        {
            Plugin = plugin;
        }

        public IMagicMirrorPlugin Plugin { get; }

    }
}
