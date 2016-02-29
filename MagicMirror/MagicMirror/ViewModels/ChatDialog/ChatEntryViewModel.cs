
namespace Baku.MagicMirror.ViewModels
{
    internal class ChatEntryViewModel : MagicMirrorViewModel
    {
        public ChatEntryViewModel(string talker, string sentence)
        {
            Talker = talker;
            Sentence = sentence;
        }

        /// <summary>発話者(人間/ロボット)</summary>
        public string Talker { get; }

        /// <summary>発話の本文</summary>
        public string Sentence { get; }
    }

    internal class RobotChatEntryViewModel : ChatEntryViewModel
    {
        public RobotChatEntryViewModel(string sentence) : base("Robot", sentence)
        {

        }
    }

    internal class HumanChatEntryViewModel : ChatEntryViewModel
    {
        public HumanChatEntryViewModel(string sentence, int recognitionPrecision = 100)
            : base("Human", sentence)
        {
            RecognitionPrecision = recognitionPrecision;
        }

        /// <summary>認識精度(0以上100以下)</summary>
        public int RecognitionPrecision { get; }
    }
}
