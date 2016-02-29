using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Baku.LibqiDotNet;
using Baku.MagicMirror.Models;

namespace Baku.MagicMirror.ViewModels
{
    /// <summary>接続処理の補助機能を実装します。</summary>
    internal class QiConnectionHelper
    {
        const string TcpPrefix = "tcp://";
        const string DefaultPortSuffix = ":9559";
        //接続先アドレス指定として妥当なパターンの2つのうちの一つ(※Pattern2だとグローバル名みたいのがあるロボットに対応しないけどとりあえずいいよね…?)
        private static readonly Regex CorrectIpPattern1 = new Regex(@"^(tcp://)?\d+\.\d+\.\d+\.\d+(:\d+)?$");
        private static readonly Regex CorrectIpPattern2 = new Regex(@"^(tcp://)?[a-zA-Z]\w*\.[a-zA-Z]\w*(:\d+)?$");
        private static readonly Regex PortPattern = new Regex(@":\d+$");

        private QiConnectionHelper() { }

        /// <summary>外部からアクセス可能な唯一のインスタンスを取得します。</summary>
        public static QiConnectionHelper Instance { get; } = new QiConnectionHelper();

        /// <summary>初期化を行います。</summary>
        public void Initialize()
        {
            if (IsInitialized || Helper.IsInDesignMode) return;

            QiApplicationProxy.Initialize(QiApplication.Create());
            IsInitialized = true;
        }

        /// <summary>インスタンスが初期化済みかどうかを取得します。</summary>
        public bool IsInitialized { get; private set; }

        /// <summary>指定したアドレスへ接続を試みる。</summary>
        /// <param name="address">接続先(例:"127.0.0.1", "tcp://pepper.local", "tcp://192.168.3.3:9559")</param>
        /// <returns>接続成功した場合はtrue</returns>
        public async Task<TryConnectResult> TryConnectAsync(string address)
        {
            //未初期化対策
            if (!IsInitialized) Initialize();

            //アドレスを正規化
            string normalizedAddress = GetNormalizedUrl(address);
            if (string.IsNullOrEmpty(normalizedAddress))
            {
                return new TryConnectResult(false, null);
            }

            return await Task.Run(() =>
            {
                //ここが重い
                var session = QiSession.Create(normalizedAddress);
                if (!session.IsConnected)
                {
                    return new TryConnectResult(false, null);
                }

                return new TryConnectResult(true, new QiSessionViewModel(session));

            });
        }
  
        public static string GetNormalizedUrl(string address)
        {
            //アドレスの有効性検証しつつ。
            if (!(CorrectIpPattern1.IsMatch(address) || CorrectIpPattern2.IsMatch(address)))
            {
                return "";
            }

            string result = TcpPrefix + address.Replace(TcpPrefix, "");
            //ポート番号無い場合は既定値の9559を追加
            if (!PortPattern.IsMatch(result))
            {
                result += DefaultPortSuffix;
            }
            return result;

        }

        internal class TryConnectResult
        {
            internal TryConnectResult (bool success, QiSessionViewModel session)
            {
                Success = success;
                Session = session;
            }

            /// <summary>接続に成功したかを取得します。</summary>
            public bool Success { get; }

            /// <summary><see cref="Success"/>がtrueの場合、接続結果のセッションを取得します。</summary>
            public QiSessionViewModel Session { get; }
        }
    }


}
