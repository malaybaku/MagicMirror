using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Baku.LibqiDotNet;

namespace Baku.MagicMirror.ViewModels
{
    internal class ChatDialogViewModel : TabItemViewModel
    {
        const string ALTextToSpeechServiceName = "ALTextToSpeech";
        const string SayMethodName = "say";

        //TODO: animated側は色々と未検証
        const string ALAnimatedSpeechServiceName = "ALAnimatedSay";
        const string AnimatedSayMethodName = "animatedSay";

        public ChatDialogViewModel(QiConnectionServiceViewModel connectionService) : base("チャット")
        {
            _connectionService = connectionService;
        }

        private readonly QiConnectionServiceViewModel _connectionService;

        private string _sentence;
        public string Sentence
        {
            get { return _sentence; }
            set { SetAndRaise(ref _sentence, value); }
        }


        public async void Say()
        {
            await Task.Run(() =>
            {
                var tts = _connectionService
                    .CurrentSession?
                    .GetService(ALTextToSpeechServiceName)
                    .Call(SayMethodName, new QiString(Sentence));
            });
        }

        public void AnimatedSay()
        {
            //何もしない: とりあえず未実装という体裁
        }

        public ObservableCollection<ChatEntryViewModel> ChatEntries { get; } = new ObservableCollection<ChatEntryViewModel>();

        //TODO:
        // - シグナルキャッチによるダイアログへのまともな表示
        // - 発話ロボットのIP表示する感じのオプション
        // - 音声認識キャッチ
        // - 


    }
}
