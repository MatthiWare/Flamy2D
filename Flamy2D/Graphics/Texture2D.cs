using Flamy2D.Imaging;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;
using System.IO;

namespace Flamy2D.Graphics
{
    public class Texture2D
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
            Bind();
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
            }
            Unbind();
        }

        public static Texture2D LoadFromFile(string path, TextureConfiguration config)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("File doesn't exist", path);

            Texture2D tex;

            // Load the Image
            int w = -1, h = -1, n = -1;
            IntPtr data = ImageLib.Load(path, ref w, ref h, ref n, 4);
            tex = new Texture2D(config, w, h);
            tex.SetData(data, null);
            ImageLib.Free(data);
            
            return tex;
        }

        public void SetData(IntPtr data, Rectangle? rect, PixelFormat format = PixelFormat.Rgba, PixelType type = PixelType.UnsignedByte)
        {
            Rectangle r = rect ?? Bounds;

            Bind();
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
            }
            Unbind();
        }

        public void SetData<T>(T[] data, Rectangle? rect, PixelFormat format = PixelFormat.Rgba, PixelType type = PixelType.UnsignedByte) where T : struct
        {
            Rectangle r = rect ?? Bounds;

            Bind();
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
            }
            Unbind();
        }

        private void Unbind()
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private void Bind()
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
        }
    }
}
