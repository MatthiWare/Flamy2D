using Flamy2D.Base;
using Flamy2D.Buffer;
using Flamy2D.Graphics.Shaders;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;

namespace Flamy2D.Graphics
{
    public partial class SpriteBatch
    {
        private const int MAX_BATCHES = 64 * 32;

        private const int MAX_VERTICES = MAX_BATCHES * 4;
        private const int MAX_INDICES = MAX_BATCHES * 6;

        private Texture2D CurrentTexture;
        private Texture2D Dot;

        private Vertex2D[] Vertices;

        private int abo = -1;
        private GLBufferDynamic<Vertex2D> vbo;
        private GLBuffer<uint> ibo;

        private ShaderProgram program;

        public Vector3 Position { get; set; }
        public Quaternion Orientation { get; set; }
        public Matrix4 ProjectionMatrix { get; set; }

        public Resolution Resolution { get; set; }

        public Matrix4 ViewMatrix
        {
            get
            {
                Matrix4 trans = Matrix4.CreateTranslation(-Position);
                Matrix4 orientation = Matrix4.CreateFromQuaternion(Orientation);

                return trans * orientation;
            }
        }

        public Matrix4 ViewProjectionMatrix
        {
            get
            {
                return ViewMatrix * ProjectionMatrix;
            }
        }

        public float FOV { get; set; }

        private Game game;

        private int vertexCount;
        private int indexCount;

        private bool active;

        public SpriteBatch(Game game, ShaderProgram shader = null)
        {
            this.game = game;

            FOV = 60f;

            Orientation = Quaternion.Identity;
            Position = Vector3.Zero;
            Resolution = game.Resolution;

            ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(0, Resolution.Width, Resolution.Height, 0, 0, 16);

            Dot = new Texture2D(TextureConfiguration.Nearest, 1, 1);
            Dot.SetData(new[] { Color4.White }, null, type: OpenTK.Graphics.OpenGL4.PixelType.Float);

            if (shader == null)
            {
                VertexShader vert = new VertexShader(vert_source);
                FragmentShader frag = new FragmentShader(frag_source);
                program = new ShaderProgram(vert, frag);
                program.Link();
            }
            else
                program = shader;


            GLBufferSettings settings = new GLBufferSettings
            {
                AttribSize = 0,
                Hint = BufferUsageHint.DynamicDraw,
                Normalized = false,
                Offset = 0,
                Target = BufferTarget.ArrayBuffer,
                Type = VertexAttribPointerType.Float
            };

            int index = 0;
            uint[] tempIndices = new uint[MAX_INDICES];

            for (uint i = 0; i < MAX_VERTICES; i += 4)
            {
                // triangle 1
                tempIndices[index++] = i;
                tempIndices[index++] = i + 1;
                tempIndices[index++] = i + 2;

                // triangle 2
                tempIndices[index++] = i + 1;
                tempIndices[index++] = i + 3;
                tempIndices[index++] = i + 2;
            }

            CurrentTexture = Dot;

            abo = GL.GenVertexArray();

            vbo = new GLBufferDynamic<Vertex2D>(settings, Vertex2D.Size, MAX_VERTICES);

            ibo = new GLBuffer<uint>(GLBufferSettings.DynamicIndices, tempIndices);

            Vertices = new Vertex2D[MAX_VERTICES];
        }

        public void Begin()
        {
            if (active)
                throw new InvalidOperationException("Cannot begin an active batch.");

            active = true;

            vertexCount = 0;
            indexCount = 0;
        }

        public void End()
        {
            if (!active)
                throw new InvalidOperationException("Cannot end an inactive batch.");

            Flush();

            active = false;
        }

       

        public void Draw(Texture2D tex, float x, float y, Color4 color)
        {
            Draw(tex, new Vector2(x, y), color, 1f);
        }

        public void Draw(Texture2D tex, float x, float y, Color4 color, float scale, float depth = 0, SpriteEffects effects = SpriteEffects.None)
        {
            Draw(tex, new Vector2(x, y), color, scale, depth, effects);
        }

        public void Draw(Texture2D tex, Vector2 pos, Color4 color, float scale, float depth=0, SpriteEffects effects = SpriteEffects.None)
        {
            Rectangle destRect = new Rectangle((int)pos.X, (int)pos.Y, tex.Width, tex.Height);
            Draw(tex, null, destRect, color, new Vector2(scale), depth, effects);
        }

        public void Draw(Texture2D tex, Rectangle? srcRect, Rectangle destRect, Color4 color, Vector2 scale, float depth = 0, SpriteEffects effects = SpriteEffects.None)
        {
            if (CurrentTexture.TextureId != -1 && tex.TextureId != CurrentTexture.TextureId)
                Flush();

            CurrentTexture = tex;

            if (indexCount + 6 >= MAX_INDICES || vertexCount + 4 >= MAX_VERTICES)
                Flush();

            Rectangle src = srcRect ?? tex.Bounds;

            float x = destRect.X;
            float y = destRect.Y;
            float w = destRect.Width * scale.X;
            float h = destRect.Height * scale.Y;

            float srcX = src.X;
            float srcY = src.Y;
            float srcW = src.Width;
            float srcH = src.Height;

            Vector2 topLeft = new Vector2(srcX / (float)tex.Width, srcY / (float)tex.Height);

            Vector2 topRight = new Vector2((srcX + srcW) / (float)tex.Width, srcY / (float)tex.Height);

            Vector2 bottomLeft = new Vector2(srcX / (float)tex.Width, (srcY + srcH) / (float)tex.Height);

            Vector2 bottomRight = new Vector2((srcX + srcW) / (float)tex.Width, (srcY + srcH) / (float)tex.Height);

            if (effects.HasFlag(SpriteEffects.FlipHorizontal))
            {
                SwapVec(ref topLeft, ref topRight);
                SwapVec(ref bottomLeft, ref bottomRight);
            }

            if (effects.HasFlag(SpriteEffects.FlipVertical))
            {
                SwapVec(ref topLeft, ref bottomLeft);
                SwapVec(ref topRight, ref bottomRight);
            }

            // Top left
            Vertices[vertexCount++] = new Vertex2D(
                pos: new Vector3(x, y, depth),
                text: topLeft,
                color: color
            );

            // Top right
            Vertices[vertexCount++] = new Vertex2D(
                pos: new Vector3(x + w, y, depth),
                text: topRight,
                color: color
            );

            // Bottom Left
            Vertices[vertexCount++] = new Vertex2D(
                pos: new Vector3(x, y + h, depth),
                text: bottomLeft,
                color: color
            );

            // Bottom Right
            Vertices[vertexCount++] = new Vertex2D(
                pos: new Vector3(x + w, y + h, depth),
                text: bottomRight,
                color: color
            );

            indexCount += 6;
        }

        //private void DrawInternal(Texture2D tex, Rectangle? srcRect, Rectangle destRect, Color4 color, Vector2 scale, float dx, float dy, float depth, float rot)
        //{
        //    if (CurrentTexture.TextureId != -1 && tex.TextureId != CurrentTexture.TextureId)
        //        Flush();

        //    CurrentTexture = tex;

        //    if (indexCount + 6 >= MAX_INDICES || vertexCount + 4 >= MAX_VERTICES)
        //        Flush();

        //    Rectangle src = srcRect ?? tex.Bounds;

        //    Quaternion quat = new Quaternion(rot, 0, 0);
        //    Vector2 pos = new Vector2(destRect.X, destRect.Y);
        //    Vector2 size = new Vector2(destRect.Width * scale.X, destRect.Height*scale.Y);

        //    float sin = (float)Math.Sin(rot);
        //    float cos = (float)Math.Sin(rot);

        //    float x = pos.X;
        //    float y = pos.Y;
        //    float w = size.X;
        //    float h = size.Y;

        //    // Top left
        //    Vertices[vertexCount++] = new Vertex2D(
        //        pos: new Vector3(x + dx * cos - dy * sin, y + dx * sin + dy * cos, depth),
        //        text: new Vector2(src.X / (float)tex.Width, src.Y / (float)tex.Height),
        //        color: color
        //    );

        //    // Top right
        //    Vertices[vertexCount++] = new Vertex2D(
        //        pos: new Vector3(
        //            x + (dx + w) * cos - dy * sin,
        //            y + (dx + w) * sin + dy * cos,
        //            depth
        //        ),
        //        text: new Vector2(
        //            (src.X + src.Width) / (float)tex.Width,
        //            src.Y / (float)tex.Height
        //        ),
        //        color: color
        //    );

        //    // Bottom Left
        //    Vertices[vertexCount++] = new Vertex2D(
        //        pos: new Vector3(
        //            x + dx * cos - (dy + h) * sin,
        //            y + dx * sin + (dy + h) * cos,
        //            depth
        //        ),
        //        text: new Vector2(
        //            src.X / (float)tex.Width,
        //            (src.Y + src.Height) / (float)tex.Height
        //        ),
        //        color: color
        //    );

        //    // Bottom Right
        //    Vertices[vertexCount++] = new Vertex2D(
        //        pos: new Vector3(
        //            x + (dx + w) * cos - (dy + h) * sin,
        //            y + (dx + w) * sin + (dy + h) * cos,
        //            depth
        //        ),
        //        text: new Vector2(
        //            (src.X + src.Width) / (float)tex.Width,
        //            (src.Y + src.Height) / (float)tex.Height
        //        ),
        //        color: color
        //    );

        //    indexCount += 6;

        //}

        public void Flush()
        {
            if (indexCount == 0)
                return;

            program.Use(() =>
            {
                GL.BindVertexArray(abo);

                vbo.UploadData(Vertices);

                vbo.PointTo(program.Attrib("v_pos"), 2, 0);
                vbo.PointTo(program.Attrib("v_tex"), 2, 3 * sizeof(float));
                vbo.PointTo(program.Attrib("v_col"), 4, 5 * sizeof(float));

                CurrentTexture.Bind();

                ibo.Bind();

                program["MVP"] = ViewProjectionMatrix;

                GL.DrawElements(BeginMode.Triangles, ibo.Buffer.Count, DrawElementsType.UnsignedInt, 0);

                GL.BindVertexArray(0);

                Array.Clear(Vertices, 0, Vertices.Length);

                vertexCount = 0;
                indexCount = 0;
            });
        }

        private static void SwapVec(ref Vector2 a, ref Vector2 b)
        {
            Vector2 temp = a;
            a = b;
            b = temp;
        }
    }
}
