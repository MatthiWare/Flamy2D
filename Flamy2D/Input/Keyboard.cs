using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flamy2D.Input
{
    public class Keyboard : IDisposable
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                keys.Clear();
                keys = null;

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Keyboard() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


    }
}
