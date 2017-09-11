using System.Drawing;

namespace Flamy2D.Fonts
{
    public struct Character
    {
        public int Channel;

        public Rectangle Bounds;

        public Point Offset;

        public char Char;

        public int TexturePage;

        public int XAdvance;
    }
}
