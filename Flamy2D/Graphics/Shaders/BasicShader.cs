using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Flamy2D.Graphics.Shaders
{
    public class BasicShader : Shader
    {
        public BasicShader(ShaderType type, params string[] sources)
            : base(type, sources)
        {
            Compile();

        }

        public static Shader Create<Shader>(params string[] sources) where Shader : BasicShader
        {
            return (Shader)Activator.CreateInstance(typeof(Shader), sources);
        }

        public static Shader FromFile<Shader>(string path) where Shader : BasicShader
        {
            String fullPath = Path.GetFullPath(path);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException("Could not load shader, file not found", path);

            String source = File.ReadAllText(fullPath, Encoding.ASCII);

            return Create<Shader>(source);
        }

        public override void Compile()
        {
            internalShaderID = GL.CreateShader(shaderType);

            int[] lenghts = new int[shaderSources.Length];
            for (int i = 0; i < lenghts.Length; i++)
            {
                lenghts[i] = -1;
            }

            GL.ShaderSource(internalShaderID, lenghts.Length, shaderSources, ref lenghts[0]);

            GL.CompileShader(internalShaderID);

            int status;
            GL.GetShader(internalShaderID, ShaderParameter.CompileStatus, out status);

            if (status == 0)
            {
                String error = GL.GetShaderInfoLog(internalShaderID);

                throw new Exception(String.Format("Could not compile {0}: {1}", shaderType, error));
            }
        }
    }
}
