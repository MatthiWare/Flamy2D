using System;

namespace Flamy2D
{
    public static class LogExtensions
    {
        public static void Log<T>(this T self, string log) where T : class
        {
            Console.WriteLine($"[{self.GetType().Name}]: {log}");
        }
    }
}
