using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

using Baku.MagicMirror.Plugin;

namespace Baku.MagicMirror.Models
{
    internal static class PluginLoader
    {
        public static IEnumerable<IMagicMirrorPlugin> Plugins
            => DirectoryPluginLoader.Plugins
                .Concat(ThisAssemblyPluginLoader.Plugins);

        /// <summary>外部で実装されたプラグインのロード機能を実装します。</summary>
        internal class DirectoryPluginLoader
        {
            private DirectoryPluginLoader() { }

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

                        var holder = new DirectoryPluginLoader();
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

        /// <summary>アセンブリ内部で実装されたプラグインのロード機能を実装します。</summary>
        internal class ThisAssemblyPluginLoader
        {
            private ThisAssemblyPluginLoader() { }

            private static IEnumerable<IMagicMirrorPlugin> _loadedPlugins;
            internal static IEnumerable<IMagicMirrorPlugin> Plugins
            {
                get
                {
                    if (_loadedPlugins == null)
                    {
                        var holder = new ThisAssemblyPluginLoader();
                        var catalog = new AssemblyCatalog(typeof(ThisAssemblyPluginLoader).Assembly);
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




}
