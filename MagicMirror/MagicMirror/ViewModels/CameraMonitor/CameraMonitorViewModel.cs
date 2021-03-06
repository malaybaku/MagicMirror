﻿using System;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Reactive.Linq;

using Baku.LibqiDotNet;
using Baku.MagicMirror.Models;
using Baku.MagicMirror.Plugin;
using Baku.MagicMirror.Views;

namespace Baku.MagicMirror.ViewModels
{
    [Export(typeof(IMagicMirrorPlugin))]
    internal class CameraMonitorViewModel : MagicMirrorViewModel, IMagicMirrorPlugin
    {
        public CameraMonitorViewModel()
        {
            //デフォ設定、まあこんなもんでしょう。
            SelectedCamera = CameraType.Top;
            SelectedColorSpace = CameraColorSpace.RGB;
            SelectedResolution = CameraResolution.VGA;
            SelectedFps = 1;
        }

        private IQiConnectionServiceProxy _connectionService;

        public CameraOptionsViewModel Options { get; } = new CameraOptionsViewModel();

        private BitmapSource _cameraImage;
        public BitmapSource CameraImage
        {
            get { return _cameraImage; }
            set
            {
                if(_cameraImage != value)
                {
                    _cameraImage = value;
                    RaisePropertyChanged();
                }
            }
        }

        private CameraType _selectedCamera;
        public CameraType SelectedCamera
        {
            get { return _selectedCamera; }
            set
            {
                if(_selectedCamera != value)
                {
                    _selectedCamera = value;
                    RaisePropertyChanged();
                }
            }
        }

        private CameraColorSpace _selectedColorSpace;
        public CameraColorSpace SelectedColorSpace
        {
            get { return _selectedColorSpace; }
            set
            {
                if (_selectedColorSpace != value)
                {
                    _selectedColorSpace = value;
                    RaisePropertyChanged();
                }
            }
        }

        private CameraResolution _selectedResolution;
        public CameraResolution SelectedResolution
        {
            get { return _selectedResolution; }
            set
            {
                if (_selectedResolution != value)
                {
                    _selectedResolution = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _selectedFps = 1;
        public int SelectedFps
        {
            get { return _selectedFps; }
            set { SetAndRaise(ref _selectedFps, value); }
        }

        private int _maxParallelDownload = 1;
        public int MaxParallelDownload
        {
            get { return _maxParallelDownload; }
            set { SetAndRaise(ref _maxParallelDownload, value); }
        }

        private IDisposable _monitorObserver;

        public async void StartMonitor()
        {
            if (_monitorObserver != null) return;

            await Task.Run(() =>
            {
                var vd = _connectionService
                    .CurrentSession?
                    .GetService(CameraMonitor.ALVideoDeviceServiceName);
                if (vd == null) return;

                //設定をそのまま入れて購読開始
                var subscribeName = new QiString(vd.Call(
                    CameraMonitor.SubcsribeMethodName,
                    new QiString(CameraMonitor.SubscriberDefaultName),
                    new QiInt32((int)SelectedCamera),
                    new QiInt32((int)SelectedResolution),
                    new QiInt32((int)SelectedColorSpace),
                    new QiInt32(SelectedFps)
                    ).GetString());

                //上の設定は途中でGUIから変更食らうのでコッチは固定するのがポイント
                var camType = SelectedCamera;
                var camRes = SelectedResolution;
                var camCs = SelectedColorSpace;

                double intervalSec = 1.0 / SelectedFps;

                int maxParallel = MaxParallelDownload;
                int parallelCount = 0;

                _monitorObserver = Observable.Timer(TimeSpan.FromSeconds(intervalSec), TimeSpan.FromSeconds(intervalSec))
                    .Subscribe(_ =>
                    {
                        if (parallelCount >= maxParallel) return;

                        var res = vd.Call(CameraMonitor.DownloadMethodName, subscribeName);
                        //通信途絶すると恐らくエラーを食らうハズ
                        if (res.ValueKind == QiValueKind.QiUnknown) return;

                        //※本来は初期設定時の値を使えるハズだが、変なタイミングで設定を変える可能性もゼロではないので
                        //いちおうColorSpaceとかバイト配列のサイズに関連する情報を確保する
                        int width = res[0].GetInt32();
                        int height = res[1].GetInt32();
                        int channel = res[2].GetInt32();
                        var cs = (CameraColorSpace)res[3].GetInt32();

                        byte[] data = res[6].GetRaw();

                        if (width * height * channel != data.Length) return;

                        CameraImage = CameraMonitor.GetFreezedImageFromBytes(data, camType, cs, camRes);
                    });
            });

        }

        public void ApplyMonitor()
        {
            StopMonitor();
            StartMonitor();
        }

        public void StopMonitor()
        {
            //すぐヌル代入することで変な競合を防ぐ意図
            _monitorObserver?.Dispose();
            _monitorObserver = null;
        }


        #region IMagicMirrorPlugin

        public string Name { get; } = "Monitor";

        public string JapaneseDescription { get; } = "カメラ情報をモニタリングします。";

        public string EnglishDescription { get; } = "Monitors Camera Information";

        public Guid Guid { get; } = new Guid("20de6d89-5c7d-433b-8313-092461dacf32");

        public UserControl GuiContent { get; private set; }

        public bool EnableAfterInitialize { get; } = false;

        public void Initialize(IQiConnectionServiceProxy connectionService)
        {
            _connectionService = connectionService;
            GuiContent = new CameraView { DataContext = this };
        }

        public void Enable()
        {

        }

        public void Update()
        {

        }

        public void Disable()
        {
            StopMonitor();
        }

        #endregion



    }
}
