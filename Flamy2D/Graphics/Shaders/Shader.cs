using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flamy2D.Graphics.Shaders
{
    public abstract class Shader
    {
        protected ShaderType shaderType;

        protected string[] shaderSources;

        protected int internalShaderID = -1;

        public int ShaderId { get { return internalShaderID; } }

        protected Shader(ShaderType type, params string[] sources)
        {
            shaderType = type;
            shaderSources = sources;
        }

        public abstract Compile();

        public virtual void Dispose()
        {
            if (internalShaderID != -1)
                GL.DeleteShader(internalShaderID);

            internalShaderID = -1;
        }
    }
}
