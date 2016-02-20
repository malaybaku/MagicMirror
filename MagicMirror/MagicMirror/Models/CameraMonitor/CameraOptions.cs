namespace Baku.MagicMirror.Models
{
    internal enum CameraType
    {
        Top = 0,
        Bottom = 1,
        Depth = 2
    }

    internal enum CameraColorSpace
    {
        YinYUV = 0,
        UinYUV = 1,
        VinYUV = 2,
        RinRGB = 3,
        GinRGB = 4,
        BinRGB = 5,
        HinHSY = 6,
        SinHSY = 7,
        YinHSY = 8,
        YUV422 = 9,
        YUV = 10,
        RGB = 11,
        HSY = 12,
        BGR = 13,
        YYCbCr = 14,
        H2RGB = 15,
        HSMixed = 16,
        Depth = 17,
        XYZ = 19,
        Distance = 21,
        RawDepth = 23
    }

    internal enum CameraResolution
    {
        QQQQVGA = 8,
        QQQVGA = 7,
        QQVGA = 0,
        QVGA = 1,
        VGA = 2,
        VGA4 = 3,
        VGA16 = 4
    }
}
