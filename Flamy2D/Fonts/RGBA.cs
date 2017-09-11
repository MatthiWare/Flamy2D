using System;
using System.Diagnostics;

namespace Flamy2D.Fonts
{
    [DebuggerDisplay("RGBA: ({R},{G},{B},{A})")]
    struct RGBA
    {
        public byte R, G, B, A;

        public RGBA(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }


    }
}
