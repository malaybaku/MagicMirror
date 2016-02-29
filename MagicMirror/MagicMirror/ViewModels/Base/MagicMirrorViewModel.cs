using System;
using System.Runtime.CompilerServices;
using Livet;

namespace Baku.MagicMirror.ViewModels
{
    internal class MagicMirrorViewModel : ViewModel
    {
        protected void SetAndRaise(ref string target, string value, [CallerMemberName]string pname = "")
        {
            if (target != value)
            {
                target = value;
                RaisePropertyChanged(pname);
            }
        }

        protected void SetAndRaise<T>(ref T target, T value, [CallerMemberName]string pname="")
            where T : struct, IEquatable<T>
        {
            if (!target.Equals(value))
            {
                target = value;
                RaisePropertyChanged(pname);
            }
        }


    }
}
