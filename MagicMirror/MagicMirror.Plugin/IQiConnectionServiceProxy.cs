using System.Collections.Generic;

namespace MagicMirror.Plugin
{
    /// <summary>アプリケーションから提供される外部への接続サービス提供元を定義します。</summary>
    public interface IQiConnectionServiceProxy
    {
        /// <summary>接続済みセッションの一覧を取得します。</summary>
        IReadOnlyList<IQiSessionProxy> Sessions { get; }

        /// <summary>明示的に選択されている場合はカレントのセッションを取得します。</summary>
        IQiSessionProxy CurrentSession { get; }

    }
}
