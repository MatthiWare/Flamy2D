using System;
using System.Linq;
using Flamy2D.Assets;
using Flamy2D.Graphics;
using OpenTK;
using OpenTK.Graphics;
using System.Collections.Generic;
using _Font = System.Drawing.Font;
using Gfx = System.Drawing.Graphics;
using System.Drawing;
using System.Drawing.Imaging;

namespace Flamy2D.Fonts
{
    public class Font : IAsset, IDisposable
    {
        private _Font font;

        private int LineHeight = 100;

        readonly Dictionary<char, GlyphInfo> glyphs = new Dictionary<char, GlyphInfo>();

        private Bitmap buffer;
        private Brush brush;

        public Font(string fontName, float size, FontStyle fs)
        {
            font = new _Font(fontName, size, fs);
            buffer = new Bitmap(1920, 1080);
            brush = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
        }

        //private Size MeasureString(string text)
        //{
        //    if (text == string.Empty)
        //        return Size.Empty;

        //    Size size = new Size();
        //    float top = 0, bottom = 0;

        //    for (int i = 0; i < text.Length; i++)
        //    {
        //        char c = text[i];

        //        uint index = face.GetCharIndex(c);

        //        face.LoadGlyph(index, LoadFlags.Default, LoadTarget.Normal);

        //        size.Width += (float)face.Glyph.Advance.X;

        //        if (face.HasKerning && i < text.Length - 1)
        //        {
        //            char cc = text[i + 1];
        //            size.Width += (float)face.GetKerning(index, face.GetCharIndex(cc), KerningMode.Default).X;
        //        }

        //        float top_ = (float)face.Glyph.Metrics.HorizontalBearingY;
        //        float bottom_ = (float)(face.Glyph.Metrics.Height - face.Glyph.Metrics.HorizontalBearingY);

        //        if (top_ > top)
        //            top = top_;

        //        if (bottom_ > bottom)
        //            bottom = bottom_;
        //    }

        //    size.Height = top + bottom;

        //    return size;
        //}

        //private void AddCharacter(uint c)
        //{
        //    if (c == (uint)'\t')
        //    {
        //        GlyphInfo space = GetGlyph((uint)' ');
        //        glyphs.Add(c, new GlyphInfo(space.AdvanceX * 5, space.AdvanceY, space.CharIndex));
        //        return;
        //    }

        //    uint index = face.GetCharIndex(c);
        //    if (index == 0)
        //        throw new Exception("undefined char code");

        //    face.LoadGlyph(index, LoadFlags.Default, LoadTarget.Normal);
        //    face.Glyph.RenderGlyph(RenderMode.Normal);

        //    if (face.Glyph.Bitmap.Width == 0 && face.Glyph.Bitmap.Rows == 0)
        //    {
        //        glyphs.Add(c, new GlyphInfo(
        //            (int)Math.Ceiling(face.Glyph.Advance.X.ToDecimal()),
        //            (int)Math.Ceiling(face.Glyph.Advance.Y.ToDecimal()),
        //            index));
        //        return;
        //    }

        //    RGBA[] pixels = new RGBA[face.Glyph.Bitmap.Width * face.Glyph.Bitmap.Rows];

        //    byte[] data = face.Glyph.Bitmap.BufferData;
        //    for (int i = 0; i < face.Glyph.Bitmap.Width * face.Glyph.Bitmap.Rows; i++)
        //    {
        //        pixels[i] = new RGBA(255, 255, 255, data[i]);
        //    }

        //    Rectangle r = new Rectangle(0, 0, face.Glyph.Bitmap.Width, face.Glyph.Bitmap.Rows);
        //    Texture2D tex = new Texture2D(TextureConfiguration.Linear, r.Width, r.Height);
        //    tex.SetData(pixels, r);

        //    GlyphInfo info = new GlyphInfo(tex, r,
        //        face.Glyph.Advance.X.Ceiling(),
        //        face.Glyph.Advance.Y.Ceiling(),
        //        face.Glyph.BitmapLeft, face.Glyph.BitmapTop);

        //    glyphs.Add(c, info);
        //}

        private void AddCharacter(char c)
        {
            if (c == '\t')
            {
                GlyphInfo space = GetGlyph(' ');
                glyphs.Add(c, new GlyphInfo(space.AdvanceX + 5, space.AdvanceY));

                return;
            }

            Gfx g = Gfx.FromImage(buffer);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.DrawString(c.ToString(), font, brush, 0, 0);

            SizeF size = g.MeasureString(c.ToString(), font);

            Rectangle r = new Rectangle(0, 0, (int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));

            RGBA[] pixels = new RGBA[r.Width * r.Height];

            int q = 0;
            for (int x = 0; x < r.Width; x++)
                for (int y = 0; y < r.Height; y++)
                {
                    var color = buffer.GetPixel(x, y);
                    pixels[q++] = new RGBA(255, 255, 255, color.A);
                }


            Texture2D tex = new Texture2D(TextureConfiguration.Linear, r.Width, r.Height);
            tex.SetData(pixels, null);

            GlyphInfo info = new GlyphInfo(tex, r, r.Width, 0, 0, 0);

            glyphs.Add(c, info);

            g.Clear(Color.Transparent);

            g.Dispose();
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

                if (glyph.Render)
                {
                    batch.Draw(
                            glyph.Texture,
                            glyph.Rectangle,
                            new Rectangle((int)penX + glyph.XOffset, (int)penY + (LineHeight - glyph.YOffset), glyph.Rectangle.Width, glyph.Rectangle.Height),
                            color);

                    penX += glyph.AdvanceX;
                }
                else
                {
                    penX += glyph.AdvanceX;
                }
            }

        }

        private GlyphInfo GetGlyph(char c)
        {
            if (!glyphs.ContainsKey(c))
                AddCharacter(c);

            return glyphs[c];
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    buffer.Dispose();
                    font.Dispose();
                    glyphs.Clear();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Font() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
