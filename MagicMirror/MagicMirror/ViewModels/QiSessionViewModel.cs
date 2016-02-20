using System;
using System.Collections.Generic;
using Baku.MagicMirror.Models;
using MagicMirror.Plugin;

namespace Baku.MagicMirror.ViewModels
{
    internal class QiSessionViewModel : MagicMirrorViewModel, IQiSessionProxy
    {
        public QiSessionViewModel(QiSessionProxy session)
        {
            _session = session;
            Address = session.Address;
        }
        private readonly QiSessionProxy _session;

        public string Address { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetAndRaise(ref _isSelected, value); }
        }

        public bool IsConnected => _session.IsConnected;

        public IReadOnlyDictionary<string, IQiServiceProxy> Services => _session.Services;

        public QiServiceViewModel GetService(string name)
            => new QiServiceViewModel(_session.GetService(name));

        IQiServiceProxy IQiSessionProxy.GetService(string name) => _session.GetService(name);
    }
}
