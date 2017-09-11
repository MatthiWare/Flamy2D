using System;
using Flamy2D.Graphics;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Flamy2D.Assets.Providers
{
    public class TextureProvider : AssetHandler<Texture2D>
    {
        private ConcurrentDictionary<string, Texture2D> cache = new ConcurrentDictionary<string, Texture2D>();

        public TextureProvider(ContentManager mgr)
            : base(mgr, "Textures")
        { }

        public override Texture2D Load(string assetName, params object[] args)
        {
            return GetOrAdd(assetName, args.Length < 1 ? TextureConfiguration.Nearest : (TextureConfiguration)args[0]);
        }

        public override void Save(Texture2D asset, string path)
        {
            throw new NotImplementedException();
        }

        private Texture2D GetOrAdd(string name, TextureConfiguration config)
        {
            return cache.GetOrAdd(name, Texture2D.LoadFromFileAsync(name, config));
        }
    }
}
