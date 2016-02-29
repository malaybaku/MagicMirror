using Baku.MagicMirror.Models;
using Microsoft.Expression.Interactivity.Core;
using System;
using System.Windows.Input;

namespace Baku.MagicMirror.ViewModels
{
    internal class DetectedRobotViewModel : MagicMirrorViewModel
    {
        internal DetectedRobotViewModel(DetectedRobot robot)
        {
            _robot = robot;
            ItemNameDisplay = _robot.ItemNameDisplay;
            HostName = _robot.HostName;
            IpAddress = _robot.IpAddress;
            IsRealRobot = _robot.IsRealRobot;

            _robot.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(_robot.IsRealRobot))
                {
                    IsRealRobot = _robot.IsRealRobot;
                }
            };
        }

        internal bool HasRobot(DetectedRobot robot) => _robot == robot;

        private readonly DetectedRobot _robot;

        public string ItemNameDisplay { get; }
        public string HostName { get; }
        public string IpAddress { get; }

        private bool _isRealRobot;
        public bool IsRealRobot
        {
            get { return _isRealRobot; }
            set { SetAndRaise(ref _isRealRobot, value); }
        }


        private ICommand _requestConnectCommand;
        public ICommand RequestConnectCommand
            => _requestConnectCommand ?? (_requestConnectCommand = new ActionCommand(RequestConnect));

        public void RequestConnect()
            => ConnectionRequested?.Invoke(this, new RequestConnectEventArgs(IpAddress));

        public event EventHandler<RequestConnectEventArgs> ConnectionRequested;

    }

    internal class RequestConnectEventArgs : EventArgs
    {
        public RequestConnectEventArgs(string ipAddress)
        {
            IpAddress = ipAddress;
        }
        
        public string IpAddress { get; }

    }

}
