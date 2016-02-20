using System.Collections.Generic;
using System.Linq;

using Baku.MagicMirror.Models;

namespace Baku.MagicMirror.ViewModels
{
    internal class MagicMirrorWindowViewModel : MagicMirrorViewModel
    {
        public MagicMirrorWindowViewModel()
        {
            _connectionService = new QiConnectionServiceViewModel();

            var pluginTabs = PluginLoader
                .Plugins
                .Select(p =>
                {
                    p.Initialize(_connectionService);
                    return new PluginTabViewModel(p);
                });

            TabItems = new TabItemViewModel[]
            {
                _connectionService,
                new ChatDialogViewModel(_connectionService),
                new CameraMonitorViewModel(_connectionService),
            }
                .Concat(pluginTabs)
                .ToArray();

            _selectedItem = TabItems.FirstOrDefault();
        }

        private readonly QiConnectionServiceViewModel _connectionService;

        public IList<TabItemViewModel> TabItems { get; }

        private TabItemViewModel _selectedItem;
        public TabItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if(_selectedItem != value)
                {
                    _selectedItem = value;
                    RaisePropertyChanged();
                }
            }
        }
       
    }
}
