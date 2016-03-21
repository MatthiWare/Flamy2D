using System;
using SharpFont;
using Flamy2D.Assets;
using Flamy2D.Graphics;
using OpenTK;
using OpenTK.Graphics;
using System.Collections.Generic;
using System.Drawing;

namespace Flamy2D.Fonts
{
    public class Font : IAsset
    {
        private Library library;

        private Face face;
        private int LineHeight;

        readonly Dictionary<uint, GlyphInfo> glyphs = new Dictionary<uint, GlyphInfo>();

        static Font()
        {
            
        }

        public Font(string filename, float size)
        {
            library = new Library();
            face = new Face(library, filename);

            face.SetCharSize(Fixed26Dot6.FromSingle(0), Fixed26Dot6.FromSingle(size), 0, 96);
            LineHeight = face.Size.Metrics.Height.ToInt32();
        }

        private Size MeasureString(string text)
        {
            if (text == "")
                return new Size();

            Size size = new Size();
            float top = 0, bottom = 0;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                uint index = face.GetCharIndex(c);

                face.LoadGlyph(index, LoadFlags.Default, LoadTarget.Normal);

                size.Width += (float)face.Glyph.Advance.X;

                if (face.HasKerning && i < text.Length - 1)
                {
                    char cc = text[i + 1];
                    size.Width += (float)face.GetKerning(index, face.GetCharIndex(cc), KerningMode.Default).X;
                }

                float top_ = (float)face.Glyph.Metrics.HorizontalBearingY;
                float bottom_ = (float)(face.Glyph.Metrics.Height - face.Glyph.Metrics.HorizontalBearingY);

                if (top_ > top)
                    top = top_;

                if (bottom_ > bottom)
                    bottom = bottom_;
            }

            size.Height = top + bottom;

            return size;
        }

        private void AddCharacter(uint c)
        {
            if (c == (uint)'\t')
            {
                GlyphInfo space = GetGlyph((uint)' ');
                glyphs.Add(c, new GlyphInfo(space.AdvanceX * 5, space.AdvanceY, space.CharIndex));
                return;
            }

            uint index = face.GetCharIndex(c);
            if (index == 0)
                throw new Exception("undefined char code");

            face.LoadGlyph(index, LoadFlags.Default, LoadTarget.Normal);
            face.Glyph.RenderGlyph(RenderMode.Normal);

            if (face.Glyph.Bitmap.Width == 0 && face.Glyph.Bitmap.Rows == 0)
            {
                glyphs.Add(c, new GlyphInfo(
                    (int)Math.Ceiling(face.Glyph.Advance.X.ToDecimal()),
                    (int)Math.Ceiling(face.Glyph.Advance.Y.ToDecimal()),
                    index));
                return;
            }

            RGBA[] pixels = new RGBA[face.Glyph.Bitmap.Width * face.Glyph.Bitmap.Rows];

            byte[] data = face.Glyph.Bitmap.BufferData;
            for (int i = 0; i < face.Glyph.Bitmap.Width * face.Glyph.Bitmap.Rows; i++)
            {
                pixels[i] = new RGBA(255, 255, 255, data[i]);
            }

            Rectangle r = new Rectangle(0, 0, face.Glyph.Bitmap.Width, face.Glyph.Bitmap.Rows);
            Texture2D tex = new Texture2D(TextureConfiguration.Linear, r.Width, r.Height);
            tex.SetData(pixels, r);

            GlyphInfo info = new GlyphInfo(tex, r,
                face.Glyph.Advance.X.Ceiling(),
                face.Glyph.Advance.Y.Ceiling(),
                face.Glyph.Metrics.HorizontalAdvance.Ceiling(),
                face.Glyph.BitmapLeft, face.Glyph.BitmapTop, index);

            glyphs.Add(c, info);
        }

        public void DrawString(SpriteBatch batch, string text, Vector2 pos, Color4 color)
        {
            DrawString(batch, text, (int)pos.X, (int)pos.Y, color);
        }

        public void DrawString(SpriteBatch batch, string text, int x, int y, Color4 color)
        {
            if (text == "")
                return;

            float penX = x, penY = y;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (c == '\n')
                {
                    penY += LineHeight;
                    penX = x;
                    continue;
                }

                GlyphInfo glyph = GetGlyph(c);

                batch.Draw(
                        glyph.Texture,
                        glyph.Rectangle,
                        new Rectangle((int)penX + glyph.XOffset, (int)penY + (LineHeight - glyph.YOffset), glyph.Rectangle.Width, glyph.Rectangle.Height),
                        color);

                if (i < text.Length - 1)
                {
                    GlyphInfo g2 = GetGlyph(text[i + 1]);
                    float krn= face.GetKerning(glyph.CharIndex, g2.CharIndex, KerningMode.Default).X.ToSingle();
                    penX += krn;
                }
            }

        }

        private GlyphInfo GetGlyph(uint cp)
        {
            if (!glyphs.ContainsKey(cp))
                AddCharacter(cp);
            return glyphs[cp];
        }

        private GlyphInfo GetGlyph(char cp)
        {
            return GetGlyph((uint)cp);
        }

    }

    class Size
    {
        public float Width { get; set; }
        public float Height { get; set; }

        public Size()
        {
            Width = 0;
            Height = 0;
        }
    }
}
