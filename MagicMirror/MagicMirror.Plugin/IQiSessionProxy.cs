using System.Collections.Generic;
using Baku.LibqiDotNet;
using System.ComponentModel;

namespace Baku.MagicMirror.Plugin
{
    /// <summary>プラグインで利用可能な接続済セッションを定義します。</summary>
    public interface IQiSessionProxy : INotifyPropertyChanged
    {

        /// <summary>セッションの接続先であるURLを取得します。このURLはセッションを一意に識別します。</summary>
        string Url { get; }

        /// <summary>セッションが接続中であるかを取得します。</summary>
        bool IsConnected { get; }

        /// <summary>セッションがアプリケーション本体からの複数選択に含まれているかを取得します。</summary>
        bool IsSelected { get; }

        /// <summary>セッションがアプリケーション本体からアクティブなカレントセッションとして選択されているかを取得します。</summary>
        bool IsCurrent { get; }

        /// <summary>名前を指定してサービスをロード/取得します。</summary>
        /// <param name="name">サービス名</param>
        /// <returns>指定したサービス</returns>
        IQiServiceProxy GetService(string name);

        /// <summary>ロード済みのサービス一覧を取得します。</summary>
        IReadOnlyDictionary<string, IQiServiceProxy> Services { get; }

        /// <summary>
        /// 名前と実装を指定してセッションにサービスを登録、公開します。
        /// </summary>
        /// <param name="serviceName">サービス名</param>
        /// <param name="qiObject">サービスの実装となっているオブジェクト。このオブジェクトは<see cref="QiObjectBuilder.BuildObject"/>によって生成されたものであることが想定されています。</param>
        /// <returns>登録されたサービスに対応するID</returns>
        uint RegisterService(string serviceName, QiObject qiObject);

        /// <summary>サービスのIDを指定して登録を解除します。</summary>
        /// <param name="id">登録時に取得したサービスID</param>
        void UnregisterService(uint id);
    }

}
