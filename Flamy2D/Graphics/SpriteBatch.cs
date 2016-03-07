﻿using Flamy2D.Base;
using Flamy2D.Buffer;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;

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

        public void Draw(Texture2D tex, Rectangle? srcRect, Rectangle destRect, Color4 color, Vector2 origin, Vector2 scale, int depth = 0, float rot = 0)
        {
            DrawInternal(tex, srcRect, destRect, color, scale, -origin.X, -origin.Y, depth, rot);
        }

        public void Draw(Texture2D tex, Rectangle? srcRect, Vector2 pos, Color4 color, float scale, int depth = 0, float rot = 0)
        {
            Rectangle destRect = new Rectangle((int)pos.X, (int)pos.Y, srcRect.HasValue ? srcRect.Value.Width : tex.Width, srcRect.HasValue ? srcRect.Value.Height : tex.Height);
            DrawInternal(tex, srcRect, destRect, color, new Vector2(scale), 0, 0, depth, rot);
        }

        public void Draw(Texture2D tex, Rectangle? srcRect, Rectangle destRect, Color4 color, Vector2 origin, float scale, int depth = 0, float rot = 0)
        {
            DrawInternal(tex, srcRect, destRect, color, new Vector2(scale), -origin.X, -origin.Y, depth, rot);
        }

        public void Draw(Texture2D tex, Rectangle? srcRect, Vector2 pos, Color4 color, Vector2 origin, Vector2 scale, int depth = 0, float rot = 0)
        {
            Rectangle destRect = new Rectangle((int)pos.X, (int)pos.Y, srcRect.HasValue ? srcRect.Value.Width : tex.Width, srcRect.HasValue ? srcRect.Value.Height : tex.Height);
            Draw(tex, srcRect, destRect, color, origin, scale, depth, rot);
        }

        public void Draw(Texture2D tex, Vector2 pos, Color4 color, float scale, int depth = 0, float rot = 0)
        {
            Rectangle destRect = new Rectangle((int)pos.X, (int)pos.Y, tex.Width, tex.Height);

            DrawInternal(tex, null, destRect, color, new Vector2(scale), 0, 0, depth, rot);
        }

        public void Draw(Texture2D texture, Rectangle? sourceRect, Rectangle destRect, Color4 color, Vector2 scale, float depth = 0, SpriteEffects effects = SpriteEffects.None)

        private void DrawInternal(Texture2D tex, Rectangle? srcRect, Rectangle destRect, Color4 color, Vector2 scale, float dx, float dy, float depth, float rot)
        {
            if (CurrentTexture.TextureId != -1 && tex.TextureId != CurrentTexture.TextureId)
                Flush();

            CurrentTexture = tex;

            if (indexCount + 6 >= MAX_INDICES || vertexCount + 4 >= MAX_VERTICES)
                Flush();

            Rectangle src = srcRect ?? tex.Bounds;

            Quaternion quat = new Quaternion(rot, 0, 0);
            Vector2 pos = new Vector2(destRect.X, destRect.Y);
            Vector2 size = new Vector2(destRect.Width, destRect.Height);

            float sin = (float)Math.Sin(rot);
            float cos = (float)Math.Sin(rot);

            float x = pos.X;
            float y = pos.Y;
            float w = size.X;
            float h = size.Y;

            // Top left
            Vertices[vertexCount++] = new Vertex2D(
                pos: new Vector3(x + dx * cos - dy * sin, y + dx * sin + dy * cos, depth),
                text: new Vector2(src.X / (float)tex.Width, src.Y / (float)tex.Height),
                color: color
            );

            // Top right
            Vertices[vertexCount++] = new Vertex2D(
                pos: new Vector3(
                    x + (dx + w) * cos - dy * sin,
                    y + (dx + w) * sin + dy * cos,
                    depth
                ),
                text: new Vector2(
                    (src.X + src.Width) / (float)tex.Width,
                    src.Y / (float)tex.Height
                ),
                color: color
            );

            // Bottom Left
            Vertices[vertexCount++] = new Vertex2D(
                pos: new Vector3(
                    x + dx * cos - (dy + h) * sin,
                    y + dx * sin + (dy + h) * cos,
                    depth
                ),
                text: new Vector2(
                    src.X / (float)tex.Width,
                    (src.Y + src.Height) / (float)tex.Height
                ),
                color: color
            );

            // Bottom Right
            Vertices[vertexCount++] = new Vertex2D(
                pos: new Vector3(
                    x + (dx + w) * cos - (dy + h) * sin,
                    y + (dx + w) * sin + (dy + h) * cos,
                    depth
                ),
                text: new Vector2(
                    (src.X + src.Width) / (float)tex.Width,
                    (src.Y + src.Height) / (float)tex.Height
                ),
                color: color
            );

            indexCount += 6;

        }

        public void Flush()
        {
            if (indexCount == 0)
                return;

            GL.BindVertexArray(abo);

            vbo.UploadData(Vertices);

            CurrentTexture.Bind();

            ibo.Bind();

            GL.DrawElements(BeginMode.Triangles, ibo.Buffer.Count, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);

            Array.Clear(Vertices, 0, Vertices.Length);

            vertexCount = 0;
            indexCount = 0;
        }

        private static void SwapVec(ref Vector2 a, ref Vector2 b)
        {
            Vector2 temp = a;
            a = b;
            b = temp;
        }
    }
}
