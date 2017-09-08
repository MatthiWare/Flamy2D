using Flamy2D;
using Flamy2D.Fonts;
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
           //SetUnmanagedDllDirectory();

            string test = "info face=\"LVDC Common2\" size=32 bold=0 italic=0 charset=\"\" unicode=1 stretchH=100 smooth=1 aa=4 padding=0,0,0,0 spacing=1,1 outline=0";
            foreach (string x in BitmapFontLoader.Split(test, ' '))
                Console.WriteLine(x);

           

            GameConfiguration config = new GameConfiguration();
            config.FPSTarget = 60;
            config.FixedFPS = true;
            config.Resizable = true;
            config.VSync = VSyncMode.Off;

            TestGame game = new TestGame(config);
            //game.Run();
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
