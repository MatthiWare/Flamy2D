using Flamy2D.Extensions;
using System;

namespace Flamy2D
{
    public static class LogExtensions
    {
        public static void Log<T>(string log) where T : class
        {
            Console.WriteLine(log);
        }
    }
}
