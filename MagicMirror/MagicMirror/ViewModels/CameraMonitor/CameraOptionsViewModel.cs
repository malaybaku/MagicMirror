using System;
using System.Collections.Generic;
using System.Linq;

using Baku.MagicMirror.Models;

namespace Baku.MagicMirror.ViewModels
{
    internal class CameraOptionsViewModel : MagicMirrorViewModel
    {
        public IList<CameraType> CameraTypes { get; } = Enum.GetValues(typeof(CameraType)).OfType<CameraType>().ToArray();
        public IList<CameraColorSpace> ColorSpaces { get; } = Enum.GetValues(typeof(CameraColorSpace)).OfType<CameraColorSpace>().ToArray();
        public IList<CameraResolution> Resolutions { get; } = Enum.GetValues(typeof(CameraResolution)).OfType<CameraResolution>().ToArray();
    }
}
