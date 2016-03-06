using Flamy2D.Base;
using Flamy2D.Buffer;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;

namespace Flamy2D.Graphics
{
    public class SpriteBatch
    {
        private const int MAX_BATCHES = 64 * 32;

        private const int MAX_VERTICES = MAX_BATCHES * 4;
        private const int MAX_INDICES = MAX_BATCHES * 4;

        private Texture2D CurrentTexture;
        private Texture2D Dot;

        private Vertex2D[] Vertices;

        private int abo = -1;
        private GLBufferDynamic<Vertex2D> vbo;
        private GLBuffer<uint> ibo;

        private int vertexCount;
        private int indexCount;

        private Game game;

        private bool active;

        public SpriteBatch(Game game)
        {
            this.game = game;

            Dot = new Texture2D(TextureConfiguration.Nearest, 1, 1);
            Dot.SetData(new[] { Color4.White }, null, type: OpenTK.Graphics.OpenGL4.PixelType.Float);

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

        private static void SwapVec(ref Vector2 a, ref Vector2 b)
        {
            Vector2 temp = a;
            a = b;
            b = temp;
        }
    }
}
