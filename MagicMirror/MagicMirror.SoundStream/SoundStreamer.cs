using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;

using Livet;
using Baku.MagicMirror.Plugin;


namespace Baku.MagicMirror.SoundStream
{
    //NOTE: メンバにGUIを持つが、実際はinterface実装 + ViewModelっぽく作ってVVMにする例

    [Export(typeof(IMagicMirrorPlugin))]
    public class SoundStreamer : ViewModel, IMagicMirrorPlugin
    {
        #region IMagicMirrorPlugin

        public Guid Guid { get; } = new Guid("31871630-8486-4b08-9bd2-7780b44b5f4b");

        public string EnglishDescription => "Download and Play sound from Robot in real-time";
        public string JapaneseDescription => "ロボットへの音声出力や音声入力を行えるようにします。";

        public string Name { get; } = "Sound";

        public UserControl GuiContent { get; private set; } 

        public bool EnableAfterInitialize => false;

        public void Disable()
        {
            StopUpStream();
            StopDownStream();
        }

        public void Enable()
        {
            //何もしない
        }

        public void Update()
        {
            //何もしない
        }

        public void Initialize(IQiConnectionServiceProxy connectionService)
        {
            _connectionService = connectionService;
            GuiContent = new SoundStreamerView() { DataContext = this };
        }

        #endregion

        private SoundUploader _soundUploader;
        private SoundDownloader _soundDownloader;

        private bool _isExecutingUpStream;
        public bool IsExecutingUpStream
        {
            get { return _isExecutingUpStream; }
            set
            {
                if (_isExecutingUpStream != value)
                {
                    _isExecutingUpStream = value;
                    RaisePropertyChanged();
                }
            }
        }
       
        private bool _isExecutingDownStream;
        public bool IsExecutingDownStream
        {
            get { return _isExecutingDownStream; }
            set
            {
                if (_isExecutingDownStream != value)
                {
                    _isExecutingDownStream = value;
                    RaisePropertyChanged();
                }
            }
        }

        public async void StartUpStream()
        {
            if (IsExecutingUpStream) return;
            IsExecutingUpStream = true;

            var uploader = new SoundUploader();
            bool successToInitialize = await uploader.TryInitializeAsync(_connectionService);
            if (!successToInitialize)
            {
                IsExecutingUpStream = false;
                return;
            }

            _soundUploader = uploader;
            await _soundUploader.Run();

            IsExecutingUpStream = false;
        }
        public void StopUpStream() => _soundUploader?.Dispose();

        public async void StartDownStream()
        {
            if (IsExecutingDownStream) return;
            IsExecutingDownStream = true;

            var downloader = new SoundDownloader();
            bool successToInitialize = await downloader.TryInitializeAsync(_connectionService);
            if (!successToInitialize)
            {
                IsExecutingDownStream = false;
                return;
            }

            _soundDownloader = downloader;
            await _soundUploader.Run();

            IsExecutingDownStream = false;
        }
        public void StopDownStream() => _soundDownloader?.Dispose();

        private IQiConnectionServiceProxy _connectionService;


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            StopUpStream();
            StopDownStream();
        }
    }
}
