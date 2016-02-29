
namespace Baku.MagicMirror.LED
{
    /// <summary>0, .., 255 RGB color</summary>
    public class ColorModel
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public static ColorModel Gray => new ColorModel { R = 0x80, G = 0x80, B = 0x80 };
        public static ColorModel Black => new ColorModel();
    }
}
