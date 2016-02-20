using Baku.LibqiDotNet;
using MagicMirror.Plugin;

namespace Baku.MagicMirror.ViewModels
{
    //なんかさあ、これ意味なくない？
    internal class QiServiceViewModel : MagicMirrorViewModel, IQiServiceProxy
    {

        public QiServiceViewModel(IQiServiceProxy service)
        {
            _service = service;
        }

        private readonly IQiServiceProxy _service;

        public string Name => _service.Name;

        public IQiSessionProxy Session => _service.Session;

        public IQiServiceInfoProxy ServiceInfo => _service.ServiceInfo;

        public QiValue Call(string methodName, params QiAnyValue[] args) => _service.Call(methodName, args);

        public int Post(string methodName, params QiAnyValue[] args) => _service.Post(methodName, args);


    }
}
