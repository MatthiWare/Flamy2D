using OpenTK.Graphics.OpenGL4;

namespace Flamy2D.Buffer
{
    public struct GLBufferSettings
    {

        public BufferTarget Target { get; set; }
        public BufferUsageHint Hint { get; set; }
        public int AttribSize { get; set; }
        public VertexAttribPointerType Type { get; set; }
        public bool Normalized { get; set; }
        public int Offset { get; set; }


    }
}
