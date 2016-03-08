using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace Flamy2D.Graphics.Shaders
{
    public partial class ShaderProgram
    {
        Dictionary<string, int> attributes;

        public int Attrib(string attrib)
        {
            if (!attributes.ContainsKey(attrib))
                attributes.Add(attrib, GL.GetAttribLocation(ProgramId, attrib));

            return attributes[attrib];
        }
    }
}
