using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Flamy2D.Imaging
{
    public static class ImageLib
    {
        [DllImport("stb_image.dll")]
        private static extern IntPtr stbi_load(string filename, ref int x, ref int y, ref int n, int req_comp);

        [DllImport("stb_image.dll")]
        private static extern void stbi_image_free(IntPtr data);

        private static void SetUnmanagedDllDirectory()
        {
            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            path = Path.Combine(path, IntPtr.Size == 8 ? "win64 " : "win32");
            if (!SetDllDirectory(path)) throw new System.ComponentModel.Win32Exception();
        }

        /// <summary>
        /// Sets the path where the unmanaged libs are located. 
        /// </summary>
        /// <param name="path">The unmanaged libs path. </param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDllDirectory(string path);

        public static IntPtr Load(string filename, ref int x, ref int y, ref int n, int req_comp)
        {
         //   SetUnmanagedDllDirectory();
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                        return stbi_load(filename, ref x, ref y, ref n, req_comp);
                default:
                    throw new NotSupportedException("OS is not supported");
            }
        }

        public static void Free(IntPtr data)
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    stbi_image_free(data);
                    break;
                default:
                    throw new NotSupportedException("OS is not supported");
            }
        }

    }
}
