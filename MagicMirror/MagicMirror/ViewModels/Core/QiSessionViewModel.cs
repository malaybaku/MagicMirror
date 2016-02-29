using System;
using System.Collections.Generic;
using System.ComponentModel;
using Baku.LibqiDotNet;
using Baku.MagicMirror.Plugin;

namespace Baku.MagicMirror.ViewModels
{
    internal class QiSessionViewModel : MagicMirrorViewModel, IQiSessionProxy, IDisposable
    {
        //デフォルトでセッションのサービスはどっからでも叩けることにする(ちょっと不健全)
        const string ListenTarget = "tcp://0.0.0.0:0";

        public QiSessionViewModel() : this(null)
        {
        }

        public QiSessionViewModel(QiSession session)
        {
            _session = session;
            Url = _session?.GetUrl() ?? "Unknown";
        }

        private readonly QiSession _session;
        private readonly Dictionary<string, IQiServiceProxy> _services = new Dictionary<string, IQiServiceProxy>();

        public string Url { get; }

        public bool IsConnected => _session?.IsConnected ?? false;

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetAndRaise(ref _isSelected, value); }
        }

        private bool _isCurrent;
        public bool IsCurrent
        {
            get { return _isCurrent; }
            set { SetAndRaise(ref _isCurrent, value); }
        }

        public bool IsListening { get; private set; }

        public IReadOnlyDictionary<string, IQiServiceProxy> Services => _services;

        public IQiServiceProxy GetService(string name)
        {
            if (!IsConnected) return new NullQiServiceProxy();
            if (Services.ContainsKey(name)) return Services[name];

            try
            {
                //TODO: これさあ失敗したらどうなるんだっけか…
                var serviceModel = _session.GetService(name);

                var service = new QiServiceViewModel(this, serviceModel, name);
                _services[name] = service;
                return Services[name];
            }
            catch (InvalidOperationException)
            {
                return new NullQiServiceProxy();
            }

        }

        void IDisposable.Dispose()
        {
            _session.Close();
            _session.Destroy();
        }

        public uint RegisterService(string serviceName, QiObject qiObject)
        {
            if (!IsConnected) return 0;

            if (!IsListening) _session.Listen(ListenTarget);

            return (uint)_session
                .RegisterService(serviceName, qiObject)
                .GetUInt64(0);
        }

        public void UnregisterService(uint id)
        {
            if(IsConnected)
            {
                _session.UnregisterService(id).Wait();
            }
        }

    }

    /// <summary>無効なセッションを表します。</summary>
    internal class NullQiSessionProxy : IQiSessionProxy
    {
        public bool IsConnected => false;

        public bool IsCurrent => false;

        public bool IsSelected => false;

        public IReadOnlyDictionary<string, IQiServiceProxy> Services { get; } = new Dictionary<string, IQiServiceProxy>();

        public string Url { get; } = "";

        public event PropertyChangedEventHandler PropertyChanged;

        public IQiServiceProxy GetService(string name) => new NullQiServiceProxy();

        public uint RegisterService(string serviceName, QiObject qiObject) => 0;

        public void UnregisterService(uint id) { }

    }

}
