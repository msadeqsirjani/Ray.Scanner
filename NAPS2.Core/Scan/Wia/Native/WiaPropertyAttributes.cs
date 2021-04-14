using System;
using System.Linq;

namespace NAPS2.Scan.Wia.Native
{
    public class WiaPropertyAttributes
    {
        public WiaPropertyAttributes(IntPtr storage, int id)
        {
            WiaException.Check(NativeWiaMethods.GetPropertyAttributes(storage, id, out var flags, out var min, out var nom, out var max, out var step, out _, out var elems));
            Flags = (WiaPropertyFlags)flags;
            Min = min;
            Nom = nom;
            Max = max;
            Step = step;
            Values = elems?.Skip(2).Cast<object>().ToArray();
        }

        public WiaPropertyFlags Flags { get; }

        public int Min { get; }

        public int Nom { get; }

        public int Max { get; }

        public int Step { get; }

        public object[] Values { get; }
    }
}