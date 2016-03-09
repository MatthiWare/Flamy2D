using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK;
using OpenTK.Graphics;
using System.Drawing;

namespace Flamy2D.Graphics.Shaders
{
    public partial class ShaderProgram
    {

        Dictionary<string, int> uniforms;

		public object this[string uniform]
        {
            get { return GetUniform(uniform); }
			set { SetUniform(uniform, value); }
        }

		public object this[int id]
        {
			set { SetUniformValue(id, value); }
        }

        public int GetUniform(string uniform)
        {
            if (!uniforms.ContainsKey(uniform))
                uniforms[uniform] = GL.GetUniformLocation(ProgramId, uniform);

            return uniforms[uniform];
        }

        public void SetUniform(string uniform, object value)
        {
            if (CurrentProgramId != 0 && CurrentProgramId != ProgramId)
            {
                throw new System.Exception("An other program is already loaded!");
            }

            this[(int)this[uniform]] = value;
        }

        public void SetUniformValue(int uniformId, object value)
        {
           if(CurrentProgramId != 0 && CurrentProgramId != ProgramId)
            {
                throw new System.Exception("An other program is already loaded!");
            }

            using (UseProgram())
            {
				TypeSwitch.On(value)
					.Case((int x) => GL.Uniform1(uniformId, x))
					.Case((uint x)=>GL.Uniform1(uniformId, x))
                    .Case((float x) => GL.Uniform1(uniformId, x))
                    .Case((Vector2 x) => GL.Uniform2(uniformId, x))
                    .Case((Vector3 x) => GL.Uniform3(uniformId, x))
                    .Case((Vector4 x) => GL.Uniform4(uniformId, x))
                    .Case((Quaternion x) => GL.Uniform4(uniformId, x))
                    .Case((Color4 x) => GL.Uniform4(uniformId, x))
                    .Case((int[] x) => GL.Uniform1(uniformId, x.Length, x))
                    .Case((uint[] x) => GL.Uniform1(uniformId, x.Length, x))
                    .Case((float[] x) => GL.Uniform1(uniformId, x.Length, x))
                    .Case((Matrix4 x) => GL.UniformMatrix4(uniformId, false, ref x))
                    .Case((Color x) => GL.Uniform4(uniformId, x.R, x.G, x.B, x.A))
                    .Default(x => Throw("GLUniform type {0} is not is not implemented.", value.GetType()));
            } 
        }

        private void Throw(string x, object y)
        {
            throw new InvalidCastException(string.Format(x, y));
        }

    }
}
