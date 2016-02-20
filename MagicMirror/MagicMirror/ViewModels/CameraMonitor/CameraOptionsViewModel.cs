using Baku.MagicMirror.Models;
using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baku.MagicMirror.ViewModels
{

    internal class CameraOptionsViewModel : MagicMirrorViewModel
    {
        public  CameraOptionsViewModel()
        {
            CameraTypes = Enum.GetValues(typeof(CameraType)).OfType<CameraType>().ToArray();
            ColorSpaces = Enum.GetValues(typeof(CameraColorSpace)).OfType<CameraColorSpace>().ToArray();
            Resolutions = Enum.GetValues(typeof(CameraResolution)).OfType<CameraResolution>().ToArray();
        }

        public IList<CameraType> CameraTypes { get; }

        public IList<CameraColorSpace> ColorSpaces { get; }

        public IList<CameraResolution> Resolutions { get; }
    }
}
