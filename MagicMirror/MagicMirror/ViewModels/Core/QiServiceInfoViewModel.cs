using System.Collections.Generic;
using System.Linq;
using Baku.LibqiDotNet;

using Baku.MagicMirror.Plugin;

namespace Baku.MagicMirror.ViewModels
{
    /// <summary>サービス情報を表します。</summary>
    internal class QiServiceInfoViewModel : IQiServiceInfoProxy
    {
        public QiServiceInfoViewModel(QiServiceInfo info, string name)
        {
            _serviceInfo = info;
            Name = name;

            MethodInfo = _serviceInfo
                .MethodInfos
                .Values
                .ToDictionary(
                    //一意性取るためにフルシグネチャ使う
                    v => v.Name + QiSignatures.MethodNameSuffix + v.ReturnValueSignature + v.ArgumentSignature,
                    v => v
                );
        }

        private readonly QiServiceInfo _serviceInfo;

        public string Name { get; }

        public IReadOnlyDictionary<string, QiMethodInfo> MethodInfo { get; }
    }

    /// <summary>無効なサービス情報を表します。</summary>
    internal class NullQiServiceInfoProxy : IQiServiceInfoProxy
    {
        public string Name { get; } = "Null";

        public IReadOnlyDictionary<string, QiMethodInfo> MethodInfo { get; } = new Dictionary<string, QiMethodInfo>();
    }
}
