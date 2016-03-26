using OpenTK.Audio.OpenAL;
using System;

namespace Flamy2D.Audio
{
    [Serializable]
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
