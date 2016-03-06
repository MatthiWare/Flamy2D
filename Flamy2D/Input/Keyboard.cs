using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flamy2D.Input
{
    public class Keyboard
    {
        private List<KeyState> keys;

        public Keyboard()
        {
            keys = new List<KeyState>();
        }

        public bool IsKeyDown(Key k)
        {
            RegisterKey(k);

            return keys.First(state => k == state.Key).IsDown;
        }

        public bool IsAnyKeyDown(params Key[] ks)
        {
            return ks.Any(k => IsKeyDown(k));
        }

        public bool IsKeyUp(Key k)
        {
            return !IsKeyDown(k);
        }

        public bool IsAnyKeyUp(params Key[] ks)
        {
            return ks.Any(k => IsKeyUp(k));
        }

        internal void RegisterKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            RegisterKey(e.Key);

            keys.First(state => e.Key == state.Key).IsDown = true;
        }

        internal void RegisterKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            RegisterKey(e.Key);

            keys.First(state => e.Key == state.Key).IsDown = false;
        }

        private void RegisterKey(Key k)
        {
            if (keys.All(state => k != state.Key))
                keys.Add(new KeyState(k));
        }


    }
}
