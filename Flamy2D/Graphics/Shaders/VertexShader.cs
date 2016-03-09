namespace Flamy2D.Graphics.Shaders
{
    public class VertexShader : BasicShader
    {
        public VertexShader(params string[] sources)
                : base(OpenTK.Graphics.OpenGL4.ShaderType.VertexShader, sources)
        {

        }
    }
}
