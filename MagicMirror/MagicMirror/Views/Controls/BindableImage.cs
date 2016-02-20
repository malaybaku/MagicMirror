using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;


namespace Baku.MagicMirror.Views.Controls
{
    public class BindableImage : Image
    {
        static BindableImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BindableImage), new FrameworkPropertyMetadata(typeof(BindableImage)));
        }

        #region 依存関係プロパティ

        public ImageSource BindableSource
        {
            get { return (ImageSource)GetValue(BindableSourceProperty); }
            set { SetValue(BindableSourceProperty, value); }
        }

        public static readonly DependencyProperty BindableSourceProperty
            = DependencyProperty.Register(
                nameof(BindableSource),
                typeof(ImageSource),
                typeof(BindableImage),
                new FrameworkPropertyMetadata(
                    null, FrameworkPropertyMetadataOptions.AffectsRender,
                    OnBindableSourceUpdate
                    )
                );

        private static void OnBindableSourceUpdate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as BindableImage)?.UpdateSource();
        }

        #endregion

        private void UpdateSource()
        {
            Source = BindableSource;
        }

    }
}
