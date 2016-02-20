using Baku.LibqiDotNet;

namespace MagicMirror.Plugin
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


        /// <summary>関数を非同期的に呼び出します。</summary>
        /// <param name="name">関数名</param>
        /// <param name="args">関数の引数</param>
        /// <returns>非同期の待機状態を表すID</returns>
        int Post(string name, params QiAnyValue[] args);

        //TODO:intをユーザに渡す設計は普通に考えてアンチ感高いのでどうにかしたい


        //TODO: 実際にはシグナル処理周りもどうにかしないとダメ
    }
}
