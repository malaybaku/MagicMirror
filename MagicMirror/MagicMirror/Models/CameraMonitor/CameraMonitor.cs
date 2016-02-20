using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Baku.MagicMirror.Models
{
    /// <summary>ALVideoDeviceのメソッドに関するユーティリティを実装します。</summary>
    internal static class CameraMonitor
    {
        public const string ALVideoDeviceServiceName = "ALVideoDevice";
        public const string SubcsribeMethodName = "subscribeCamera";
        public const string UnsubscribeMethodName = "unsubscribe";
        public const string DownloadMethodName = "getImageRemote";

        //購読時の登録名、正直何使っても問題なさそうではあるが。
        public const string SubscriberDefaultName = "MagicMirror";

        //描画結果のDPI: 描画サイズの決め方の事情から割と適当に決めても問題ない
        public static readonly double Dpi = 96.0;

        public static BitmapSource GetFreezedImageFromBytes(byte[] data, CameraType cameraType, CameraColorSpace colorSpace, CameraResolution resolution)
        {
            var size = GetSize(resolution);
            int channel = GetChannel(colorSpace);

            if (size.Width * size.Height * channel != data.Length)
            {
                return BitmapSource.Create(size.Width, size.Height, Dpi, Dpi, PixelFormats.Gray8, null, new byte[size.Width * size.Height], size.Width);
            } 

            if (channel == 1)
            {
                return GetSingleChanneledImage(data, size.Width, size.Height,
                    (colorSpace == CameraColorSpace.RinRGB ? ChannelSelection.Red : 
                     colorSpace == CameraColorSpace.GinRGB ? ChannelSelection.Green :
                     colorSpace == CameraColorSpace.BinRGB ? ChannelSelection.Blue :
                     ChannelSelection.Gray)
                    );
            }

            //ホントは2チャネルのケースって4/4/4/4とかだったりするのだけど面倒なので赤と青に振っておしまい
            if (channel == 2)
            {
                return GetTwoChanneledImage(data, size.Width, size.Height, ChannelSelection.Red, ChannelSelection.Blue);
            }

            if (channel == 3)
            {
                return GetThreeChanneledImage(data, size.Width, size.Height);
            }

            if (channel == 12)
            {
                Get12ChanneledImage(data, size.Width, size.Height);
            }


            throw new NotImplementedException();
        }



        private static ImageSize GetSize(CameraResolution resolution)
        {
            switch(resolution)
            {
                case CameraResolution.QQQQVGA:
                    return new ImageSize { Width = 40, Height = 30 };
                case CameraResolution.QQQVGA:
                    return new ImageSize { Width = 80, Height = 60 };
                case CameraResolution.QQVGA:
                    return new ImageSize { Width = 160, Height = 120 };
                case CameraResolution.QVGA:
                    return new ImageSize { Width = 320, Height = 240 };
                case CameraResolution.VGA:
                    return new ImageSize { Width = 640, Height = 480 };
                case CameraResolution.VGA4:
                    return new ImageSize { Width = 1280, Height = 960 };
                case CameraResolution.VGA16:
                    return new ImageSize { Width = 2560, Height = 1920 };
                default:
                    throw new InvalidOperationException();
            }
        }

        private static int GetChannel(CameraColorSpace colorSpace)
        {
            switch(colorSpace)
            {
                case CameraColorSpace.YinYUV:
                case CameraColorSpace.UinYUV:
                case CameraColorSpace.VinYUV:
                case CameraColorSpace.RinRGB:
                case CameraColorSpace.GinRGB:
                case CameraColorSpace.BinRGB:
                case CameraColorSpace.HinHSY:
                case CameraColorSpace.SinHSY:
                case CameraColorSpace.YinHSY:
                    return 1;
                case CameraColorSpace.YUV422:
                case CameraColorSpace.YYCbCr:
                case CameraColorSpace.Depth:
                case CameraColorSpace.Distance:
                case CameraColorSpace.RawDepth:
                    return 2;
                case CameraColorSpace.YUV:
                case CameraColorSpace.RGB:
                case CameraColorSpace.HSY:
                case CameraColorSpace.H2RGB:
                case CameraColorSpace.HSMixed:
                    return 3;
                case CameraColorSpace.XYZ:
                    return 12;
                default:
                    throw new InvalidOperationException();
            }
        }

        private static BitmapSource GetSingleChanneledImage(byte[] src, int width, int height, ChannelSelection cs)
        {
            var result = new WriteableBitmap(width, height, Dpi, Dpi, PixelFormats.Rgb24, null);

            var buf = new byte[width * height * 3];
            for (int i = 0; i < width * height; i++)
            {
                if (cs == ChannelSelection.Red || cs == ChannelSelection.Gray) buf[i * 3] = src[i];
                if (cs == ChannelSelection.Green || cs == ChannelSelection.Gray) buf[i * 3 + 1] = src[i];
                if (cs == ChannelSelection.Blue || cs == ChannelSelection.Gray) buf[i * 3 + 2] = src[i];
            }

            result.WritePixels(new Int32Rect(0, 0, width, height), buf, width, 0);

            result.Freeze();
            return result;
        }

        private static BitmapSource GetTwoChanneledImage(byte[] src, int width, int height, ChannelSelection cs1, ChannelSelection cs2)
        {
            var result = new WriteableBitmap(width, height, Dpi, Dpi, PixelFormats.Rgb24, null);

            var buf = new byte[width * height * 3];
            for (int i = 0; i < width * height; i++)
            {
                if (cs1 == ChannelSelection.Red) buf[i * 3] = src[i * 2];
                if (cs1 == ChannelSelection.Green) buf[i * 3 + 1] = src[i * 2];
                if (cs1 == ChannelSelection.Blue) buf[i * 3 + 2] = src[i * 2];

                if (cs1 == ChannelSelection.Red) buf[i * 3] = src[i * 2 + 1];
                if (cs1 == ChannelSelection.Green) buf[i * 3 + 1] = src[i * 2 + 1];
                if (cs1 == ChannelSelection.Blue) buf[i * 3 + 2] = src[i * 2 + 1];
            }

            result.WritePixels(new Int32Rect(0, 0, width, height), buf, width, 0);

            result.Freeze();
            return result;
        }

        private static BitmapSource GetThreeChanneledImage(byte[] src, int width, int height)
        {
            var res = BitmapSource.Create(width, height, Dpi, Dpi, PixelFormats.Rgb24, null, src, width * 3);
            res.Freeze();
            return res;
        }

        //ちょっと特殊: 1ピクセルにfloatでx, y, zの座標を表すボクセルが入ってるらしい
        private static BitmapSource Get12ChanneledImage(byte[] src, int width, int height)
        {
            //めんどくさいのでとりあえず対応しない(真っ黒の)方針で。データ潤沢に拾えるようになったらまた考えよう。
            var res = BitmapSource.Create(width, height, Dpi, Dpi, PixelFormats.Gray16, null, new byte[width * height * 2], width * 2);
            res.Freeze();
            return res;
        }

        struct ImageSize
        {
            public int Width;
            public int Height;
        }

        enum ChannelSelection
        {
            Red,
            Green,
            Blue,
            Gray
        }

    }




}
