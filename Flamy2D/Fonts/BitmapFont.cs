using Flamy2D.Assets;
using Flamy2D.Graphics;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flamy2D.Fonts
{
    public class BitmapFont : IEnumerable<Character>, IAsset, IDisposable
    {
        #region Properties

        public int AlphaChannel { get; set; }

        public int BaseHeight { get; set; }

        public int BlueChannel { get; set; }

        public bool Bold { get; set; }

        public IDictionary<char, Character> Characters { get; set; }

        public string Charset { get; set; }

        public string FamilyName { get; set; }

        public int FontSize { get; set; }

        public int GreenChannel { get; set; }

        public bool Italic { get; set; }

        public IDictionary<Kerning, int> Kernings { get; set; }

        public int LineHeight { get; set; }

        public int OutlineSize { get; set; }

        public bool Packed { get; set; }

        public Padding Padding { get; set; }

        public Page[] Pages { get; set; }

        public int RedChannel { get; set; }

        public bool Smoothed { get; set; }

        public Point Spacing { get; set; }

        public int StretchedHeight { get; set; }

        public int SuperSampling { get; set; }

        public Size TextureSize { get; set; }

        public Character this[char character] => Characters[character];

        public IDictionary<int, Texture2D> Textures { get; set; }

        public bool Unicode { get; set; }

        #endregion

        public const int NoMaxWidth = -1;

        public BitmapFont() { }

        public int GetKerning(char previous, char current)
        {
            int result;

            var kerning = new Kerning(previous, current, 0);
            if (!Kernings.TryGetValue(kerning, out result))
                result = 0;

            return result;
        }

        public Size MeasureFont(string text) => MeasureFont(text, NoMaxWidth);

        public Size MeasureFont(string text, double maxWidth)
        {
            var normalizedText = NormalizeLineBreaks(text);
            List<int> lineHeights = new List<int>();
            int currentLineWidth = 0, currentLineHeight = LineHeight;
            int blockWidth = 0, blockHeight = 0;
            char previous = ' ';

            foreach (char c in normalizedText)
            {
                if (c == '\n')
                {
                    lineHeights.Add(currentLineHeight);
                    blockWidth = Math.Max(blockWidth, currentLineWidth);
                    currentLineWidth = 0;
                    currentLineHeight = LineHeight;
                    break;
                }

                var data = this[c];
                int width = data.XAdvance + GetKerning(previous, c);

                if (maxWidth != NoMaxWidth && currentLineWidth + width >= maxWidth)
                {
                    lineHeights.Add(currentLineHeight);
                    blockWidth = Math.Max(blockWidth, currentLineWidth);
                    currentLineWidth = 0;
                    currentLineHeight = LineHeight;
                }

                currentLineWidth += width;
                currentLineHeight = Math.Max(currentLineHeight, data.Bounds.Height + data.Offset.Y);
                previous = c;
            }

            if (currentLineHeight != 0)
                lineHeights.Add(currentLineHeight);

            for (int i = 0; i < lineHeights.Count - 1; i++)
                lineHeights[i] = LineHeight;

            foreach (int height in lineHeights)
                blockHeight += height;

            return new Size(Math.Max(currentLineWidth, blockWidth), blockHeight);
        }

        public string NormalizeLineBreaks(string s) => s.Replace("\r\n", "\n").Replace("\r", "\n");

        public void DrawString(SpriteBatch batch, string text, Vector2 pos, Color4 color)
        {
            DrawString(batch, text, (int)pos.X, (int)pos.Y, color);
        }

        public void DrawString(SpriteBatch batch, string text, int x, int y, Color4 color)
        {

        }

        public IEnumerator<Character> GetEnumerator()
        {
            foreach (var kvp in Characters)
                yield return kvp.Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Characters = null;
                    Kernings = null;

                    foreach (var pair in Textures)
                        pair.Value.Dispose();

                    Textures = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~BitmapFont() {
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
