

using Microsoft.Expression.Interactivity.Core;
using System.Windows.Input;

namespace Baku.MagicMirror.ViewModels
{
    //NOTE: ホーム画面くらいにしか使わないのではないかコイツ。
    internal class DocumentPaneViewModel : PaneViewModel
    {
        public DocumentPaneViewModel(MagicMirrorDockWindowViewModel parent)
        {
            _parent = parent;
            OnCloseCommand = new ActionCommand(OnClose);
        }

        protected readonly MagicMirrorDockWindowViewModel _parent;

        public ICommand OnCloseCommand { get; }

        public void OnClose()
        {
            _parent.RemoveDocument(this);
        }
    }
}
