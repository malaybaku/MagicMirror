
namespace Baku.MagicMirror.ViewModels
{
    internal class PaneViewModel : MagicMirrorViewModel
    {

        #region Title

        private string _title = null;
        public string Title
        {
            get { return _title; }
            protected set { SetAndRaise(ref _title, value); }
        }

        #endregion

        #region ContentId

        private string _contentId = null;
        public string ContentId
        {
            get { return _contentId; }
            protected set { SetAndRaise(ref _contentId, value); }
        }

        #endregion

        #region IsSelected

        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetAndRaise(ref _isSelected, value); }
        }

        #endregion

        #region IsActive

        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set { SetAndRaise(ref _isActive, value); }
        }

        #endregion

    }
}
