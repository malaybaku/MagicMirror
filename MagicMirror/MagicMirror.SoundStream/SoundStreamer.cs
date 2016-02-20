using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;

using MagicMirror.Plugin;

namespace MagicMirror.SoundStream
{
    //NOTE: メンバにGUIを持つが、実際はinterface実装 + ViewModelっぽく作ってVVMにする例

    [Export(typeof(IMagicMirrorPlugin))]
    public class SoundStreamer : IMagicMirrorPlugin
    {
        #region IMagicMirrorPlugin

        public Guid Guid { get; } = new Guid("31871630-8486-4b08-9bd2-7780b44b5f4b");

        public string EnglishDescription => "Download and Play sound from Robot in real-time";
        public string JapaneseDescription => "ロボットの音声をリアルタイムにダウンロード再生します";

        public string Name { get; } = "Sound";

        public UserControl GuiContent { get; private set; } 

        public bool EnableAfterInitialize => false;

        public void Disable()
        {
            //
        }

        public void Dispose()
        {

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

        //プレースホルダーとして(下記はXAMLから呼ばれる)

        public void StartStreaming()
        {
            MessageBox.Show("Test Start Streaming");
        }

        public void StopStreaming()
        {
            MessageBox.Show("Test Stop");
        }



        private IQiConnectionServiceProxy _connectionService;
    }
}
