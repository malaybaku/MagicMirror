namespace Baku.MagicMirror.ViewModels
{
    internal class HelpDocumentViewModel : DocumentPaneViewModel
    {
        public HelpDocumentViewModel(MagicMirrorDockWindowViewModel parent) : base(parent)
        {
            Title = "Help";
            ContentId = HelpContentId;
        }

        public static string HelpContentId { get; } = "c040f813-be38-45f0-9381-5276a42dc6c7";
    }
}
