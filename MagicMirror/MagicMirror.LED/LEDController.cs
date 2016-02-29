using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;

using Livet;
using Baku.MagicMirror.Plugin;

namespace Baku.MagicMirror.LED
{
    [Export(typeof(IMagicMirrorPlugin))]
    public class LEDController : ViewModel, IMagicMirrorPlugin
    {
        #region IMagicMirrorPlugin

        public string Name { get; } = "LED";
        public string EnglishDescription => "Controls Eye LED Lights";
        public string JapaneseDescription => "LEDライトを操作します";

        public UserControl GuiContent { get; private set; }

        public Guid Guid { get; } = new Guid("a0ad1cd3-d524-4f8d-b819-d345eba20403");

        public bool EnableAfterInitialize => false;

        public void Initialize(IQiConnectionServiceProxy connectionService)
        {
            _connectionService = connectionService;
        }

        public void Enable()
        {

        }

        public void Update()
        {
            //何もしない
        }

        public void Disable()
        {

        }

        #endregion

        private IQiConnectionServiceProxy _connectionService;

        private ColorModel _L0 = ColorModel.Black;
        public ColorModel ColorL0
        {
            get { return _L0; }
            set
            {
                if(_L0 != value)
                {
                    _L0 = value;
                    RaisePropertyChanged();
                }
            }
        }

        public void ChangeRightSingle(string area)
        {

        }

        public void ChangeLeftSingle(string area)
        {

        }
    }

}
