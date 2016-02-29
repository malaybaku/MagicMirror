using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;

using Baku.LibqiDotNet;
using Baku.MagicMirror.Views;
using Baku.MagicMirror.Plugin;
using System.Windows.Input;
using Microsoft.Expression.Interactivity.Core;

namespace Baku.MagicMirror.ViewModels
{
    [Export(typeof(IMagicMirrorPlugin))]
    internal class ChatDialogViewModel : MagicMirrorViewModel, IMagicMirrorPlugin
    {
        const string ALTextToSpeechServiceName = "ALTextToSpeech";
        const string SayMethodName = "say";

        //TODO: animated側は色々と未検証
        const string ALAnimatedSpeechServiceName = "ALAnimatedSay";
        const string AnimatedSayMethodName = "animatedSay";

        const string SetLanguageMethodName = "setLanguage";

        #region IMagicMirrorPlugin

        public Guid Guid { get; } = new Guid("b4db3694-1a27-4755-8e3d-ba26af6afc1a");

        public string JapaneseDescription { get; } = "会話ログを表示します。発話の指示も行えます。";
        public string EnglishDescription { get; } = "Show Chat Dialog";
        public string Name { get; } = "チャット";

        public UserControl GuiContent { get; private set; }

        public void Initialize(IQiConnectionServiceProxy connectionService)
        {
            _connectionService = connectionService;
            _connectionService.PropertyChanged += OnCurrentSessionChanged;
            GuiContent = new ChatDialogView { DataContext = this };
        }

        public void Enable()
        {
        } 

        public void Update()
        {

        }

        public void Disable()
        {

        }

        public bool EnableAfterInitialize { get; } = false;


        #endregion

        public ChatDialogViewModel()
        {
            SayCommand = new ActionCommand(Say);
            ClearLogCommand = new ActionCommand(ChatEntries.Clear);
            SetLanguageToJapaneseCommand = new ActionCommand(() => SetLanguage("Japanese"));
            SetLanguageToEnglishCommand = new ActionCommand(() => SetLanguage("English"));
        }

        private IQiConnectionServiceProxy _connectionService;
        private IQiSessionProxy _currentSession;

        private string _sentence;
        public string Sentence
        {
            get { return _sentence; }
            set { SetAndRaise(ref _sentence, value); }
        }

        public ObservableCollection<ChatEntryViewModel> ChatEntries { get; } = new ObservableCollection<ChatEntryViewModel>();

        private async void Say()
        {
            if (string.IsNullOrWhiteSpace(Sentence)) return;

            await Task.Run(() =>
            {
                var tts = _connectionService
                    .CurrentSession?
                    .GetService(ALTextToSpeechServiceName);

                tts?.Call(SayMethodName, new QiString(Sentence));

                //成功したっぽい場合
                if (tts != null) Sentence = "";
            });
        }

        private void AnimatedSay()
        {
            //何もしない: とりあえず未実装という体裁
        }

        public ICommand SayCommand { get; }

        public ICommand ClearLogCommand { get; }

        public ICommand SetLanguageToJapaneseCommand { get; }

        public ICommand SetLanguageToEnglishCommand { get; }

        private void SetLanguage(string languageName)
        {
            _connectionService
                .CurrentSession?
                .GetService(ALTextToSpeechServiceName)
                .Call(SayMethodName, new QiString(Sentence));
        }

        private void OnCurrentSessionChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is IQiConnectionServiceProxy)) return;
            if (e.PropertyName != nameof(_connectionService.CurrentSession)) return;

            if (_currentSession == _connectionService.CurrentSession) return;

            if (_currentSession != null)
            {
                UnsubscribeDialogIO(_currentSession);
            }
            _currentSession = _connectionService.CurrentSession;
            if (_currentSession != null)
            {
                SubscribeDialogIO(_currentSession);
            }

        }

        private void SubscribeDialogIO(IQiSessionProxy session)
        {
            var mem = session.GetService("ALMemory");

            //人の会話検出
            var signalHuman = mem.CallObject("subscriber", new QiString("Dialog/LastInput"));
            signalHuman.ConnectSignal("signal", OnHumanSpeechDetected);
            _humanSignals[session.Url] = signalHuman;

            //ロボット側
            var signalRobot = mem.CallObject("subscriber", new QiString("ALTextToSpeech/CurrentSentence"));
            signalRobot.ConnectSignal("signal", OnRobotSpeechDetected);
            _robotSignals[session.Url] = signalRobot;
        }

        private void UnsubscribeDialogIO(IQiSessionProxy session)
        {
            if(_humanSignals.ContainsKey(session.Url))
            {
                _humanSignals[session.Url].DisconnectSignal(OnHumanSpeechDetected);
                _humanSignals.Remove(session.Url);
            }

            if(_robotSignals.ContainsKey(session.Url))
            {
                _robotSignals[session.Url].DisconnectSignal(OnRobotSpeechDetected);
                _robotSignals.Remove(session.Url);
            }

        }

        private void OnHumanSpeechDetected(QiValue qv)
        {
            if(qv.Count > 0 && qv[0].ContentValueKind == QiValueKind.QiString)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    ChatEntries.Add(new HumanChatEntryViewModel(qv[0].GetString()));
                });
            }
        }

        private void OnRobotSpeechDetected(QiValue qv)
        {
            if (qv.Count == 0 || qv[0].ContentValueKind != QiValueKind.QiString) return;

            string sentence = qv[0].GetString();
            if(!string.IsNullOrWhiteSpace(sentence))
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    ChatEntries.Add(new RobotChatEntryViewModel(sentence));
                });
            }
        }

        private readonly Dictionary<string, QiObject> _humanSignals = new Dictionary<string, QiObject>();
        private readonly Dictionary<string, QiObject> _robotSignals = new Dictionary<string, QiObject>();


    }
}
