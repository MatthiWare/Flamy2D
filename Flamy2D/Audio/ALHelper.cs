using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flamy2D.Audio
{
    internal static class ALHelper
    {
        internal static readonly XRamExtension XRam = new XRamExtension();
        internal static readonly EffectsExtension Efx = new EffectsExtension();

        internal static void Check()
        {
            ALError error;
            if ((error = AL.GetError()) != ALError.NoError)
                throw new ALException(error);
        }
    }
}
