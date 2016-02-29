using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Devices.Enumeration;

//reference (dns-sd for naoqi robots) 
//  http://qiita.com/bellx2/items/5c7649d830167e56e84f

namespace Baku.MagicMirror.Models
{
    internal class ConnectionSearcher
    {
        /// <summary>シングルトンインスタンスにアクセスします。</summary>
        public static ConnectionSearcher Instance { get; } = new ConnectionSearcher();

        /// <summary>サーチにより見つかっている接続可能なロボットの一覧です。</summary>
        public ObservableCollection<DetectedRobot> Robots { get; }

        public bool IsRunning { get; private set; }

        /// <summary>サーチを開始します。バッテリ制約など特殊な理由が無い限りアプリケーション終了までサーチを継続しても弊害はありません。</summary>
        public void Start()
        {
            if (IsRunning) return;
            IsRunning = true;

            _realRobotWatcher.Start();
            _generalRobotWatcher.Start();
        }

        /// <summary>サーチを停止します。</summary>
        public void Stop()
        {
            if (!IsRunning) return;
            IsRunning = false;

            _realRobotWatcher.Stop();
            _generalRobotWatcher.Stop();
        }

        /// <summary>実機ロボットだけ探す場合のサービス名</summary>
        const string realRobotServiceName = "_nao._tcp";
        /// <summary>実機/仮想ロボットの両方とも探す場合のサービス名</summary>
        const string generalRobotServiceName = "_naoqi._tcp";

        /// <summary>サービス表示名(PC名みたいなもん)をさすプロパティ名</summary>
        const string itemNameDisplayPropName = "System.ItemNameDisplay";
        /// <summary>自称のホスト名をさすプロパティ名</summary>
        const string hostNamePropName = "System.Devices.Dnssd.HostName";
        /// <summary>IPアドレスをさすプロパティ名</summary>
        const string ipAddressPropName = "System.Devices.IpAddress";

        private ConnectionSearcher()
        {
            Robots = _robots;

            _realRobotWatcher = CreateWatcher(realRobotServiceName);
            _realRobotWatcher.Added += (_, e) => OnRobotAdded(e.Properties, true);
            _realRobotWatcher.Removed += (_, e) => OnRobotRemoved(e.Properties);

            _generalRobotWatcher = CreateWatcher(generalRobotServiceName);
            _generalRobotWatcher.Added += (_, e) => OnRobotAdded(e.Properties, false);
            _generalRobotWatcher.Removed += (_, e) => OnRobotRemoved(e.Properties);
        }

        private readonly DeviceWatcher _realRobotWatcher;
        private readonly DeviceWatcher _generalRobotWatcher;

        private object _robotsLock = new object();
        private readonly ObservableCollection<DetectedRobot> _robots = new ObservableCollection<DetectedRobot>();

        private void OnRobotAdded(IReadOnlyDictionary<string, object> props, bool expectedToRealRobot)
        {
            string dname = props.ContainsKey(itemNameDisplayPropName) ? (string)props[itemNameDisplayPropName] : "Unknown";
            string hname = props.ContainsKey(hostNamePropName) ? (string)props[hostNamePropName] : "Unknown";
            string ip = props.ContainsKey(ipAddressPropName) ? (props[ipAddressPropName] as string[])[0] : "";

            AddOrUpdate(new DetectedRobot(dname, hname, ip, expectedToRealRobot));
        }

        private void OnRobotRemoved(IReadOnlyDictionary<string, object> props)
        {
            string dname = props.ContainsKey(itemNameDisplayPropName) ? (string)props[itemNameDisplayPropName] : "Unknown";
            string hname = props.ContainsKey(hostNamePropName) ? (string)props[hostNamePropName] : "Unknown";
            string ip = props.ContainsKey(ipAddressPropName) ? (props[ipAddressPropName] as string[])[0] : "";

            RemoveIfExists(new DetectedRobot(dname, hname, ip, false));
        }

        private void AddOrUpdate(DetectedRobot robot)
        {
            lock (_robotsLock)
            {
                if (!_robots.Any(r => r == robot))
                {
                    _robots.Add(robot);
                    return;
                }

                if (robot.IsRealRobot)
                {
                    var target = _robots.FirstOrDefault(r => r == robot);
                    target.IsRealRobot = true;
                }
            }
        }
        
        private void RemoveIfExists(DetectedRobot robot)
        {
            lock (_robotsLock)
            {
                if (!_robots.Any(r => r == robot)) return;

                var target = _robots.FirstOrDefault(r => r == robot);
                if (target != null)
                {
                    _robots.Remove(target);
                }
            }
        }

        private static DeviceWatcher CreateWatcher(string serviceName)
        {
            // DNS-SDを使うためのID
            var protocolId = "{4526e8c1-8aac-4153-9b16-55e86ada0e54}";
            // デバイス検索文字列
            var aqsFilter = $"System.Devices.AepService.ProtocolId:={protocolId} AND System.Devices.Dnssd.Domain:=\"local\" AND System.Devices.Dnssd.ServiceName:=\"{serviceName}\"";
            // 取得するプロパティ
            var properties = new[] {
                "System.Devices.Dnssd.HostName", // ホスト名 (自称)
                "System.Devices.IpAddress", // IPアドレスの配列(string[])
            };

            return DeviceInformation.CreateWatcher(aqsFilter, properties, DeviceInformationKind.AssociationEndpointService);
        }

    }

}
