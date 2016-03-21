using Flamy2D.Graphics;
using System;
using System.Drawing;

namespace Flamy2D.Fonts
{
    class GlyphInfo
    {
        public int AdvanceX, AdvanceY, HorizontalAdvance, XOffset, YOffset;
        public bool Render;
        public Rectangle Rectangle;
        public Texture2D Texture;
        public uint CharIndex;

        public GlyphInfo(Texture2D t, Rectangle r, int advX, int advY, int horzAdv, int xOff, int yOff, uint idx)
        {
            Texture = t;
            Rectangle = r;
            AdvanceX = advX;
            AdvanceY = advY;
            XOffset = xOff;
            HorizontalAdvance = horzAdv;
            YOffset = yOff;
           Render = true;
            CharIndex = idx;
        }

        public GlyphInfo(int advX, int advY, uint index)
        {
            Render = false;
            AdvanceX = advX;
            AdvanceY = advY;
            CharIndex = index;
        }
    }
}
