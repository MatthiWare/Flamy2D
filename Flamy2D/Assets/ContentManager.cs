using Flamy2D.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace Flamy2D.Assets
{
    public class ContentManager : ILog
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

            this.Log("Unspupported {0} asset type!", typeof(T).Name);

            return default(T);
        }

        public T LoadFrom<T>(string path, params object[] args) where T :IAsset
        {
            if (AssetHandlers.ContainsKey(typeof(T)))
            {
                AssetHandler<T> provider = (AssetHandler<T>)AssetHandlers[typeof(T)];

                return provider.Load(path, args);
            }

            this.Log("Unspupported {0} asset type!", typeof(T).Name);

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

            this.Log("Unspupported {0} asset type!", typeof(T).Name);
        }

        public void SaveTo<T>(T asset, string assetPath) where T : IAsset
        {
            if (AssetHandlers.ContainsKey(typeof(T)))
            {
                AssetHandler<T> provider = (AssetHandler<T>)AssetHandlers[typeof(T)];
                provider.Save(asset, assetPath);
            }
        }

    }
}
