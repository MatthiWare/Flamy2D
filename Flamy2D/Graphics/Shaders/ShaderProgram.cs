using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace Flamy2D.Graphics.Shaders
{
    public partial class ShaderProgram : IDisposable
    {
        public static int CurrentProgramId;

        static ShaderProgram()
        {
            CurrentProgramId = 0;
        }

        readonly List<Shader> shaderObjects;

        public int ProgramId { get; private set; }

        public ShaderProgram(params Shader[] shaders)
        {
            uniforms = new Dictionary<string, int>();
            attributes = new Dictionary<string, int>();

            ProgramId = GL.CreateProgram();

            shaderObjects = new List<Shader>(shaders.Length);

            foreach (Shader s in shaders)
                Attach(s);
        }

        public void Use(Action a)
        {
            using (UseProgram())
                a();
        }

        public ShaderProgramHandle UseProgram()
        {
            ShaderProgramHandle handle = new ShaderProgramHandle(CurrentProgramId);

            if (CurrentProgramId != ProgramId)
            {
                GL.UseProgram(ProgramId);

                CurrentProgramId = ProgramId;
            }

            return handle;
        }

        public ShaderProgram Link()
        {
            GL.LinkProgram(ProgramId);

            int status;
            GL.GetProgram(ProgramId, GetProgramParameterName.LinkStatus, out status);

            if (status == 0)
            {
                String err = GL.GetProgramInfoLog(ProgramId);
                throw new Exception(string.Format("Could not link program: {0}", err));
            }

            return this;
        }

        public void Attach(Shader shader)
        {
            GL.AttachShader(ProgramId, shader.ShaderId);

            shaderObjects.Add(shader);
        }

        public void Detach(Shader shader)
        {
            if (shaderObjects.Contains(shader))
            {
                GL.DetachShader(ProgramId, shader.ShaderId);

                shaderObjects.Remove(shader);
            }
        }

        public void Dispose()
        {
            foreach (Shader s in shaderObjects)
            {
                Detach(s);

                s.Dispose();
            }

            if (ProgramId != -1)
                GL.DeleteProgram(ProgramId);

            shaderObjects.Clear();

            ProgramId = -1;
        }
    }
}
