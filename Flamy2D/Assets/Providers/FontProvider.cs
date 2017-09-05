using System;
using Font = Flamy2D.Fonts.Font;
using System.Collections.Generic;
using System.Drawing;

namespace Flamy2D.Assets.Providers
{
    public class FontProvider : AssetHandler<Font>
    {
        private Dictionary<Tuple<string, float, FontStyle>, Font> cache = new Dictionary<Tuple<string, float, FontStyle>, Font>();

        public FontProvider(ContentManager mgr)
            : base(mgr, "Fonts")
        { }

        public override Font Load(string assetName, params object[] args)
        {
            float size = args.Length >= 1 ? (float)args[0] : 12f;
            FontStyle fs = args.Length >= 2 ? (FontStyle)args[1] : FontStyle.Regular;

            return GetFont(assetName, size, fs);
        }

        private Font GetFont(string name, float size,FontStyle fs)
        {
            var f = new Tuple<string, float, FontStyle>(name, size, fs);

            if (!cache.ContainsKey(f))
                cache.Add(f, new Font(name, size, fs));

            return cache[f];
        }
    }

   
}
