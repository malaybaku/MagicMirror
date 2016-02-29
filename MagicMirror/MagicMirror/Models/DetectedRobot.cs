using System.ComponentModel;

namespace Baku.MagicMirror.Models
{
    internal class DetectedRobot : INotifyPropertyChanged
    {
        internal DetectedRobot()
            : this("TestDisp", "TestHost", "255.255.255.255", false)
        { }

        internal DetectedRobot(string displayName, string hostName, string ipAddress, bool isRealRobot)
        {
            ItemNameDisplay = displayName;
            HostName = hostName;
            IpAddress = ipAddress;
            _isRealRobot = isRealRobot;
        }

        public string ItemNameDisplay { get; }
        public string HostName { get; }
        public string IpAddress { get; }

        private bool _isRealRobot;
        public bool IsRealRobot
        {
            get { return _isRealRobot; }
            set
            {
                if (_isRealRobot == value) return;
                _isRealRobot = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRealRobot)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override bool Equals(object obj)
            => (obj is DetectedRobot) &&
               this == (obj as DetectedRobot);

        public override int GetHashCode()
            => ItemNameDisplay.GetHashCode() +
               HostName.GetHashCode() +
               IpAddress.GetHashCode();

        public static bool operator ==(DetectedRobot r1, DetectedRobot r2)
            => r1.ItemNameDisplay == r2.ItemNameDisplay &&
               r1.HostName == r2.HostName &&
               r1.IpAddress == r2.IpAddress;

        public static bool operator !=(DetectedRobot r1, DetectedRobot r2)
            => !(r1 == r2);
    }

}
