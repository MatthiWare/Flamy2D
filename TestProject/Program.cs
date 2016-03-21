using Flamy2D;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace TestProject
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
           SetUnmanagedDllDirectory();

            GameConfiguration config = new GameConfiguration();
            config.FPSTarget = 60;
            config.FixedFPS = true;
            config.Resizable = false;

            TestGame game = new TestGame(config);
            game.Run();
        }

        public static void SetUnmanagedDllDirectory()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, IntPtr.Size == 8 ? "win64 " : "win32");
            if (!SetDllDirectory(path)) throw new System.ComponentModel.Win32Exception();
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDllDirectory(string path);
    }
}
