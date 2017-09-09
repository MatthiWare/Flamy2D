using Flamy2D.Assets;
using Flamy2D.Imaging;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace Flamy2D.Graphics
{
    public class Texture2D : IAsset, IDisposable
    {
        public int TextureId { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Rectangle Bounds { get { return new Rectangle(0, 0, Width, Height); } }

        public Texture2D(TextureConfiguration config, int widht, int height)
        {
            Width = widht;
            Height = height;

            // Get the texture ID
            TextureId = GL.GenTexture();

            // Bind the texture
            Bind(() =>
            {
                // Choose the filters
                TextureMinFilter minFilter = TextureMinFilter.Linear;
                TextureMagFilter magFilter = TextureMagFilter.Linear;

                switch (config.Interpolation)
                {
                    case InterpolationMode.Linear:
                        minFilter = config.Mipmap ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear;
                        break;
                    case InterpolationMode.Nearest:
                        minFilter = config.Mipmap ? TextureMinFilter.NearestMipmapNearest : TextureMinFilter.Nearest;
                        magFilter = TextureMagFilter.Nearest;
                        break;
                }

                // set the min filter parameter
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);

                // set the mag filter parameter
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);

                // create the texture
                GL.TexImage2D(
                    target: TextureTarget.Texture2D,
                    level: 0,
                    internalformat: PixelInternalFormat.Rgba,
                    width: Width,
                    height: Height,
                    border: 0,
                    format: PixelFormat.Bgra,
                    type: PixelType.UnsignedByte,
                    pixels: IntPtr.Zero
                );

                if (config.Mipmap)
                    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            });
        }

        public static async Task<Texture2D> LoadFromFileAsync(string path, TextureConfiguration config)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("File doesn't exist", path);

            return await Task.Run(() =>
            {
                int w = -1, h = -1, n = -1;
                IntPtr data = ImageLib.Load(path, ref w, ref h, ref n, 4);
                Texture2D tex = new Texture2D(config, w, h);
                tex.SetData(data, null);
                ImageLib.Free(data);

                return tex;
            });
        }

        public void SetData(IntPtr data, Rectangle? rect, PixelFormat format = PixelFormat.Rgba, PixelType type = PixelType.UnsignedByte)
        {
            Rectangle r = rect ?? Bounds;

            Bind(() =>
            {
                GL.TexSubImage2D(
                    target: TextureTarget.Texture2D,
                    level: 0,
                    xoffset: 0,
                    yoffset: 0,
                    width: r.Width,
                    height: r.Height,
                    format: format,
                    type: type,
                    pixels: data
                );
            });
        }

        public void SetData<T>(T[] data, Rectangle? rect, PixelFormat format = PixelFormat.Rgba, PixelType type = PixelType.UnsignedByte) where T : struct
        {
            Rectangle r = rect ?? Bounds;

            Bind(() =>
            {
                GL.TexSubImage2D(
                    target: TextureTarget.Texture2D,
                    level: 0,
                    xoffset: 0,
                    yoffset: 0,
                    width: r.Width,
                    height: r.Height,
                    format: format,
                    type: type,
                    pixels: data
                );
            });
        }

        public void Unbind(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Unbind()
        {
            Unbind(TextureUnit.Texture0);
        }

        public void Bind(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
        }

        public void Bind()
        {
            Bind(TextureUnit.Texture0);
        }

        public void Bind(Action action)
        {
            Bind();
            action();
            Unbind();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                Width = 0;
                Height = 0;
                TextureId = 0;

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Texture2D() {
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
