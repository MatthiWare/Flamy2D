using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flamy2D.Graphics.Shaders
{
    public class FragmentShader : BasicShader
    {
        public FragmentShader(params string[] sources)
            : base(OpenTK.Graphics.OpenGL4.ShaderType.FragmentShader, sources)
        {

        }
    }
}
