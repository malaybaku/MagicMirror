using System.Collections.Generic;
using System.Collections.ObjectModel;
using Baku.MagicMirror.Models;
using System.Linq;
using MagicMirror.Plugin;
using System;

namespace Baku.MagicMirror.ViewModels
{
    internal class QiConnectionServiceViewModel : TabItemViewModel, IQiConnectionServiceProxy
    {
        public QiConnectionServiceViewModel() : base("接続") { }

        //ビュー向け
        internal ObservableCollection<QiSessionViewModel> Sessions { get; } 
            = new ObservableCollection<QiSessionViewModel>();

        public QiSessionViewModel CurrentSession => Sessions.FirstOrDefault(s => s.IsSelected);

        private string _targetAddress;
        public string TargetAddress
        {
            get { return _targetAddress; }
            set { SetAndRaise(ref _targetAddress, value); }
        }

        IReadOnlyList<IQiSessionProxy> IQiConnectionServiceProxy.Sessions => Sessions;

        IQiSessionProxy IQiConnectionServiceProxy.CurrentSession
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        internal async void Connect()
        {
            if (string.IsNullOrWhiteSpace(TargetAddress)) return;

            var res = await QiConnectionService.Instance.TryConnectAsync(TargetAddress);
            if (!res.Success || res.AlreadyExists) return;


            //セッションが増えた場合は素直に追加
            var session = QiConnectionService.Instance.Sessions[res.Address];
            Sessions.Add(new QiSessionViewModel(session));
        }

        public void CheckConnections()
        {
            QiConnectionService.Instance.ShrinkSessionsByConnection();

            
        }
    }
}
