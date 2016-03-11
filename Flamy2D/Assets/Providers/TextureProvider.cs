using System;
using Flamy2D.Graphics;

namespace Flamy2D.Assets.Providers
{
    public class TextureProvider : AssetHandler<Texture2D>
    {
        public TextureProvider(ContentManager mgr)
            : base(mgr, "Textures")
        { }

        public override Texture2D Load(string assetName, params object[] args)
        {
            return Texture2D.LoadFromFile(assetName, args.Length < 1 ? TextureConfiguration.Nearest : (TextureConfiguration)args[0]);
        }
    }
}
