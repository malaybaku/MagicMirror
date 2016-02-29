using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.Expression.Interactivity.Core;

using Baku.MagicMirror.Models;

namespace Baku.MagicMirror.ViewModels
{
    internal class MagicMirrorDockWindowViewModel : MagicMirrorViewModel
    {
        public MagicMirrorDockWindowViewModel()
        {
            ConnectionPane = new ConnectionPaneViewModel(this);
            ConnectionSearcher = new ConnectionSearcherViewModel(this);

            var tools = PluginLoader
                .Plugins
                .Select(p =>
                {
                    p.Initialize(ConnectionPane);
                    if (p.EnableAfterInitialize)
                    {
                        p.Enable();
                    }

                    return new ToolPaneViewModel(p);
                });
            _tools = new ObservableCollection<ToolPaneViewModel>(tools);

            _documents = new ObservableCollection<DocumentPaneViewModel>(
                new DocumentPaneViewModel[]
                {
                    new HomeDocumentViewModel(this)
                });

            Documents = new ReadOnlyObservableCollection<DocumentPaneViewModel>(_documents);
            Tools = new ReadOnlyObservableCollection<ToolPaneViewModel>(_tools);

        }

        internal ConnectionSearcherViewModel ConnectionSearcher { get; }
        internal ConnectionPaneViewModel ConnectionPane { get; }

        private ObservableCollection<DocumentPaneViewModel> _documents;
        public ReadOnlyObservableCollection<DocumentPaneViewModel> Documents { get; }

        private ObservableCollection<ToolPaneViewModel> _tools;
        public ReadOnlyObservableCollection<ToolPaneViewModel> Tools { get; }

        /// <summary>指定されたドキュメントをアクティベートします。既存のものが無い場合は生成処理も行います。</summary>
        /// <param name="doc"></param>
        public void AddAndShowDocument(DocumentPaneViewModel doc)
        {
            var target = _documents.FirstOrDefault(d => d.ContentId == doc.ContentId);
            if(target == null)
            {
                _documents.Add(doc);
                target = doc;
            }

            target.IsActive = true;
        }

        public void RemoveDocument(DocumentPaneViewModel doc)
        {
            var target = _documents.FirstOrDefault(d => d.ContentId == doc.ContentId);
            if(target != null)
            {
                _documents.Remove(target);
            } 
        }

        public void ShowHelp()
        {
            var target = Documents.FirstOrDefault(d => d.ContentId == HelpDocumentViewModel.HelpContentId);

            if(target == null)
            {
                target = new HelpDocumentViewModel(this);
                _documents.Add(target);
            }
            target.IsActive = true;
        }

        public void ShowConnectView()
        {
            if(!Documents.Contains(ConnectionPane))
            {
                _documents.Add(ConnectionPane);
            }
            ConnectionPane.IsActive = true;
        }

        public void ShowSearchView()
        {
            if (!Documents.Contains(ConnectionSearcher))
            {
                _documents.Add(ConnectionSearcher);
            }
            ConnectionSearcher.IsActive = true;
        }

        private ICommand _openConnectionViewCommand;
        public ICommand OpenConnectionViewCommand
            => _openConnectionViewCommand ?? (_openConnectionViewCommand = new ActionCommand(ShowConnectView));

        private ICommand _openSearchViewCommand;
        public ICommand OpenSearchViewCommand
            => _openSearchViewCommand ?? (_openSearchViewCommand = new ActionCommand(ShowSearchView));


        private ICommand _openHelpPaneCommand;
        public ICommand OpenHelpPaneCommand
            => _openHelpPaneCommand ?? (_openHelpPaneCommand = new ActionCommand(ShowHelp));

    }
}
