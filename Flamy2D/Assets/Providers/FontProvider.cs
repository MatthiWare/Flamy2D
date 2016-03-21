using System;
using Flamy2D.Fonts;
using System.Collections.Generic;

namespace Flamy2D.Assets.Providers
{
    public class FontProvider : AssetHandler<Font>
    {

        Dictionary<Tuple<string, float>, Font> cache = new Dictionary<Tuple<string, float>, Font>();

        public FontProvider(ContentManager mgr)
            : base(mgr, "Fonts")
        { }

        public override Font Load(string assetName, params object[] args)
        {
            return GetFont(assetName, args.Length >= 1 ? (float)args[0] : 12f);
        }

        private Font GetFont(string name, float size)
        {
            Tuple<string, float> f = new Tuple<string, float>(name, size);
            if (!cache.ContainsKey(f))
                cache.Add(f, new Font(name, size));
            return cache[f];
        }
    }
}
