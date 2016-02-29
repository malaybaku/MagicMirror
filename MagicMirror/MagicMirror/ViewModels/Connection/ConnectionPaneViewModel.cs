using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Microsoft.Expression.Interactivity.Core;

using Baku.MagicMirror.Plugin;

namespace Baku.MagicMirror.ViewModels
{
    internal class ConnectionPaneViewModel : DocumentPaneViewModel , IQiConnectionServiceProxy
    {
        public static string ConnectionPaneContentId { get; } = "7a8e17e9-e369-48fe-afd6-118e2ca126fa";

        public ConnectionPaneViewModel(MagicMirrorDockWindowViewModel parent) : base(parent)
        {
            Title = "Connect";
            ContentId = ConnectionPaneContentId;
            ConnectCommand = new ActionCommand(async () => await ConnectAsync(TargetAddress));

            Sessions = new ReadOnlyObservableCollection<IQiSessionProxy>(_iSessions);
            SelectedSessions = new ReadOnlyObservableCollection<IQiSessionProxy>(_iSelectedSessions);

            SyncObservableCollections();
        }

        private string _targetAddress;
        public string TargetAddress
        {
            get { return _targetAddress; }
            set { SetAndRaise(ref _targetAddress, value); }
        }

        //ブロック処理: AというURLに接続トライ(時間かかる)してる間に重複でAへ接続試行しないため
        private object _connectBlockerLock = new object();
        private List<string> _connectBlocker = new List<string>();

        public ICommand ConnectCommand { get; }

        internal async Task ConnectAsync(string address, bool activate=false)
        {
            string target = QiConnectionHelper.GetNormalizedUrl(address);

            if (string.IsNullOrWhiteSpace(address))
            {
                return;
            }

            var same = _sessions.FirstOrDefault(s => s.Url == target);
            if (same != null)
            {
                if(activate) same.IsCurrent = true;
                return;
            }

            lock (_connectBlockerLock)
            {
                if (_connectBlocker.Contains(target))
                {
                    return;
                }
                else
                {
                    _connectBlocker.Add(target);
                }
            }

            var res = await QiConnectionHelper.Instance.TryConnectAsync(target);
            if (!res.Success) return;

            var session = res.Session;
            session.PropertyChanged += OnSessionPropertyChanged;

            _sessions.Add(res.Session);
            if (activate) session.IsCurrent = true;

            lock (_connectBlockerLock)
            {
                _connectBlocker.Remove(target);
            }
        }

        public void CheckConnections()
        {
            foreach(var s in _selectedSessions)
            {
                if (!s.IsConnected)
                {
                    _selectedSessions.Remove(s);
                }
            }

            foreach(var s in _sessions)
            {
                if (!s.IsConnected)
                {
                    _sessions.Remove(s);
                }
            }
        }

        private void OnSessionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var session = sender as QiSessionViewModel;
            if (session == null) return;


            if (e.PropertyName == nameof(session.IsSelected))
            {
                if (session.IsSelected && !_selectedSessions.Contains(session))
                {
                    _selectedSessions.Add(session);
                }
                else if(!session.IsSelected && _selectedSessions.Contains(session))
                {
                    _selectedSessions.Remove(session);
                }
            }
            else if (e.PropertyName == nameof(session.IsCurrent))
            {
                if (session.IsCurrent)
                {
                    CurrentSession = session;
                    foreach(var s in _sessions)
                    {
                        if (s != session) s.IsCurrent = false;
                    }
                }

                if (!session.IsCurrent && CurrentSession == session)
                {
                    CurrentSession = null;
                }
            }
        }

        private readonly ObservableCollection<QiSessionViewModel> _sessions = new ObservableCollection<QiSessionViewModel>();
        private readonly ObservableCollection<QiSessionViewModel> _selectedSessions = new ObservableCollection<QiSessionViewModel>();
        private QiSessionViewModel _currentSession;
        public QiSessionViewModel CurrentSession
        {
            get { return _currentSession; }
            private set
            {
                if (_currentSession != value)
                {
                    _currentSession = value;
                    RaisePropertyChanged();
                }
            }
        }

        #region IQiConnectionServiceProxy 

        private readonly ObservableCollection<IQiSessionProxy> _iSessions = new ObservableCollection<IQiSessionProxy>();
        public ReadOnlyObservableCollection<IQiSessionProxy> Sessions { get; }

        private readonly ObservableCollection<IQiSessionProxy> _iSelectedSessions = new ObservableCollection<IQiSessionProxy>();
        public ReadOnlyObservableCollection<IQiSessionProxy> SelectedSessions { get; }

        IQiSessionProxy IQiConnectionServiceProxy.CurrentSession => CurrentSession;

        #endregion

        //OC<Interface>がOC<VM>と同期するように仕込む
        private void SyncObservableCollections()
        {
            _sessions.CollectionChanged += (_, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (var item in e.NewItems.OfType<IQiSessionProxy>())
                    {
                        _iSessions.Add(item);
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (var item in e.NewItems.OfType<IQiSessionProxy>().Where(i => _iSessions.Contains(i)))
                    {
                        _iSessions.Remove(item);
                    }
                }
            };
            _selectedSessions.CollectionChanged += (_, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (var item in e.NewItems.OfType<IQiSessionProxy>())
                    {
                        _iSelectedSessions.Add(item);
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (var item in e.NewItems.OfType<IQiSessionProxy>().Where(i => _iSessions.Contains(i)))
                    {
                        _iSelectedSessions.Remove(item);
                    }
                }
            };

            //FIXME: selectedのほうも同じように。
        }

    }
}
