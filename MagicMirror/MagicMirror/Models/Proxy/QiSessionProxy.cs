using System;
using System.Collections.Generic;

using Baku.LibqiDotNet;
using MagicMirror.Plugin;

namespace Baku.MagicMirror.Models
{
    internal class QiSessionProxy : IQiSessionProxy, IDisposable
    {
        public QiSessionProxy(QiSession session)
        {
            _session = session;
            _services = new Dictionary<string, IQiServiceProxy>();

            Address = _session.GetUrl();
        }

        private QiSession _session;
        private Dictionary<string, IQiServiceProxy> _services;

        public string Address { get; }

        public bool IsConnected => _session?.IsConnected ?? false;

        public IReadOnlyDictionary<string, IQiServiceProxy> Services => _services;

        public IQiServiceProxy GetService(string name)
        {
            if (!IsConnected) return new NullQiServiceProxy();
            if (Services.ContainsKey(name)) return Services[name];

            try
            {
                //TODO: これさあ失敗したらどうなるんだっけか…
                var service = _session.GetService(name);

                var proxy = new QiServiceProxy(this, service, name);
                _services[name] = proxy;
                return Services[name];
            }
            catch (InvalidOperationException)
            {
                return new NullQiServiceProxy();
            }

        }

        public void Dispose()
        {
            _session.Close();
            _session.Destroy();
        }

    }

    /// <summary>無効なセッションを表します。</summary>
    internal class NullQiSessionProxy : IQiSessionProxy
    {
        public bool IsConnected => false;

        public IReadOnlyDictionary<string, IQiServiceProxy> Services { get; } = new Dictionary<string, IQiServiceProxy>();

        public IQiServiceProxy GetService(string name) => new NullQiServiceProxy();
    }

}
