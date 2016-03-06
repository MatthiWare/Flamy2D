using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;

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
            Bind(TextureUnit.Texture0);


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

            Unbind(TextureUnit.Texture0);

        }

        public void SetData(IntPtr data, Rectangle? rect, PixelFormat format = PixelFormat.Rgba, PixelType type = PixelType.UnsignedByte)
        {

        }

        private void Unbind(TextureUnit texture0)
        {
            GL.ActiveTexture(texture0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private void Bind(TextureUnit texture0)
        {
            GL.ActiveTexture(texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
        }
    }
}
