using System;
using Baku.LibqiDotNet;

namespace Baku.MagicMirror.Plugin
{
    /// <summary>プラグインで利用可能な、qi Frameworkで取得したサービスを定義します。</summary>
    public interface IQiServiceProxy
    {
        /// <summary>サービスの名前を取得します。</summary>
        string Name { get; }

        /// <summary>このサービスに対応するセッションを取得します。</summary>
        IQiSessionProxy Session { get; }

        /// <summary>サービス情報を取得します。</summary>
        IQiServiceInfoProxy ServiceInfo { get; }
        
        /// <summary>関数を同期的に呼び出します。</summary>
        /// <param name="name">関数名</param>
        /// <param name="args">関数の引数</param>
        /// <returns>呼び出し結果</returns>
        QiValue Call(string name, params QiAnyValue[] args);

        /// <summary><see cref="Call(string, QiAnyValue[])"/>の中でも、特にオブジェクト型が戻り値であるようなものを呼び出す場合はこちらを使ってください。</summary>
        /// <param name="name">関数名</param>
        /// <param name="args">関数の引数</param>
        /// <returns>呼び出し結果</returns>
        QiObject CallObject(string name, params QiAnyValue[] args);

        /// <summary>シグナルにコールバック関数を登録します。</summary>
        /// <param name="signature">シグナルのシグネチャ</param>
        /// <param name="callback">登録するコールバック関数</param>
        void ConnectSignal(string signature, Action<QiValue> callback);

        /// <summary>シグナルのコールバック関数を登録削除します。</summary>
        /// <param name="callback">登録したコールバック関数</param>
        void DisconnectSignal(Action<QiValue> callback);

        /// <summary>関数を非同期的に呼び出します。</summary>
        /// <param name="name">関数名</param>
        /// <param name="args">関数の引数</param>
        /// <returns>非同期の待機状態を表すID</returns>
        int Post(string name, params QiAnyValue[] args);

    }
}
