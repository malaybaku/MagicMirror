using Baku.LibqiDotNet;
using System.Collections.Generic;

namespace Baku.MagicMirror.Plugin
{
    /// <summary>サービスに関する情報を定義します。</summary>
    public interface IQiServiceInfoProxy
    {
        /// <summary>サービス名を取得します。</summary>
        string Name { get; }

        /// <summary>サービスに登録されたメソッドの一覧を取得します。</summary>
        IReadOnlyDictionary<string, QiMethodInfo> MethodInfo { get; }
    }
}
