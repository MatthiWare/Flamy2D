using System;
using System.Collections.Generic;
using System.IO;

namespace Flamy2D.Assets
{
    public abstract class AssetHandler<T> where T : IAsset
    {
        public string AssetRoot { get; private set; }

        public ContentManager Manager { get; private set; }

        protected AssetHandler(ContentManager mgr, string root)
        {
            AssetRoot = root;
            Manager = mgr;
        }

        public string GetAssetPath(string asset)
        {
            return Manager.NormalizePath(Path.Combine(Manager.ContentRoot, AssetRoot, asset));
        }

        public abstract T Load(string assetName, params object[] args);

        public virtual void Save(T asset, string path)
        { }
    }
}
