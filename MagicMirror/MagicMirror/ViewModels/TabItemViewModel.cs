using MetroRadiance.UI.Controls;

namespace Baku.MagicMirror.ViewModels
{
    internal class TabItemViewModel : MagicMirrorViewModel, ITabItem
    {
        public TabItemViewModel(string name)
        {
            Name = name;
            Badge = null;
        }

        public int? Badge { get; private set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if(_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Name { get; }


    }
}
