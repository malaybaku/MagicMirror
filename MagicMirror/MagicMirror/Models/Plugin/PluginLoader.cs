using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

using MagicMirror.Plugin;

namespace Baku.MagicMirror.Models
{
    /// <summary>プラグインのロード機能を実装します。</summary>
    internal class PluginLoader
    {
        private PluginLoader() { }

        private const string PluginsDirectory = "Plugins";

        private static string PluginsAbsoluteDirectory
            => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), PluginsDirectory);

        private static IEnumerable<IMagicMirrorPlugin> _loadedPlugins;
        internal static IEnumerable<IMagicMirrorPlugin> Plugins
        {
            get
            {
                if (_loadedPlugins == null)
                {
                    Directory.CreateDirectory(PluginsAbsoluteDirectory);

                    var holder = new PluginLoader();
                    var catalog = new DirectoryCatalog(PluginsAbsoluteDirectory);
                    var container = new CompositionContainer(catalog);
                    container.ComposeParts(holder);

                    _loadedPlugins = holder._plugins;
                }
                return _loadedPlugins;
            }
        }

        [ImportMany]
        List<IMagicMirrorPlugin> _plugins = new List<IMagicMirrorPlugin>();

    }
}
