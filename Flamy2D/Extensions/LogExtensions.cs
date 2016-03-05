using System;

namespace Flamy2D.Extensions
{
    public static class LogExtensions
    {
        public static void Log<T>(this T self, string format, params object[] args)
            where T : class, ILog
        {
            Console.WriteLine("[{0}]: {1}", self.GetType().Name, string.Format(format, args));
        }
    }
}
