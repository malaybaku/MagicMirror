using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baku.MagicMirror.ViewModels
{
    internal class ChatEntryViewModel : MagicMirrorViewModel
    {
        public ChatEntryViewModel(string talker, string sentence, int recognitionPrecision =-1)
        {
            Talker = talker;
            Sentence = sentence;
            RecognitionPrecision = recognitionPrecision;
            HasRecognitionPrecision = (RecognitionPrecision >= 0);
        }

        /// <summary>発話者(人間/ロボット)</summary>
        public string Talker { get; }

        /// <summary>発話の本文</summary>
        public string Sentence { get; }

        /// <summary>認識精度(0以上100以下)</summary>
        public int RecognitionPrecision { get; }

        /// <summary>認識精度は値として使うべきかどうか</summary>
        public bool HasRecognitionPrecision { get; }
    }
}
