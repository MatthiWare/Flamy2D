using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Flamy2D.Buffer
{
    public class GLBuffer<T> where T :struct
    {

        public int BufferId { get; private set; }
        public GLBufferSettings Settings { get; private set; }

        public int BufferSize { get; set; }
        public int ElementSize { get { return BufferSize / Buffer.Count; } }

        public IList<T> Buffer { get; set; }

        public GLBuffer(GLBufferSettings settings, IList<T> buffer)
        {
            Buffer = buffer;
            Settings = settings;

            BufferSize = Marshal.SizeOf(buffer[0]) * Buffer.Count;

            BufferId = GL.GenBuffer();

            Bind();
            {
                GL.BufferData(Settings.Target, BufferSize, Buffer.ToArray(), Settings.Hint);
            }
            Unbind();
        }

        public void PointTo(int where)
        {
            PointTo(where, Settings.Offset);
        }

        public void PointTo(int where, int offset)
        {
            Bind();
            {
                GL.EnableVertexAttribArray(where);

                GL.VertexAttribPointer(where, Settings.AttribSize, Settings.Type, Settings.Normalized, ElementSize, offset);
            }
            Unbind();
        }

        public void PointTo(int where, params int[] other)
        {
            if (other == null)
                throw new ArgumentNullException("other");

            if (other.Length < 2)
                throw new ArgumentException("Attribute Pointer contains less than 2 elements", "other");

            Bind();
            {
                GL.EnableVertexAttribArray(where);

                GL.VertexAttribPointer(where, other[0], Settings.Type, Settings.Normalized, ElementSize, other[1]);
            }
            Unbind();
        }

        public void Bind()
        {
            Bind<T>(this);
        }

        public static void Bind<Buff>(GLBuffer<Buff> buffer) where Buff : struct
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "Cannot bind 'null' buffer");

            GL.BindBuffer(buffer.Settings.Target, buffer.BufferId);
        }

        public static void Unbind<Buff>(GLBuffer<Buff> buffer) where Buff : struct
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "Cannot bind 'null' buffer");

            GL.BindBuffer(buffer.Settings.Target, 0);
        }

        public void Unbind()
        {
            Unbind(this);
        }
    }
}
