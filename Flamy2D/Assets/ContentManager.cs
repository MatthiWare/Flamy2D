using Flamy2D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace Flamy2D.Assets
{
    public class ContentManager : ILog, IDisposable
    {

        private string contentRoot;

        public string ContentRoot
        {
            get { return contentRoot; }
            set { contentRoot = NormalizePath(value); }
        }

        public Dictionary<Type, object> AssetHandlers { get; private set; }

        public ContentManager(string root = "")
        {
            AssetHandlers = new Dictionary<Type, object>();
            ContentRoot = root;
        }

        public string NormalizePath(string path)
        {
            return path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        }

        public void RegisterAssetHandler<T>(Type type)
        {
            AssetHandlers[typeof(T)] = Activator.CreateInstance(type, new object[] { this });
        }

        public T Load<T>(string asset, params object[] args) where T : IAsset
        {
            if (AssetHandlers.ContainsKey(typeof(T)))
            {
                AssetHandler<T> provider = (AssetHandler<T>)AssetHandlers[typeof(T)];

                return LoadFrom<T>(provider.GetAssetPath(asset), args);
            }

            this.Log("Unsupported asset type ({0}).", typeof(T).Name);

            return default(T);
        }

        public T LoadFrom<T>(string path, params object[] args) where T :IAsset
        {
            if (AssetHandlers.ContainsKey(typeof(T)))
            {
                AssetHandler<T> provider = (AssetHandler<T>)AssetHandlers[typeof(T)];

                return provider.Load(path, args);
            }

            this.Log("Unsupported asset type ({0}).", typeof(T).Name);

            return default(T);
        }

        public void Save<T>(T asset, string assetPath)where T :IAsset
        {
            if (AssetHandlers.ContainsKey(typeof(T)))
            {
                AssetHandler<T> provider = (AssetHandler<T>)AssetHandlers[typeof(T)];

                SaveTo<T>(asset, provider.GetAssetPath(assetPath));

                return;
            }

            this.Log("Unsupported asset type ({0}).", typeof(T).Name);
        }

        public void SaveTo<T>(T asset, string assetPath) where T : IAsset
        {
            if (AssetHandlers.ContainsKey(typeof(T)))
            {
                AssetHandler<T> provider = (AssetHandler<T>)AssetHandlers[typeof(T)];
                provider.Save(asset, assetPath);
            }
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

                AssetHandlers.Clear();
                AssetHandlers = null;

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ContentManager() {
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
