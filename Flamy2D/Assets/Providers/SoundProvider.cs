using Flamy2D.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flamy2D.Assets.Providers
{
    public class SoundProvider : AssetHandler<Sound>
    {
        private Dictionary<string, Sound> soundCache = new Dictionary<string, Sound>();

        public SoundProvider(ContentManager mgr)
            : base(mgr, "sounds")
        { }

        public override Sound Load(string assetName, params object[] args)
        {
            return GetSound(assetName);
        }

        private Sound GetSound(string sound)
        {
            if (!soundCache.ContainsKey(sound))
                soundCache.Add(sound, new Sound(sound));
            return soundCache[sound];
        }
    }
}
