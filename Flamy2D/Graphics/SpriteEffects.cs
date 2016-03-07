using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flamy2D.Graphics
{
    [Flags]
    public enum SpriteEffects
    {
        None = 0 << 1,
        FlipHorizontal = 1<<1,
        FlipVertical = 2<<1
    }
}
