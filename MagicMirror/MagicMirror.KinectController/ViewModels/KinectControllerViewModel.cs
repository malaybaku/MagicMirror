using System.Linq;

using Microsoft.Kinect;
using Livet;
using Baku.MagicMirror.Kinect.Models;

namespace Baku.MagicMirror.Kinect.ViewModels
{
    internal class KinectControllerViewModel : ViewModel
    {
        public KinectControllerViewModel(KinectController parent)
        {
            _parent = parent;

            _kinectSensor = KinectSensor.GetDefault();
            _kinectSensor.IsAvailableChanged += (_, __) => IsKinectAvailable = _kinectSensor.IsAvailable;

            _bodyFrameReader = _kinectSensor.BodyFrameSource.OpenReader();
            _bodyFrameReader.FrameArrived += OnFrameArrived;

            _kinectSensor.Open();

            IsKinectAvailable = _kinectSensor.IsAvailable;
            BoneViewer = new BoneViewerViewModel(this);
        }

        private bool _isKinectAvailable;
        public bool IsKinectAvailable
        {
            get { return _isKinectAvailable; }
            set
            {
                if (_isKinectAvailable != value)
                {
                    _isKinectAvailable = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _isBodyIndexFixed;
        /// <summary><see cref="FixedBodyIndex"/>で指定したインデックスに対応する人だけを追尾するかどうかを取得、設定します。</summary>
        public bool IsBodyIndexFixed
        {
            get { return _isBodyIndexFixed; }
            set
            {
                if (_isBodyIndexFixed != value)
                {
                    _isBodyIndexFixed = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _fixedBodyIndex = 0;
        /// <summary><see cref="IsBodyIndexFixed"/>がtrueの場合、指定したインデックスに割り振られた人だけを追尾します。</summary>
        public int FixedBodyIndex
        {
            get { return _fixedBodyIndex; }
            set
            {
                if (_fixedBodyIndex != value)
                {
                    _fixedBodyIndex = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if(_isEnabled != value)
                {
                    _isEnabled = value;
                    _parent.IsEnabled = value;
                    RaisePropertyChanged();
                }
            }
        }

        public BoneViewerViewModel BoneViewer { get; }

        internal int DisplayWidth => _kinectSensor.DepthFrameSource.FrameDescription.Width;
        internal int DisplayHeight => _kinectSensor.DepthFrameSource.FrameDescription.Height;
        internal CoordinateMapper CoordinateMapper => _kinectSensor.CoordinateMapper;

        private readonly KinectController _parent;
        private readonly KinectSensor _kinectSensor;
        private readonly BodyFrameReader _bodyFrameReader;
        private Body[] _bodies;

        private void OnFrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame == null)
                {
                    return;
                }

                _bodies = _bodies ?? new Body[bodyFrame.BodyCount];

                bodyFrame.GetAndRefreshBodyData(_bodies);
            }

            var body = IsBodyIndexFixed ?
                _bodies[FixedBodyIndex] :
                _bodies.FirstOrDefault(b => b.IsTracked);

            if (body?.IsTracked == true) 
            {
                BoneViewer.Update(body);
                _parent.RobotAngles.SetAnglesFromBody(body);
                //ちょっと垂れ流し感が高すぎるので消しておく: オン/オフ制御をきっちりと。
                //_parent.SendPose();
            }

        }

    }
}
