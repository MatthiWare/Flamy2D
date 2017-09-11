using System;
using Flamy2D.Fonts;
using System.Collections.Concurrent;
using System.Drawing;
using System.Threading.Tasks;

namespace Flamy2D.Assets.Providers
{
    public class FontProvider : AssetHandler<BitmapFont>
    {
        private ConcurrentDictionary<string, BitmapFont> cache = new ConcurrentDictionary<string, BitmapFont>();

        public FontProvider(ContentManager mgr)
            : base(mgr, "Fonts")
        { }

        public override BitmapFont Load(string assetName, params object[] args) => cache.GetOrAdd(assetName,  BitmapFontLoader.Load(assetName));

        public override void Save(BitmapFont asset, string path)
        {
            throw new NotImplementedException();
        }
    }

   
}
