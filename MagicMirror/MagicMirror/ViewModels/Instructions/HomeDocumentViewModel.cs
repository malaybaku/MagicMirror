using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Expression.Interactivity.Core;

namespace Baku.MagicMirror.ViewModels
{
    internal class HomeDocumentViewModel : DocumentPaneViewModel
    {
        public HomeDocumentViewModel(MagicMirrorDockWindowViewModel parent) : base(parent)
        {
            Title = "MagicMirror - Home";
            ContentId = "58250cfb-30e6-4a61-8b5d-2a8b18c3ee1d";

            Parent = parent;
            OpenProjectUrlCommand = new ActionCommand(() => Process.Start("https://github.com/malaybaku/MagicMirror"));
        }

        public MagicMirrorDockWindowViewModel Parent { get; }

        public ICommand OpenProjectUrlCommand { get; }

    }
}
