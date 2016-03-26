using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flamy2D.Audio
{
    public class ALException : Exception
    {
        public ALError Error { get; private set; }

        public ALException(ALError e)
            : base(AL.GetErrorString(e))
        {
            Error = e;
        }
    }
}
