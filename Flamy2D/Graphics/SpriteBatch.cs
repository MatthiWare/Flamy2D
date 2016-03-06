using Flamy2D.Base;
using OpenTK;
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

        private int vertexCount;
        private int indexCount;

        private Game game;

        private bool active;

        public SpriteBatch(Game game)
        {
            this.game = game;

        }

        private static void SwapVec(ref Vector2 a, ref Vector2 b)
        {
            Vector2 temp = a;
            a = b;
            b = temp;
        }
    }
}
