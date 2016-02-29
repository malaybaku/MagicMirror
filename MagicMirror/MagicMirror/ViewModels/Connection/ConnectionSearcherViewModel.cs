using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Baku.MagicMirror.Models;

namespace Baku.MagicMirror.ViewModels
{
    internal class ConnectionSearcherViewModel : DocumentPaneViewModel
    {
        public static string SearcherPaneContentId { get; } = "3460b4ce-2de6-415e-bbcc-20a0e9fa77e7";

        public ConnectionSearcherViewModel(MagicMirrorDockWindowViewModel parent) : base(parent)
        {
            Title = "Search";
            ContentId = SearcherPaneContentId;

            Robots = new ObservableCollection<DetectedRobotViewModel>(
                ConnectionSearcher.Instance
                    .Robots
                    .Select(CreateDetectedRobotViewModel)
                );
            ConnectionSearcher.Instance.Robots.CollectionChanged += OnDetectedRobotChanged;
            ConnectionSearcher.Instance.Start();
        }

        public ObservableCollection<DetectedRobotViewModel> Robots { get; }

        private bool _activateRequestedSession;
        public bool ActivateRequestedSession
        {
            get { return _activateRequestedSession; }
            set { SetAndRaise(ref _activateRequestedSession, value); }
        }

        private void OnDetectedRobotChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //実装側がそういうシステムなので基本的にAdd/Removeのどちらか片方しか生じない
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var robot in e.NewItems.OfType<DetectedRobot>())
                {
                    Robots.Add(CreateDetectedRobotViewModel(robot));
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var robot in e.NewItems.OfType<DetectedRobot>())
                {
                    var target = Robots.FirstOrDefault(r => r.HasRobot(robot));
                    if (target != null)
                    {
                        Robots.Remove(target);
                    }
                }
            }
        }

        private DetectedRobotViewModel CreateDetectedRobotViewModel(DetectedRobot robot)
        {
            var result = new DetectedRobotViewModel(robot);
            result.ConnectionRequested += 
                async (_, e) => await _parent.ConnectionPane.ConnectAsync(e.IpAddress, ActivateRequestedSession);

            return result;
        }

    }
}
