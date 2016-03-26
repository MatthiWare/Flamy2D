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
