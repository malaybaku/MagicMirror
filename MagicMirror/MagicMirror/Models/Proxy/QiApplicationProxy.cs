using System;

using Baku.LibqiDotNet;
using Baku.MagicMirror.Plugin;

namespace Baku.MagicMirror.Models
{
    internal class QiApplicationProxy : IQiApplicationProxy, IDisposable
    {
        private static QiApplicationProxy _instance;
        public static QiApplicationProxy Instance => _instance;

        public static void Initialize(QiApplication app)
        {
            _instance = new QiApplicationProxy(app);
        }

        public static bool IsInitialized => _instance != null;


        private QiApplicationProxy(QiApplication app)
        {
            _app = app;
            _isDisposed = false;
        }

        private QiApplication _app;
        private bool _isDisposed;


        internal void Reset(QiApplication app)
        {
            Dispose();

            _app = app;
            _isDisposed = false;
        }

        public void Run()
        {
            if(!_isDisposed)
            {
                _app.Run();
            }
        }
        public void Stop()
        {
            if(!_isDisposed)
            {
                _app.Stop();
            }
        }

        public void Dispose()
        {
            if(!_isDisposed)
            {
                _app.Stop();
                _app.Destroy();
                _isDisposed = true;
            }
        }

    }
}
