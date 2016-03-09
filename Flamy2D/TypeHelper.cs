using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Flamy2D
{
    public static class TypeHelper
    {
        private static readonly ConcurrentDictionary<Type, int> _cache = new ConcurrentDictionary<Type, int>();

        public static int SizeOf(Type t)
        {
            if (t == null) throw new ArgumentNullException("t", "The specified Type cannot be null");

            return _cache.GetOrAdd(t, val => {
                DynamicMethod dm = new DynamicMethod("$", typeof(int), Type.EmptyTypes);
                ILGenerator il = dm.GetILGenerator();
                il.Emit(OpCodes.Sizeof, val);
                il.Emit(OpCodes.Ret);

                Func<int> func = (Func<int>)dm.CreateDelegate(typeof(Func<int>));
                return func();
            });
        }
    }
}
