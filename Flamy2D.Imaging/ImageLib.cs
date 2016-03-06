using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Flamy2D.Imaging
{
    public static class ImageLib
    {
        public static IntPtr Load(string filename, ref int x, ref int y, ref int n, int req_comp)
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    if (Environment.Is64BitProcess)
                        return Win64.stbi_load(filename, ref x, ref y, ref n, req_comp);
                    else
                        return Win32.stbi_load(filename, ref x, ref y, ref n, req_comp);
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
                    if (Environment.Is64BitProcess)
                        Win64.stbi_image_free(data);
                    else
                        Win32.stbi_image_free(data);
                    break;
                default:
                    throw new NotSupportedException("OS is not supported");
            }
        }

        class Win64
        {
            [DllImport("stb_image-win64.dll")]
            public static extern IntPtr stbi_load(string filename, ref int x, ref int y, ref int n, int req_comp);

            [DllImport("stb_image-win64.dll")]
            public static extern void stbi_image_free(IntPtr data);
        }

        class Win32
        {
            [DllImport("stb_image-win32.dll")]
            public static extern IntPtr stbi_load(string filename, ref int x, ref int y, ref int n, int req_comp);

            [DllImport("stb_image-win32.dll")]
            public static extern void stbi_image_free(IntPtr data);
        }

    }
}
