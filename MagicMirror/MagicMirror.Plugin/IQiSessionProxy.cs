using System.Collections.Generic;

namespace MagicMirror.Plugin
{
    /// <summary>プラグインで利用可能な接続済セッションを定義します。</summary>
    public interface IQiSessionProxy
    {

        /// <summary>セッションが接続中であるかを取得します。</summary>
        bool IsConnected { get; }

        /// <summary>名前を指定してサービスをロード/取得します。</summary>
        /// <param name="name">サービス名</param>
        /// <returns>指定したサービス</returns>
        IQiServiceProxy GetService(string name);

        /// <summary>ロード済みのサービス一覧を取得します。</summary>
        IReadOnlyDictionary<string, IQiServiceProxy> Services { get; }

    }

}
