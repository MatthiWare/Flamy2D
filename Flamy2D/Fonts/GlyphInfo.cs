using Flamy2D.Graphics;
using System;
using System.Drawing;

namespace Flamy2D.Fonts
{
    class GlyphInfo
    {
        public int AdvanceX, AdvanceY, XOffset, YOffset;
        public bool Render;
        public Rectangle Rectangle;
        public Texture2D Texture;

        public GlyphInfo(Texture2D t, Rectangle r, int advX, int advY, int xOff, int yOff)
        {
            Texture = t;
            Rectangle = r;
            AdvanceX = advX;
            AdvanceY = advY;
            XOffset = xOff;
            YOffset = yOff;
            Render = true;
        }

        public GlyphInfo(int advX, int advY)
        {
            Render = false;
            AdvanceX = advX;
            AdvanceY = advY;
        }
    }
}
