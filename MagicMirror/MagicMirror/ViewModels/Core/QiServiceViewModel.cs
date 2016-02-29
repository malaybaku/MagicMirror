using System;
using Baku.LibqiDotNet;
using Baku.MagicMirror.Plugin;

namespace Baku.MagicMirror.ViewModels
{
    /// <summary>ロードされたサービスを表します。</summary>
    internal class QiServiceViewModel : MagicMirrorViewModel, IQiServiceProxy
    {
        public QiServiceViewModel(QiSessionViewModel session, QiObject service, string name)
        {
            _session = session;
            _service = service;

            ServiceInfo = new QiServiceInfoViewModel(service.ServiceInfo, name);
            Name = name;
        }

        private readonly QiSessionViewModel _session;
        private readonly QiObject _service;

        public string Name { get; }

        public IQiSessionProxy Session => _session;
        public IQiServiceInfoProxy ServiceInfo { get; }

        public QiValue Call(string methodName, params QiAnyValue[] args) => _service.Call(methodName, args);

        public int Post(string methodName, params QiAnyValue[] args) => _service.Post(methodName, args);

        public QiObject CallObject(string name, params QiAnyValue[] args) => _service.CallObject(name, args);

        public void ConnectSignal(string signature, Action<QiValue> callback) => _service.ConnectSignal(signature, callback);

        public void DisconnectSignal(Action<QiValue> callback) => _service.DisconnectSignal(callback);

    }

    /// <summary>無効なサービスを表します。</summary>
    internal class NullQiServiceProxy : IQiServiceProxy
    {
        public string Name { get; } = "Unknown";

        public IQiSessionProxy Session { get; } = new NullQiSessionProxy();

        public IQiServiceInfoProxy ServiceInfo { get; } = new NullQiServiceInfoProxy();

        public QiValue Call(string name, params QiAnyValue[] args) => QiValue.Void;

        public int Post(string name, params QiAnyValue[] args) => -1;

        public QiObject CallObject(string name, params QiAnyValue[] args)
        {
            throw new InvalidOperationException();
        }

        public void ConnectSignal(string signature, Action<QiValue> callback)
        {

        }

        public void DisconnectSignal(Action<QiValue> callback)
        {

        }

    }


}
