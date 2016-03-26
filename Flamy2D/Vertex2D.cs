using OpenTK;
using OpenTK.Graphics;

namespace Flamy2D
{
    public struct Vertex2D
    {
        public static readonly int Size;

        static Vertex2D()
        {
            Size = TypeHelper.SizeOf(typeof(Vertex2D));
        }

        public Vector3 Position { get; set; }

        public Vector2 TextureCoordinate { get; set; }

        public Color4 Color { get; set; }

        public Vertex2D(Vector3 pos, Vector2 text, Color4 color)
        {
            
            Position = pos;
            TextureCoordinate = text;
            Color = color;
        }
    }
}
