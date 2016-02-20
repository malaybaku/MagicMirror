using System;
using Baku.LibqiDotNet;
using MagicMirror.Plugin;

namespace Baku.MagicMirror.Models
{
    /// <summary>ロードされたサービスを表します。</summary>
    internal class QiServiceProxy : IQiServiceProxy
    {
        public QiServiceProxy(QiSessionProxy session, QiObject service, string name)
        {
            _session = session;
            _service = service;

            ServiceInfo = new QiServiceInfoProxy(service.ServiceInfo, name);
            Name = name;
        }

        private readonly QiSessionProxy _session;
        private readonly QiObject _service;

        public string Name { get; }

        public IQiSessionProxy Session => _session;
        public IQiServiceInfoProxy ServiceInfo { get; }

        public QiValue Call(string methodName, params QiAnyValue[] args) => _service.Call(methodName, args);

        public int Post(string methodName, params QiAnyValue[] args) => _service.Post(methodName, args);

    }

    /// <summary>無効なサービスを表します。</summary>
    internal class NullQiServiceProxy : IQiServiceProxy
    {
        public string Name { get; } = "Unknown";

        public IQiSessionProxy Session { get; } = new NullQiSessionProxy();

        public IQiServiceInfoProxy ServiceInfo { get; } = new NullQiServiceInfoProxy();

        public QiValue Call(string name, params QiAnyValue[] args) => QiValue.Void;

        public int Post(string name, params QiAnyValue[] args) => -1;

    }


}
