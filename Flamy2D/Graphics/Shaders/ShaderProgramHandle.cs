using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flamy2D.Graphics.Shaders
{
    public class ShaderProgramHandle : IDisposable
    {
        readonly int previous;

        public ShaderProgramHandle(int prev)
        {
            previous = prev;
        }

        public void Dispose()
        {
            if (ShaderProgram.CurrentProgramId != previous)
            {
                ShaderProgram.CurrentProgramId = previous;
                GL.UseProgram(previous);
            }
        }
    }
}
