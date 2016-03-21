using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flamy2D.Fonts
{
    struct CodepointerIterator
    {

        readonly string str;

        int strIndex;
        public int Index, Count;
        public uint Codepoint;

        public CodepointerIterator(string txt)
        {
            str = txt;
            Count = 0;
            for (int i = 0; i < str.Length; i++)
            {
                Count++;
                if (char.IsHighSurrogate(str, i))
                    i++;
            }

            Index = 0;
            Codepoint = 0;
            strIndex = 0;
        }

        public uint PeekNext()
        {
            return Index >= Count - 1 ? 0 : (uint)char.ConvertToUtf32(str, strIndex);
        }

        public bool Iterate()
        {
            if (Index >= Count)
                return false;
            Codepoint = (uint)char.ConvertToUtf32(str, strIndex);
            if (char.IsHighSurrogate(str, strIndex))
                strIndex++;
            strIndex++;
            Index++;
            return true;
        }
    }
}
