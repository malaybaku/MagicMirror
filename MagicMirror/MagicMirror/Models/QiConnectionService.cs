using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Baku.LibqiDotNet;
using MagicMirror.Plugin;
using System.Net;
using System.Text.RegularExpressions;

namespace Baku.MagicMirror.Models
{
    /// <summary>通信管理のコアを表します。</summary>
    internal class QiConnectionService
    {
        const string TcpPrefix = "tcp://";
        const string DefaultPortSuffix = ":9559";
        //接続先アドレス指定として妥当なパターンの2つのうちの一つ
        private static readonly Regex CorrectIpPattern1 = new Regex(@"^(tcp://)?\d+\.\d+\.\d+\.\d+(:\d+)?$");
        private static readonly Regex CorrectIpPattern2 = new Regex(@"^(tcp://)?[a-zA-Z]\w*\.[a-zA-Z]\w*(:\d+)?$");
        private static readonly Regex PortPattern = new Regex(@":\d+$");

        private QiConnectionService() { }

        /// <summary>外部からアクセス可能な唯一のインスタンスを取得します。</summary>
        public static QiConnectionService Instance { get; } = new QiConnectionService();

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

            //アドレスを最長のパターンに正規化
            string normalizedAddress = GetNormalizedUrl(address);
            if (string.IsNullOrEmpty(normalizedAddress))
            {
                return new TryConnectResult(normalizedAddress, false);
            }

            //ロード済みセッション
            if (Sessions.ContainsKey(normalizedAddress))
            {
                if (Sessions[normalizedAddress].IsConnected)
                {
                    return new TryConnectResult(normalizedAddress, true, true);
                }
                else
                {
                    //接続切れてたら除外+再試行
                    _qiSessions.Remove(normalizedAddress);
                }
            }

            return await Task.Run(() =>
            {
                //ここが重い
                var session = QiSession.Create(normalizedAddress);
                var res = session.IsConnected;

                if (res)
                {
                    string url = session.GetUrl();
                    _qiSessions.Add(url, new QiSessionProxy(session));
                    return new TryConnectResult(url, res);
                }
                else
                {
                    return new TryConnectResult(normalizedAddress, false);
                }

            });
        }

        /// <summary>非接続状態のセッションを外す</summary>
        public void ShrinkSessionsByConnection()
        {
            if (!IsInitialized) Initialize();

            _qiSessions = _qiSessions
                .Where(p => p.Value.IsConnected)
                .ToDictionary(p => p.Key, p => p.Value);
        }

        /// <summary>接続中であるはずのセッション一覧を取得します。タイミングによってはセッションが接続されなくなっている可能性もあります。</summary>
        public IReadOnlyDictionary<string, QiSessionProxy> Sessions => _qiSessions;
        private Dictionary<string, QiSessionProxy> _qiSessions = new Dictionary<string, QiSessionProxy>();
        
        public static string GetNormalizedUrl(string address)
        {
            //アドレスの有効性検証しつつ。
            if (!(CorrectIpPattern1.IsMatch(address) || CorrectIpPattern2.IsMatch(address)))
            {
                return "";
            }

            string result = address.Replace(TcpPrefix, "");
            //ポート番号無い場合は既定値の9559を追加
            if (!PortPattern.IsMatch(result))
            {
                result += DefaultPortSuffix;
            }
            return result;

        }

        internal class TryConnectResult
        {
            public TryConnectResult (string address, bool success, bool alreadyExists=false)
            {
                Address = address;
                Success = success;
                AlreadyExists = alreadyExists;
            }

            public string Address { get; }
            public bool Success { get; }
            public bool AlreadyExists { get; }
        }
    }


}
